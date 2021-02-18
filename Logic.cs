using ConsoleTables;
using LiveHostSweeper.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace LiveHostSweeper
{
    public static class Logic
    {
        public static void PingRange(ILogger logger)
        {
            var ipDataset = Navigation.IpDataSetOptions(logger);

            switch (Navigation.IpOptions())
            {
                case 1:

                    switch (Navigation.IpSearchOptions())
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

        private static void PingSweep(IPNetwork iPNetwork, ILogger logger, bool logOnlySuccess)
        {
            ConsoleTable table = new ConsoleTable("Status", "Target", "Round Trip Time");

            if (iPNetwork.Usable < 255)
            {
                string[] tokens = iPNetwork.FirstUsable.ToString().Split('.');
                int value1 = int.Parse(tokens[0]);
                int value2 = int.Parse(tokens[1]);
                int value3 = int.Parse(tokens[2]);
                int value4 = int.Parse(tokens[3]);

                string ipBlockPart1 = $"{value1}.{value2}.{value3}.";
                string ipBlockPart2 = value4.ToString();

                int totalUsableIps = (int)iPNetwork.Usable;

                //example: 192.168.0.1, 192.168.0.2, 192.168.0.3 etc.
                List<string> ipList = new List<string>();
                for (int i = value4; i < totalUsableIps + 1; i++)
                {
                    ipList.Add(ipBlockPart1 + i);
                }

                int completed = 1;

                foreach (var (ip, reply) in ipList.AsParallel().WithDegreeOfParallelism(ipList.Count).Select(ip => (ip, new Ping().Send(ip, 150))))
                {
                    Utilities.PrintPingResultsToScreen(pingReply: reply, ip, logOnlySuccess, table);
                    Utilities.PrintToScreen(ConsoleColor.Cyan, $"{Utilities.CalculatePercentage(currentValue: completed, maxValue: ipList.Count)} ({completed} of {ipList.Count} IP's pinged.)", PaddingTypes.None, overwritePreviousLine: true);
                    completed++;
                }

                logger.Information($"\n\n{table}");

                Utilities.PrintToScreen(ConsoleColor.Yellow, "", PaddingTypes.None);

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