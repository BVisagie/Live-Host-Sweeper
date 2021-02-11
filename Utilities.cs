using LiveHostSweeper.Enums;
using Serilog;
using System;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;

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
                Console.WriteLine($"\n{userInput} is invalid, please enter a valid numeric character.\n");
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

        public static void PrintPingResultsToScreen(ConsoleColor consoleColor, PingReply pingReply, ILogger logger, string targetIp, bool logOnlySuccess)
        {
            if (pingReply.Status == IPStatus.Success)
            {
                PrintToScreen(consoleColor, $"{pingReply.Status}                              | {targetIp}       | {pingReply.RoundtripTime}", PaddingTypes.None);
                logger.Information($"{pingReply.Status}                              | {targetIp}       | {pingReply.RoundtripTime}");
            }
            else
            {
                if (logOnlySuccess)
                {
                    PrintToScreen(consoleColor, $"{pingReply.Status}           | {targetIp}       | {pingReply.RoundtripTime}", PaddingTypes.None);
                }
                else
                {
                    PrintToScreen(consoleColor, $"{pingReply.Status}           | {targetIp}       | {pingReply.RoundtripTime}", PaddingTypes.None);
                    logger.Information($"{pingReply.Status}           | {targetIp}       | {pingReply.RoundtripTime}");
                }
            }
        }

        public static void PrintToScreen(ConsoleColor consoleColor, string line, PaddingTypes paddingTypes = PaddingTypes.Full)
        {
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
    }
}