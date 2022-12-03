using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace Caligulatry1
{
    class Program
    {
        private static string token { get; set; } = "5815934880:AAHmEu3yr-V8Kpa47RhjZW_b8MSrcTvGLfU";

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
            const string botname = "@qqueue_bot";

            if (msg.Text != null)
            {
                switch (msg.Text.Split(" ")[0])
                {
                    case "/queues" + botname:
                    case "/queues":
                        foreach(var item in service.GetQueues(msg.Chat.Id))
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, item.ToString());
                        }
                        break;
                    case "/addqueue" + botname:
                    case "/addqueue":
                        queue = new Queue(msg.Chat.Id, user, msg.Text.Replace(msg.Text.Split(" ")[0], ""), DateTime.Now);
                        response = service.AddQueue(queue);
                        if (response)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Queue created!");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Name is empty or queue with that name already exists");
                        }
                        queue = new Queue();
                        break;
                    case "/addme" + botname:
                    case "/addme":
                        if (msg.Text.Split(" ").Length < 2)
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
                    case "/removeme":
                        response = service.RemoveUser(msg.Chat.Id, msg.Text.Replace(msg.Text.Split(" ")[0], ""), user);
                        if (response)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Removed you from the queue");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Queue with such a name do not exist or you are not in it");
                        }
                        break;

                    case "/deletequeue" + botname: 
                    case "/deletequeue":
                        response = service.DeleteQueue(msg.Chat.Id, msg.Text.Replace(msg.Text.Split(" ")[0], ""), user);
                        if (response)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Queue Deleted");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "You are not creator or such a queue do not exists");
                        }
                        break;
                }
            }
        }
    }
}
