using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    class Program
    {
        static Api Bot;
        static int AdminId = 23523547;

        static List<BotCommand> commands = new List<BotCommand>();

        static void Main(string[] args)
        {
            Bot = new Api("5692855249:AAEeC-oIAWfzjDol2v-y6IEmttPFE2GN0wo");

            commands.Add(new BotCommand
            {
                Command = "/start",
                CountArgs = 0,
                Example = "/start",
                Execute = async (model, update) =>
                {
                    await Bot.SendTextMessage(update.Message.From.Id, "Привет, автор бота Andrew.\n" +
                        "Вот список всех команд:\n" +
                        string.Join("\n", commands.Select(s => s.Example)));
                },
                OnError = async (model, update) =>
                {


                    await Bot.SendTextMessage(update.Message.From.Id, "Не верное количество аргументов\nИспользуйте комнду так: /start");

                }
            });

            commands.Add(new BotCommand
            {
                Command = "/help",
                CountArgs = 0,
                Example = "/help",
                Execute = async (model, update) =>
                {
                    await Bot.SendTextMessage(update.Message.From.Id, string.Join("\n", commands.Select(s => s.Example)));
                },
                OnError = async (model, update) =>
                {


                    await Bot.SendTextMessage(update.Message.From.Id, "Не верное количество аргументов\nИспользуйте комнду так: /start");

                }
            });
            commands.Add(new BotCommand
            {
                Command = "/run",
                CountArgs = 1,
                Example = "/run[path|url]",
                Execute = async (model, update) =>
                {
                    try
                    {
                        Process.Start(model.Args.FirstOrDefault());
                        await Bot.SendTextMessage(update.Message.From.Id, "Завдання виконано!");
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessage(update.Message.From.Id, "Виникла помилка:" + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessage(update.Message.From.Id, "Не верное количество аргументов\nИспользуйте комнду так: /run[path|url]");
                }
            });

            Run().Wait();

            Console.ReadKey();
        }


        static async Task Run()
        {
            await Bot.SendTextMessage(AdminId, $"Запущен бот: {Environment.UserName}");

            var offset = 0;

            while (true)
            {
                var updates = await Bot.GetUpdates(offset);

                foreach (var update in updates)
                {
                    if (update.Message.From.Id == AdminId)
                    {
                        if (update.Message.Type == MessageType.TextMessage)
                        {
                            var model = BotCommand.Parse(update.Message.Text);

                            if (model != null)
                            {
                                foreach (var cmd in commands)
                                {
                                    if (cmd.Command == model.Command)
                                    {
                                        if (cmd.CountArgs == model.Args.Length)
                                        {
                                            cmd.Execute?.Invoke(model, update);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessage(update.Message.From.Id, "Це не команда\nДля просмотра списка команд ведіть /help");
                            }
                        }
                    }
                    else
                    {
                        await Bot.SendTextMessage(update.Message.From.Id, "Я создан тільки для свого керівника");
                    }
                    offset = update.Id + 1;

                }

                Task.Delay(500).Wait();
            }
        }
    }
}

