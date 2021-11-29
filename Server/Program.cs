using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using Server.Telegram;

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

            using var bot = TelegramBotFactory.Create();

            // TODO: ИЗМЕНИТЬ!
            var task = Task.Factory.StartNew(() => 
            {
                while (true)
                { }
            });

            Task.WaitAll(task);
            LogManager.Shutdown();
        }
    }
}
