using LiveHostSweeper.Enums;
using Serilog;
using System;
using System.IO;
using System.Net;

namespace LiveHostSweeper
{
    internal static class Navigation
    {
        /// <summary>
        /// Presents main menu options and interprets user input.
        /// </summary>
        public static MainMenuOptions PresentAndHandleMainMenuOptions()
        {
            Utilities.PrintToScreen(ConsoleColor.DarkCyan, "Live Host Sweeper", PaddingTypes.Top);

            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [1] to perform a ping sweep of a given IPv4 or IPv6 address.", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [2] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2)
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-2");
                userSelection = Utilities.ValidateUserInputToInt();
            }

            return userSelection switch
            {
                1 => MainMenuOptions.IPv4,
                2 => MainMenuOptions.Exit,
                _ => MainMenuOptions.Unknown,
            };
        }

        public static void PresentAndHandleExitOptions()
        {
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [1] to restart the application.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [2] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2)
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-3.");
                userSelection = Utilities.ValidateUserInputToInt();
            }

            switch (userSelection)
            {
                case 1:
                    Utilities.RestartApplication();
                    break;

                case 2:
                    Environment.Exit(0);
                    break;
            }
        }

        public static IPNetwork IpDataSetOptions(ILogger logger)
        {
            Utilities.PrintToScreen(ConsoleColor.DarkCyan, "Please provide a valid target IPv4 or IPv6 address range. (Example: 192.168.0.143, 192.168.168.100/24, 2001:0db8::/64)", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.DarkCyan, "Please note that currently pinging more than 255 hosts is not supported :)", PaddingTypes.Bottom);
            var targetIpnetwork = Console.ReadLine();

            while (!Logic.ValidateIp(targetIpnetwork))
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{targetIpnetwork} is invalid, please provide a valid target IPv4 or IPv6 address range.");
                targetIpnetwork = Console.ReadLine();
            }

            return Logic.RetrieveIpData(targetIpnetwork, logger);
        }

        public static int IpOptions()
        {
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [1] to start a ping sweep of this IP range.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [2] to restart the application.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [3] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2 && userSelection != 3)
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-3.");
                userSelection = Utilities.ValidateUserInputToInt();
            }

            return userSelection;
        }

        public static int IpSearchOptions()
        {
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [1] to log the entire ping sweep to console and file.", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [2] to log only succesfull pings to console and file.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [3] to restart the application.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Gray, "Press [4] to exit.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Yellow, $"Your log file should be located here: {new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName}", PaddingTypes.Full);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2 && userSelection != 3 && userSelection != 4)
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-3.", PaddingTypes.Bottom);
                userSelection = Utilities.ValidateUserInputToInt();
            }

            return userSelection;
        }
    }
}