using LiveHostSweeper.Enums;
using Serilog;
using System;
using System.Net;
using System.Net.NetworkInformation;

namespace LiveHostSweeper
{
    internal static class IPLogic
    {
        public static void PingRange(ILogger logger)
        {
            Utilities.PrintToScreen(ConsoleColor.Cyan, "Please provide a valid target IPv4 or IPv6 address range. (Example: 192.168.0.143, 192.168.168.100/24, 2001:0db8::/64)");
            var targetIpnetwork = Console.ReadLine();

            while (!ValidateIp(targetIpnetwork))
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{targetIpnetwork} is invalid, please provide a valid target IPv4 or IPv6 address range.", PaddingTypes.Top);
                targetIpnetwork = Console.ReadLine();
            }

            var ipDataset = RetrieveIpData(targetIpnetwork, logger);

            Utilities.PrintToScreen(ConsoleColor.Green, "Press [1] to start a ping sweep of this IP range.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Yellow, "Press [2] to restart the application.", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Red, "Press [3] to exit.", PaddingTypes.Bottom);

            int userSelection = Utilities.ValidateUserInputToInt();

            while (userSelection != 1 && userSelection != 2 && userSelection != 3)
            {
                Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-3.", PaddingTypes.Bottom);
                userSelection = Utilities.ValidateUserInputToInt();
            }

            switch (userSelection)
            {
                case 1:

                    Utilities.PrintToScreen(ConsoleColor.Green, "Press [1] to log the entire ping sweep to the log file?", PaddingTypes.Top);
                    Utilities.PrintToScreen(ConsoleColor.DarkGreen, "Press [2] to log only succesfull pings to the log file?", PaddingTypes.None);
                    Utilities.PrintToScreen(ConsoleColor.Yellow, "Press [3] to restart the application.", PaddingTypes.None);
                    Utilities.PrintToScreen(ConsoleColor.Red, "Press [4] to exit.", PaddingTypes.Bottom);

                    userSelection = Utilities.ValidateUserInputToInt();

                    while (userSelection != 1 && userSelection != 2 && userSelection != 3 && userSelection != 4)
                    {
                        Utilities.PrintToScreen(ConsoleColor.Red, $"{userSelection} is invalid, please make a selection between 1-3.", PaddingTypes.Bottom);
                        userSelection = Utilities.ValidateUserInputToInt();
                    }

                    switch (userSelection)
                    {
                        case 1:
                            PingSweep(ipDataset, logger, logOnlySuccess: false);
                            break;

                        case 2:
                            PingSweep(ipDataset, logger, logOnlySuccess: true);
                            break;

                        case 3:
                            Utilities.RestartApplication();
                            break;

                        case 4:
                            Environment.Exit(0);
                            break;
                    }

                    break;

                case 2:
                    Utilities.RestartApplication();
                    break;

                case 3:
                    Environment.Exit(0);
                    break;
            }
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
        /// Grab the data from the given IP, returns IPNetwork
        /// Logs to screen and file.
        /// </summary>
        /// <param name="targetIpnetwork"></param>
        private static IPNetwork RetrieveIpData(string targetIpnetwork, ILogger logger)
        {
            IPNetwork ipnetwork = IPNetwork.Parse(targetIpnetwork);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Network : {ipnetwork.Network}", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Netmask : {ipnetwork.Netmask}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Broadcast : {ipnetwork.Broadcast}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"FirstUsable : {ipnetwork.FirstUsable}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"LastUsable : {ipnetwork.LastUsable}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Usable : {ipnetwork.Usable}", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.Cyan, $"Cidr : {ipnetwork.Cidr}", PaddingTypes.Bottom);

            logger.Information($"Network : {ipnetwork.Network}");
            logger.Information($"Netmask : {ipnetwork.Netmask}");
            logger.Information($"Broadcast : {ipnetwork.Broadcast}");
            logger.Information($"FirstUsable : {ipnetwork.FirstUsable}");
            logger.Information($"LastUsable : {ipnetwork.LastUsable}");
            logger.Information($"Usable : {ipnetwork.Usable}");
            logger.Information($"Cidr : {ipnetwork.Cidr}");

            return ipnetwork;
        }

        private static void PingSweep(IPNetwork iPNetwork, ILogger logger, bool logOnlySuccess)
        {
            Utilities.PrintToScreen(ConsoleColor.White, "--------------------------------------------------------------------------", PaddingTypes.Top);
            Utilities.PrintToScreen(ConsoleColor.Cyan, "Ping Reply Status                    | Target Ip         | Round Trip Time", PaddingTypes.None);
            Utilities.PrintToScreen(ConsoleColor.White, "--------------------------------------------------------------------------", PaddingTypes.None);

            logger.Information("--------------------------------------------------------------------------");
            logger.Information("Ping Reply Status                    | Target Ip         | Round Trip Time");
            logger.Information("--------------------------------------------------------------------------");

            if (iPNetwork.Usable < 255)
            {
                using var ping = new Ping();

                string[] tokens = iPNetwork.FirstUsable.ToString().Split('.');
                int value1 = int.Parse(tokens[0]);
                int value2 = int.Parse(tokens[1]);
                int value3 = int.Parse(tokens[2]);
                int value4 = int.Parse(tokens[3]);

                string ipPart1 = $"{value1}.{value2}.{value3}.";
                string ipPart2 = value4.ToString();
                PingReply reply;

                for (int i = 0; i < iPNetwork.Usable; i++)
                {
                    reply = null;
                    string targetIp = ipPart1 + i;
                    //logger.Information($"Now looking at : {targetIp}");

                    try
                    {
                        reply = ping.Send(targetIp);

                        switch (reply.Status)
                        {
                            case IPStatus.Success:
                                Utilities.PrintPingResultsToScreen(ConsoleColor.Green, pingReply: reply, logger, targetIp, logOnlySuccess);
                                break;

                            case IPStatus.DestinationPortUnreachable:
                            case IPStatus.DestinationNetworkUnreachable:
                            case IPStatus.DestinationHostUnreachable:
                                Utilities.PrintPingResultsToScreen(ConsoleColor.Gray, pingReply: reply, logger, targetIp, logOnlySuccess);
                                break;

                            default:
                                Utilities.PrintPingResultsToScreen(ConsoleColor.Red, pingReply: reply, logger, targetIp, logOnlySuccess);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}