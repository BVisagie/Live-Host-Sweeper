using LiveHostSweeper.Enums;
using System;

namespace LiveHostSweeper
{
    internal static class Navigation
    {
        /// <summary>
        /// Presents main menu options and interprets user input.
        /// </summary>
        public static MainMenuOptions PresentAndHandleMainMenuOptions()
        {
            Utilities.PrintToScreen(ConsoleColor.DarkYellow, "Live Host Sweeper", PaddingTypes.Top);

            Utilities.PrintToScreen(ConsoleColor.White, "Press [1] to perform a ping sweep of a given IPv4 or IPv6 address.", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.White, "Press [2] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2)
            {
                Utilities.PrintToScreen(ConsoleColor.White, $"{userSelection} is invalid, please make a selection between 1-2");
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
            Utilities.PrintToScreen(ConsoleColor.White, "Press [1] to return to the main menu.", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.White, "Press [2] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2)
            {
                Utilities.PrintToScreen(ConsoleColor.White, $"{userSelection} is invalid, please make a selection between 1-2.", PaddingTypes.Bottom);
                userSelection = Utilities.ValidateUserInputToInt();
            }

            switch (userSelection)
            {
                case 1:
                    //once you have the path you get the directory with:
                    var directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    // Starts a new instance of the program itself
                    System.Diagnostics.Process.Start($"{directory}\\LiveHostSweeper.exe");

                    // Closes the current process
                    Environment.Exit(0);
                    break;

                case 2:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}