using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace wifiPopper
{
    class Program
    {
        static string date;
        static string root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static void Main(string[] args)
        {
            date = DateTime.Now.ToShortDateString().Replace('/', '-');
            try
            {
                File.AppendAllText($"{root}/wifipopper_{date}.txt", "-- new entry --\n");
            }
            catch { }
            string arg1;
            if (args == null || args.Length == 0)
            {
                Console.Write("enter command; ");
                arg1 = Console.ReadLine();
            }
            else
                arg1 = args[0];
            if (arg1 == "-wifidown")
            {
                plink("root@192.168.1.2 -pw admin \". /etc/profile; wifi down\"");
                Console.WriteLine(status);
            }
            if (arg1 == "-wifiup")
            {
                plink("root@192.168.1.2 -pw admin \". /etc/profile; wifi up\"");
                Console.WriteLine(status);
            }
            if (arg1 == "-status")
                Console.WriteLine(status);
            //Console.ReadKey();
            //}
        }
        static string status
        {
            get
            {
                List<string> output = plink("root@192.168.1.2 -pw admin iwconfig");
                foreach (string s in output)
                    if (s.Contains("MyRV_"))
                    {
                        appendlog("WIFI_UP",false);
                        return "WIFI_UP";
                    }
                foreach (string s in errLines)
                    if (s.Contains("FATAL ERROR") || s.Contains("Access denied"))
                    {
                        appendlog("WIFI_STATE_UNKNOWN",true);
                        return "WIFI_STATE_UNKNOWN";
                    }
               
                    appendlog("WIFI_DOWN",false);
                    return "WIFI_DOWN";
                
            }

        }
        static List<string> errLines;
        static List<string> plink(string args, bool skipStdErr = true, string plinkLoc = "")
        {
            if (string.IsNullOrEmpty(plinkLoc))
                plinkLoc = @"C:\Program Files\PuTTY\plink";
            List<string> output = new List<string>();
            errLines = new List<string>();
            ProcessStartInfo startInfo = new ProcessStartInfo(plinkLoc);
            //argument location on different computers
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = args;
            Process cmd = Process.Start(startInfo);
            string line;
            //if (!skipStdErr)
            while ((line = cmd.StandardError.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                errLines.Add(line);
            }
            appendlog(errLines, true);

            foreach (string s in errLines)
                if (s.Contains("FATAL ERROR") || s.Contains("Access denied"))
                {
                    Console.WriteLine("WIFI_STATE_UNKNOWN");
                    appendlog("WIFI_STATE_UNKNOWN", true);
                    //Console.Read();
                    Process.GetCurrentProcess().Kill();
                }
            line = "";
            while ((line = cmd.StandardOutput.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                output.Add(line);
            }
            appendlog(output, false);
            return output;
        }
        static void appendlog(string s, bool tabbed)
        {
            while (true)
            {
                try
                {
                    if (tabbed)
                        File.AppendAllText($"{root}/wifipopper_{date}.txt", $"\t{s}\n");
                    else
                        File.AppendAllText($"{root}/wifipopper_{date}.txt", $"{s}\n");
                    break;
                }
                catch { Thread.Sleep(120); }
            }

        }
        static void appendlog(List<string> lines, bool tabbed)
        {
            while (true)
            {
                try
                {
                    foreach (string s in lines)
                        if (tabbed)
                            File.AppendAllText($"{root}/wifipopper_{date}.txt", $"\t{s}\n");
                        else
                            File.AppendAllText($"{root}/wifipopper_{date}.txt", $"{s}\n");
                    break;
                }
                catch { Thread.Sleep(120); }
            }
        }
    }
}

