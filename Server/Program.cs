using System;
using NLog;
using NLog.Config;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("../../../../NLog.config");
            var logger = LogManager.GetCurrentClassLogger();

            LogManager.Shutdown();
        }
    }
}
