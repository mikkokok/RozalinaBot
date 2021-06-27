using Microsoft.Extensions.Configuration;
using RozalinaBot.Collectors.Ouman;
using RozalinaBot.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace RozalinaBot.InfoDeployers.Telegram
{
    internal class RozalinaBot
    {
        private static TelegramBotClient _botClient;
        private const string TuxFile = "Files/tux.png";
        private static OumanCollector _oumanCollector;
        private IConfiguration _config;
        private List<OumanUser> _oumanUsers;

        public RozalinaBot(IConfiguration config)
        {
            _config = config;
            _botClient = new TelegramBotClient(_config["Telegram:TelegramToken"]);
            _oumanCollector = new OumanCollector(_config);
            _oumanUsers = _config.GetSection("Telegram").GetSection("OumanRegisteredUsers").Get<List<OumanUser>>();
            InitListeners();
        }
        private void InitListeners()
        {
            _botClient.OnMessage += BotClient_OnMessage;
            _botClient.StartReceiving();
        }

        private async void BotClient_OnMessage(object sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.Text) || e.Message.From != null && !IsRegisteredUser(e.Message.From))
                return;
            var message = e.Message;

            switch (message.Text.Split(' ').First())
            {
                case "/getOuman":
                    await SendOumanReadings(message.From.Id);
                    break;
                case "/startOuman":
                    _oumanCollector.StartPolling();
                    await SendMessage("Ouman polling started", message.From.Id);
                    break;
                case "/stopOuman":
                    _oumanCollector.StopPolling();
                    await SendMessage("Ouman polling stopped", message.From.Id);
                    break;
                case "/photo":
                    await SendTuxFile(message.Chat.Id);
                    break;
                //case "/setCatLitter":
                //    await _storageCollector.UpdateCatLitterTime();
                //    await SendMessage("Cat litter time set", message.From.Id);
                //    break;
                //case "/getCatLitter":
                //    var time = await _storageCollector.GetCatLitterTime();
                //    await SendMessage($"Last time was {time}", message.From.Id);
                //    break;
                case "/readme":
                case "/Readme":
                case "/":
                case "/Usage":
                    await ReplyUsage(message.From.Id);
                    break;
                default:
                    await SendAdminMessages(message.Text, message.From.Username);
                    break;
            }
        }

        private static string ComposeMessage(string from, string query)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Message from {from}");
            sb.Append($"Message: {query}");
            return sb.ToString();
        }
        public async Task SendAdminMessages(string message, string from)
        {
            var sb = new StringBuilder(message);
            sb.AppendLine($"from: {from}");
            foreach (var adminId in GetAdmins())
            {
                await _botClient.SendTextMessageAsync(adminId, message);
            }
        }
        public async Task SendToAll(string message, string from = "")
        {
            var sb = new StringBuilder(message);
            if (!string.IsNullOrEmpty(from))
                sb.AppendLine($"from: {from}");
            foreach (var adminId in GetAllUsers())
            {
                await _botClient.SendTextMessageAsync(adminId, message);
            }
        }
        private static async Task SendOumanReadings(int sendToId)
        {
            var readings = _oumanCollector.LastResult;
            await SendMessage(readings, sendToId);
        }
        private static async Task SendTuxFile(long chatId)
        {
            await _botClient.SendChatActionAsync(chatId, ChatAction.UploadPhoto);

            var fileName = Path.GetFileNameWithoutExtension(TuxFile);

            using (var fileStream = new FileStream(TuxFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileToSend = new InputOnlineFile(fileStream, fileName);
                await _botClient.SendPhotoAsync(chatId, fileToSend, "Nice Picture");
            }
        }
        private bool IsRegisteredUser(User user)
        {
            return _oumanUsers.Any(registeredUser => registeredUser.Id == user.Id && registeredUser.Username.Equals(user.Username));
        }

        private IEnumerable<int> GetAdmins()
        {
            return (from user in _oumanUsers where user.IsAdmin select user.Id).ToList();
        }
        private IEnumerable<int> GetAllUsers()
        {
            return _oumanUsers.Select(u => u.Id);
        }

        private static async Task ReplyUsage(int replyToId)
        {
            await _botClient.SendTextMessageAsync(replyToId, GetUsage());
        }

        private static string GetUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            sb.AppendLine("/getOuman - send latest Ouman readings");
            sb.AppendLine("/startOuman - start reading Ouman readings");
            sb.AppendLine("/stopOuman - stop reading Ouman readings");
            sb.AppendLine("/photo - send a photo");
            //sb.AppendLine("/getCatLitter - Get last time");
            //sb.AppendLine("/setCatLitter - Set last time");
            sb.AppendLine("/Usage /Readme - send this how-to");

            return sb.ToString();
        }
        private static async Task SendMessage(string message, int replyToId)
        {
            await _botClient.SendTextMessageAsync(replyToId, message);
        }
    }
}
