using LiveHostSweeper.Enums;
using System;
using System.Net.NetworkInformation;
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

        public static void PrintPingResultsToScreen(ConsoleColor consoleColor, PingReply pingReply)
        {
            Console.WriteLine("\n=====================================================================================================");
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"\nPing results are in, status: {pingReply.Status} for IP: {pingReply.Address}. Round trip time (ms): {pingReply.RoundtripTime} \n");
            Console.ResetColor();
            Console.WriteLine("=====================================================================================================\n");
            CountDown(seconds: 5);
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
    }
}