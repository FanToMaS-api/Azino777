using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using Server.Configuration;
using Server.Telegram.Impl;
using Server.Telegram.Service;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var fullPath = Directory.GetFiles(path)
                .FirstOrDefault(x => x.EndsWith("nlog.config", StringComparison.OrdinalIgnoreCase));
            LogManager.Configuration = new XmlLoggingConfiguration(fullPath);

            var token = ConfigurationManager.AppSettings.Get(Config.TELEGRAM_TOKEN);

            using ITelegramService service = new TelegramService(token);
            service.Initialize();
            var task = Task.Factory.StartNew(async () =>
            {
                await service.StartAsync();
            });

            var task1 = Task.Factory.StartNew(() =>
            {
                while (true)
                { }
            });

            Task.WaitAll(task1);
            LogManager.Shutdown();
        }
    }
}
