using ConsoleTables;
using LiveHostSweeper.Enums;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace LiveHostSweeper
{
    internal static class Logic
    {
        public static void PingRange(ILogger logger)
        {
            const int timeout = 150;

            var ipDataset = Navigation.IpDataSetOptions(logger);

            switch (Navigation.IpOptions())
            {
                case 1:

                    switch (Navigation.IpSearchOptions())
                    {
                        case 1:
                            PingSweep(ipDataset, logger, logOnlySuccess: false, responseTimout: timeout);
                            break;

                        case 2:
                            PingSweep(ipDataset, logger, logOnlySuccess: true, responseTimout: timeout);
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

        private static void PingSweep(IPNetwork iPNetwork, ILogger logger, bool logOnlySuccess, int responseTimout)
        {
            ConsoleTable table = new ConsoleTable("Status", "Target", "Round Trip Time");

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
                int total = (int)iPNetwork.Usable;

                for (int i = value4; i < total + 1; i++)
                {
                    reply = null;
                    string targetIp = ipPart1 + i;

                    try
                    {
                        reply = ping.Send(targetIp, timeout: responseTimout);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    Utilities.PrintPingResultsToScreen(pingReply: reply, targetIp, logOnlySuccess, table);

                    Utilities.PrintToScreen(ConsoleColor.Cyan, $"{Utilities.CalculatePercentage(currentValue: i, maxValue: total)} ({i} of {total} IP's pinged.)", PaddingTypes.None, overwritePreviousLine: true);
                }

                Utilities.PrintToScreen(ConsoleColor.White, "\n\n", PaddingTypes.None);
                logger.Information($"{table}");
                table.Write();

                Utilities.PrintToScreen(ConsoleColor.Yellow, $"Your log file should be saved here: {new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName}", PaddingTypes.Full);

                Navigation.PresentAndHandleExitOptions();
            }
        }

        /// <summary>
        /// https://github.com/lduchosal/ipnetwork
        /// Grab the data from the given IP, returns IPNetwork
        /// Logs to screen and file.
        /// </summary>
        /// <param name="targetIpnetwork"></param>
        public static IPNetwork RetrieveIpData(string targetIpnetwork, ILogger logger)
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

        /// <summary>
        /// https://github.com/lduchosal/ipnetwork
        /// Tests whether given IP can be parsed.
        /// </summary>
        /// <param name="targetIpnetwork"></param>
        public static bool ValidateIp(string targetIpnetwork)
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
    }
}