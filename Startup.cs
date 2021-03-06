﻿using Serilog;
using System;
using System.IO;

namespace LiveHostSweeper
{
    internal static class StartupMethods
    {
        public static ILogger StartFileLogger()
        {
            string errorDirectoryPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd");
            var filePathAndName = $"{errorDirectoryPath}\\LiveHostSweeper-{dateTime}";
            var shortGuid = Utilities.ShortUid();
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{filePathAndName}_{shortGuid}.txt", outputTemplate: "[{Timestamp:HH:mm:ss}][{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void SetupBaseWindow()
        {
            Console.Title = "Live Host Sweeper v1.0 | Feb 2021";
        }
    }
}