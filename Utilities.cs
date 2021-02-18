using ConsoleTables;
using LiveHostSweeper.Enums;
using System;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LiveHostSweeper
{
    internal static class Utilities
    {
        public static int ValidateUserInputToInt()
        {
            var userInput = Console.ReadLine();
            int userSelection;

            while (!int.TryParse(userInput, out userSelection))
            {
                PrintToScreen(ConsoleColor.Red, $"{userInput} is invalid, please enter a valid numeric character.");
                userInput = Console.ReadLine();
            }

            return userSelection;
        }

        public static void CountDown(int seconds)
        {
            for (int timer = seconds; timer >= 0; timer--)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\r{0:00}\n", timer);
                Console.ResetColor();
                Thread.Sleep(1000);
            }
        }

        public static void PrintPingResultsToScreen(PingReply pingReply, string targetIp, bool logOnlySuccess, ConsoleTable consoleTable)
        {
            if (logOnlySuccess && pingReply.Status == IPStatus.Success)
            {
                consoleTable.AddRow(pingReply.Status, targetIp, pingReply.RoundtripTime);
            }
            else if (logOnlySuccess && pingReply.Status != IPStatus.Success)
            {
                //do no logging anywhere
            }
            else if (!logOnlySuccess)
            {
                consoleTable.AddRow(pingReply.Status, targetIp, pingReply.RoundtripTime);
            }
        }

        public static void PrintToScreen(ConsoleColor consoleColor, string line, PaddingTypes paddingTypes = PaddingTypes.Full, bool overwritePreviousLine = false)
        {
            if (overwritePreviousLine)
            {
                ClearCurrentConsoleLine();
            }

            Console.ForegroundColor = consoleColor;

            switch (paddingTypes)
            {
                case PaddingTypes.Full:
                    Console.WriteLine($"\n{line}\n");
                    break;

                case PaddingTypes.Top:
                    Console.WriteLine($"\n{line}");
                    break;

                case PaddingTypes.Bottom:
                    Console.WriteLine($"{line}\n");
                    break;

                case PaddingTypes.None:
                    Console.WriteLine($"{line}");
                    break;
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Creates a short mostly unique id composed of only alphanumeric characters.
        /// https://stackoverflow.com/a/42026123/3324415
        /// </summary>
        public static string ShortUid()
        {
            return Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
        }

        public static void RestartApplication()
        {
            // Once you have the path you get the directory with:
            var directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Starts a new instance of the program itself
            System.Diagnostics.Process.Start($"{directory}\\LiveHostSweeper.exe");

            // Closes the current process
            Environment.Exit(0);
        }

        /// <summary>
        /// https://stackoverflow.com/a/5027364/3324415
        /// </summary>
        private static void ClearCurrentConsoleLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <summary>
        /// https://stackoverflow.com/a/48345459/3324415
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="maxValue"></param>
        public static string CalculatePercentage(int currentValue, int maxValue)
        {
            //Calculate percentage
            decimal percentage = (decimal)currentValue / maxValue;

            //Render percentage
            return $"{percentage:P}";
        }

        /// <summary>
        /// Based on given current and total, return example: "55 seconds left." or "5 minutes and 40 seconds left."
        /// </summary>
        /// <param name="currentSeconds"></param>
        /// <param name="totalSeconds"></param>
        public static string CalculateTimeLeft(int currentSeconds, int totalSeconds)
        {
            int remainingSeconds = totalSeconds - currentSeconds;

            if (remainingSeconds <= 60)
            {
                return $"{remainingSeconds} seconds left.";
            }
            else
            {
                TimeSpan time = TimeSpan.FromSeconds(remainingSeconds);
                if (time.Minutes == 1)
                {
                    return $"{time.Minutes} minute and {time.Seconds} seconds left.";
                }
                else
                {
                    return $"{time.Minutes} minutes and {time.Seconds} seconds left.";
                }
            }
        }
    }
}