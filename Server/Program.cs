using System;
using NLog;
using NLog.Config;
using Server.Telegram;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("../../../../NLog.config");

            var bot = TelegramBotFactory.Create();

            Console.ReadKey();
            bot.Dispose();
            LogManager.Shutdown();
        }
    }
}
