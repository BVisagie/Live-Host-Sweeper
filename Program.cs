using LiveHostSweeper.Enums;
using System;

namespace LiveHostSweeper
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            StartupMethods.SetupBaseWindow();
            //var fileLogger = StartupMethods.StartFileLogger();

            switch (Navigation.PresentAndHandleMainMenuOptions())
            {
                case MainMenuOptions.IPv4:
                case MainMenuOptions.IPv6:
                    IPLogic.PingRange();
                    break;

                case MainMenuOptions.Exit:

                case MainMenuOptions.Unknown:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}