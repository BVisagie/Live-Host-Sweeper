using LiveHostSweeper.Enums;
using Serilog;
using System;

namespace LiveHostSweeper
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            StartupMethods.SetupBaseWindow();
            ILogger fileLogger = StartupMethods.StartFileLogger();
            fileLogger.Information("Logging started!");

            switch (Navigation.PresentAndHandleMainMenuOptions())
            {
                case MainMenuOptions.IPv4:
                case MainMenuOptions.IPv6:
                    Logic.PingRange(fileLogger);
                    break;

                case MainMenuOptions.Exit:

                case MainMenuOptions.Unknown:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}