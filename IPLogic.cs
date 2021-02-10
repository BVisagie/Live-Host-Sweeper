using LiveHostSweeper.Enums;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace LiveHostSweeper
{
    internal static class IPLogic
    {
        public static void PresentAndHandleSinglePing()
        {
            Utilities.PrintToScreen(ConsoleColor.White, "Please provide a valid target IPv4 or IPv6 address range. (Example: 192.168.168.100/24, 2001:0db8::/64)");
            var targetIp = Console.ReadLine();

            while (!ValidateIp(targetIp))
            {
                Utilities.PrintToScreen(ConsoleColor.Green, $"{targetIp} is invalid, please provide a valid target IPv4 or IPv6 address range.");
                targetIp = Console.ReadLine();
            }

            using var ping = new Ping();

            try
            {
                PingReply reply = ping.Send(targetIp);

                switch (reply.Status)
                {
                    case IPStatus.Success:
                        Utilities.PrintPingResultsToScreen(ConsoleColor.Green, pingReply: reply);
                        break;

                    case IPStatus.DestinationPortUnreachable:
                    case IPStatus.DestinationNetworkUnreachable:
                    case IPStatus.DestinationHostUnreachable:
                        Utilities.PrintPingResultsToScreen(ConsoleColor.Yellow, pingReply: reply);
                        break;

                    default:
                        Utilities.PrintPingResultsToScreen(ConsoleColor.Red, pingReply: reply);
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }

            Navigation.PresentAndHandleExitOptions();
        }

        public static void PingRange()
        {
            Utilities.PrintToScreen(ConsoleColor.Cyan, "Please provide a valid target IPv4 or IPv6 address range. (Example: 192.168.168.100/24, 2001:0db8::/64)");
            var targetIpnetwork = Console.ReadLine();

            while (!ValidateIp(targetIpnetwork))
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{targetIpnetwork} is invalid, please provide a valid target IPv4 or IPv6 address range.", PaddingTypes.Top);
                targetIpnetwork = Console.ReadLine();
            }

            var ipDataset = RetrieveIpData(targetIpnetwork);

            Utilities.PrintToScreen(ConsoleColor.Green, "Press [1] to start a ping sweep of this IP range.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Green, "Press [2] to restart the application.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Green, "Press [3] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2 && userSelection != 3)
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-2.", PaddingTypes.Bottom);
                userSelection = Utilities.ValidateUserInputToInt();
            }

            switch (userSelection)
            {
                case 1:
                    Thread.Sleep(1);
                    break;

                case 2:
                    // Once you have the path you get the directory with:
                    var directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    // Starts a new instance of the program itself
                    System.Diagnostics.Process.Start($"{directory}\\LiveHostSweeper.exe");

                    // Closes the current process
                    Environment.Exit(0);
                    break;

                case 3:
                    Environment.Exit(0);
                    break;
            }

            //if (userSelection == 1)
            //{
            //    using var ping = new Ping();

            //    for (int i = 0; i < ipDataset.Usable; i++)
            //    {
            //        try
            //        {
            //            PingReply reply = ping.Send(ipDataset);

            //            switch (reply.Status)
            //            {
            //                case IPStatus.Success:
            //                    Utilities.PrintPingResultsToScreen(ConsoleColor.Green, pingReply: reply);
            //                    break;

            //                case IPStatus.DestinationPortUnreachable:
            //                case IPStatus.DestinationNetworkUnreachable:
            //                case IPStatus.DestinationHostUnreachable:
            //                    Utilities.PrintPingResultsToScreen(ConsoleColor.Yellow, pingReply: reply);
            //                    break;

            //                default:
            //                    Utilities.PrintPingResultsToScreen(ConsoleColor.Red, pingReply: reply);
            //                    break;
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            throw;
            //        }
            //    }
            //}
            //else
            //{
            //    Environment.Exit(0);
            //}
        }

        /// <summary>
        /// https://github.com/lduchosal/ipnetwork
        /// Tests whether given IP can be parsed.
        /// </summary>
        /// <param name="targetIpnetwork"></param>
        private static bool ValidateIp(string targetIpnetwork)
        {
            try
            {
                IPNetwork ipnetwork = IPNetwork.Parse(targetIpnetwork);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// https://github.com/lduchosal/ipnetwork
        /// Grab the data from the given IP.
        /// </summary>
        /// <param name="targetIpnetwork"></param>
        private static IPNetwork RetrieveIpData(string targetIpnetwork)
        {
            IPNetwork ipnetwork = IPNetwork.Parse(targetIpnetwork);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Network : {ipnetwork.Network}", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Netmask : {ipnetwork.Netmask}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Broadcast : {ipnetwork.Broadcast}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"FirstUsable : {ipnetwork.FirstUsable}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"LastUsable : {ipnetwork.LastUsable}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Usable : {ipnetwork.Usable}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Cidr : {ipnetwork.Cidr}", PaddingTypes.Bottom);
            return ipnetwork;
        }
    }
}