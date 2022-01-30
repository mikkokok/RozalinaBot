using Microsoft.Extensions.Configuration;
using RozalinaBot.Collectors.Ouman;
using RozalinaBot.Config;
using RozalinaBot.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace RozalinaBot.InfoDeployers.Impl
{
    internal class RozalinaBot : IRozalinaBot
    {
        private static TelegramBotClient _botClient;
        private const string TuxFile = "Files/tux.png";
        private static OumanCollector _oumanCollector;
        private IConfiguration _config;
        private List<OumanUser> _oumanUsers;
        private string _doorBellUrl;

        public RozalinaBot(IConfiguration config)
        {
            _config = config;
            _botClient = new TelegramBotClient(_config["Telegram:TelegramToken"]);
            _doorBellUrl = _config["doorBellPictureUrl"];
            _oumanCollector = new OumanCollector(_config);
            _oumanUsers = _config.GetSection("Telegram").GetSection("OumanRegisteredUsers").Get<List<OumanUser>>();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
                case "/getDoorBell":
                    await SendDoorBellPicture(message.From.Id);
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
                case "/HowTo":
                case "/how":
                case "/usage":
                case "/":
                case "/Usage":
                    await ReplyUsage(message.From.Id);
                    break;
                default:
                    await SendAdminMessages(message.Text, message.From.Username);
                    break;
            }
        }
        public async Task SendAdminMessages(string message, string from)
        {
            var sb = new StringBuilder(message);
            sb.Append("from: ").AppendLine(from);
            foreach (var adminId in GetAdmins())
            {
                await _botClient.SendTextMessageAsync(adminId, message);
            }
        }
        public async Task SendToAll(string message, string from = "")
        {
            var sb = new StringBuilder(message);
            if (!string.IsNullOrEmpty(from))
                sb.Append("from: ").AppendLine(from);
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
            sb.AppendLine("/getDoorBell - send a photo from front door");
            //sb.AppendLine("/getCatLitter - Get last time");
            //sb.AppendLine("/setCatLitter - Set last time");
            sb.AppendLine("/Usage /Readme - send this how-to");

            return sb.ToString();
        }
        private static async Task SendMessage(string message, int replyToId)
        {
            await _botClient.SendTextMessageAsync(replyToId, message);
        }

        private async Task SendDoorBellPicture(int sendToId)
        {
            //await _botClient.SendChatActionAsync(sendToId, ChatAction.UploadPhoto);

            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync("http://nikalink.kokkonen.pro:8765/picture/1/current/");
                using var stream = await response.Content.ReadAsStreamAsync();
                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);
                memStream.Position = 0;
                //bitmap.SetSource(memStream.AsRandomAccessStream());
                var fileToSend = new InputOnlineFile(memStream, "doorbell.jpeg");
                await _botClient.SendPhotoAsync(sendToId, fileToSend, TimeConverter.GetCurrentTimeAsString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
