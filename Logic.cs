using ConsoleTables;
using LiveHostSweeper.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

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
                            PingSweepAsync(ipDataset, logger, logOnlySuccess: true, responseTimout: timeout);
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

                foreach (PingReply pingReply in ipList.AsParallel().WithDegreeOfParallelism(64).Select(ip => new Ping().Send(ip)))
                {
                    Console.WriteLine($"Ping status: {pingReply.Status} for IP address: {ip}");
                }

                //foreach (PingReply pingReply in ipList.AsParallel().WithDegreeOfParallelism(256).Select(ip => new Ping().Send(ip)))
                //{
                //    Utilities.PrintPingResultsToScreen(pingReply: pingReply, h, logOnlySuccess, table);
                //    Utilities.PrintToScreen(ConsoleColor.Cyan, $"{Utilities.CalculatePercentage(currentValue: i, maxValue: total)} ({i} of {total} IP's pinged.)", PaddingTypes.None, overwritePreviousLine: true);
                //}

                //for (int i = value4; i < total + 1; i++)
                //{
                //    reply = null;
                //    string targetIp = ipPart1 + i;

                //    try
                //    {
                //        reply = ping.Send(targetIp, timeout: responseTimout);
                //    }
                //    catch (Exception)
                //    {
                //        throw;
                //    }

                //    Utilities.PrintPingResultsToScreen(pingReply: reply, targetIp, logOnlySuccess, table);

                //    Utilities.PrintToScreen(ConsoleColor.Cyan, $"{Utilities.CalculatePercentage(currentValue: i, maxValue: total)} ({i} of {total} IP's pinged.)", PaddingTypes.None, overwritePreviousLine: true);
                //}

                logger.Information($"\n\n{table}");
                table.Write();

                Utilities.PrintToScreen(ConsoleColor.Yellow, $"Your log file should be saved here: {new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName}", PaddingTypes.Full);

                Navigation.PresentAndHandleExitOptions();
            }
        }

        private static async Task PingSweepAsync(IPNetwork iPNetwork, ILogger logger, bool logOnlySuccess, int responseTimout)
        {
            ConsoleTable table = new ConsoleTable("Status", "Target", "Round Trip Time");

            if (iPNetwork.Usable < 255)
            {
                //using var ping = new Ping();

                string[] tokens = iPNetwork.FirstUsable.ToString().Split('.');
                int value1 = int.Parse(tokens[0]);
                int value2 = int.Parse(tokens[1]);
                int value3 = int.Parse(tokens[2]);
                int value4 = int.Parse(tokens[3]);

                string ipBlockPart1 = $"{value1}.{value2}.{value3}.";
                string ipBlockPart2 = value4.ToString();

                int totalUsableIps = (int)iPNetwork.Usable;

                List<string> ipList = new List<string>();
                for (int i = value4; i < totalUsableIps + 1; i++)
                {
                    ipList.Add(ipBlockPart1 + i);
                }

                foreach (PingReply pingReply in ipList.AsParallel().WithDegreeOfParallelism(256).Select(h => new Ping().Send(h)))
                {
                }

                //foreach (var targetIp in pingResponseDetails.IpList and var pingReplyTask in pingResponseDetails.PingReplyTasks)
                //{
                //    //pingTask.Result is whatever type T was declared in PingAsync
                //    Console.WriteLine(pingTask.Result.Status);
                //    Console.WriteLine(pingTask.Result.Address);
                //    //Utilities.PrintPingResultsToScreen(pingReply: pingTask.Result, pingTask, logOnlySuccess, table);
                //    //Utilities.PrintToScreen(ConsoleColor.Cyan, $"{Utilities.CalculatePercentage(currentValue: i, maxValue: total)} ({i} of {total} IP's pinged.)", PaddingTypes.None, overwritePreviousLine: true);
                //}

                ///List<Task<PingResponse>> pingTasks = new List<Task<PingResponse>>();

                //List<Task<PingReply>> pingTasks = new List<Task<PingReply>>();
                //foreach (var address in addresses)
                //{
                //    pingTasks.Add(PingAsync(address));
                //}

                ////Wait for all the tasks to complete
                //Task.WaitAll(pingTasks.ToArray());

                //foreach (var ip in ipList)
                //{
                //    pingTasks.
                //    pingResponseDetails.IpList.Add(ip);
                //    pingTasks.Tasks.Add(PingAsync(ip));
                //}

                ////Wait for all the tasks to complete
                //Task.WaitAll(pingResponseDetails.PingReplyTasks.ToArray());

                ////Now you can iterate over your list of pingTasks
                //foreach (var targetIp in pingResponseDetails.IpList and var pingReplyTask in pingResponseDetails.PingReplyTasks)
                //{
                //    //pingTask.Result is whatever type T was declared in PingAsync
                //    Console.WriteLine(pingTask.Result.Status);
                //    Console.WriteLine(pingTask.Result.Address);
                //    //Utilities.PrintPingResultsToScreen(pingReply: pingTask.Result, pingTask, logOnlySuccess, table);
                //    //Utilities.PrintToScreen(ConsoleColor.Cyan, $"{Utilities.CalculatePercentage(currentValue: i, maxValue: total)} ({i} of {total} IP's pinged.)", PaddingTypes.None, overwritePreviousLine: true);
                //}

                logger.Information($"\n\n{table}");
                table.Write();

                Utilities.PrintToScreen(ConsoleColor.Yellow, $"Your log file should be saved here: {new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName}", PaddingTypes.Full);

                Navigation.PresentAndHandleExitOptions();
            }
        }

        /// <summary>
        /// Returns a completed task.
        /// https://stackoverflow.com/a/13405948/3324415
        /// </summary>
        /// <param name="address"></param>
        //private static Task<PingResponse> PingAsync(string address)
        //{
        //    var taskCompletionSource = new TaskCompletionSource<PingResponse>();
        //    Ping ping = new Ping();
        //    ping.PingCompleted += (obj, sender) => taskCompletionSource.SetResult(sender.Reply);
        //    ping.SendAsync(address, new object());
        //    return taskCompletionSource.Task;
        //}

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