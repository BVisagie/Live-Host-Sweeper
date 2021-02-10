using Serilog;
using System;
using System.IO;

namespace LiveHostSweeper
{
    internal static class StartupMethods
    {
        public static ILogger StartConsoleLogger()
        {
            return new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}][{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static ILogger StartFileLogger()
        {
            string errorDirectoryPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd");
            var filePathAndName = $"{errorDirectoryPath}\\LiveHostSweeper-{dateTime}";
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{filePathAndName}.txt", outputTemplate: "[{Timestamp:HH:mm:ss}][{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void SetupBaseWindow()
        {
            Console.Title = "Live Host Sweeper (Feb 2021)";
        }
    }
}