using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Caligulatry1
{
    class Program
    {
        private static string token { get; set; } = "1810086325:AAFEWIyvQo9MwmzHqvTBZjo42vqh9xBdeEs";

        private static TelegramBotClient client;

        private static Queue queue;

        [Obsolete]
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            ITelegramBotClient bot = (ITelegramBotClient)sender;
            var me = bot.GetMeAsync().Result;
            var user = e.Message.From.Username;
            var msg = e.Message;
            QueueService service = new QueueService();
            bool response;
            const string botname = "@PaterPatriae_bot";

            if (msg.Text != null)
            {
                switch (msg.Text.Split(" ")[0])
                {
                    case "/queues@PaterPatriae_bot":
                        foreach(var item in service.GetQueues(msg.Chat.Id))
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, item.ToString());
                        }
                        break;
                    case "/add_queue" + botname:
                        await client.SendTextMessageAsync(msg.Chat.Id, "Enter queue name");
                        break;
                    case "/name" + botname:
                        await client.SendTextMessageAsync(msg.Chat.Id, "List created!");
                        queue = new Queue(msg.Chat.Id, user, msg.Text.Replace(msg.Text.Split(" ")[0], ""), DateTime.Now);
                        service.AddQueue(queue);
                        queue = new Queue();
                        break;
                    case "/addme" + botname:
                        if(msg.Text.Split(" ").Length < 2)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "You need to enter queue name in format `/addme queue_name`");
                            break;
                        }
                        response = service.AddUser(msg.Chat.Id, msg.Text.Replace(msg.Text.Split(" ")[0], ""), user);
                        if (response)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Added you to the queue");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Queue with such a name do not exist or you are already in it");
                        }
                        break;

                    case "/removeme" + botname:
                        response = service.RemoveUser(msg.Chat.Id, msg.Text.Replace(msg.Text.Split(" ")[0], ""), user);
                        if (response)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Removed you from the queue");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Queue with such a name do not exist or you are already in it");
                        }
                        break;
                }
            }
        }

        private static IReplyMarkup GetButtons()
        {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new List<List<KeyboardButton>> {
                         new List<KeyboardButton>{new KeyboardButton { Text = "/lists"}, new KeyboardButton { Text = "/add_list"} }
                    }

                };
        }

        private static IReplyMarkup GetListButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>> {
                         new List<KeyboardButton>{new KeyboardButton { Text = "/name"}, new KeyboardButton { Text = "Add List"} }
                }

            };
        }
    }
}
