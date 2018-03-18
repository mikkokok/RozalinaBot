using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RozalinaBot.Collectors.Ouman;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RozalinaBot.InfoDeployers.Telegram
{
    internal class RozalinaBot
    {
        private static TelegramBotClient _botClient;
        private static List<User> _registeredUsers;
        private const string TuxFile = @"Files/tux.png";
        private static OumanCollector _oumanCollector;

        public RozalinaBot(string token, string oumanurl)
        {
            _botClient = new TelegramBotClient(token);
            _registeredUsers = new List<User>();
            _oumanCollector = new OumanCollector(oumanurl);
            InitListeners();
        }
        private void InitListeners()
        {
            _botClient.OnMessage += BotClient_OnMessage;
            _botClient.OnInlineQuery += BotClient_OnInlineQuery;

            _botClient.StartReceiving();
        }

        private void BotClient_OnInlineQuery(object sender, global::Telegram.Bot.Args.InlineQueryEventArgs e)
        {
            Console.WriteLine($"Got message from: {e.InlineQuery.From}");
            Console.WriteLine($"Got message query: {e.InlineQuery.Query}");
            Console.WriteLine($"Got message id: {e.InlineQuery.Id}");
        }

        private async void BotClient_OnMessage(object sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.Text) || e.Message.Type != MessageType.TextMessage)
                return;
            var message = e.Message;

            RegisterUser(message.From);

            switch (message.Text.Split(' ').First())
            {
                case "/getOuman":
                    await SendOumanReadings(message.From.Id);
                    break;

                case "/photo":
                    await SendTuxFile(message.Chat.Id);
                    break;

                default:
                    await ReplyUsage(message.From.Id);
                    break;
            }

        }
        private async Task SendOumanReadings(int sendToId)
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
                var fileToSend = new FileToSend(fileName, fileStream);
                await _botClient.SendPhotoAsync(chatId, fileToSend, "Nice Picture");
            }
        }

        private async Task ReplyUsage(int replyToId)
        {
            await _botClient.SendTextMessageAsync(replyToId, GetUsage());
        }

        private static string GetUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Usage:");
            sb.AppendLine("/getOuman - send latest Ouman readings");
            sb.AppendLine("/photo - send a photo");
            sb.AppendLine("/Usage - send this usage");

            return sb.ToString();
        }

        private void RegisterUser(User user)
        {
            if (_registeredUsers.Contains(user))
                return;
            _registeredUsers.Add(user);
        }

        private async Task SendMessage(string message, int replyToId)
        {
            await _botClient.SendTextMessageAsync(replyToId, message);
        }
        private List<string> GetChatIds()
        {
            var ids = new List<string>();
            return ids;
        }
        static async Task TestApiAsync()
        {
            var me = await _botClient.GetMeAsync();
            System.Console.WriteLine($"Hello! My name is {me.FirstName}");
        }
    }
}
