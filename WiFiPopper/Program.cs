using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        return "WIFI_UP";
                foreach (string s in errLines)
                    if (s.Contains("FATAL ERROR") || s.Contains("Access denied"))
                        return "WIFI_STATE_UNKNOWN";
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
            foreach (string s in errLines)
                if (s.Contains("FATAL ERROR") || s.Contains("Access denied"))
                {
                    Console.WriteLine("WIFI_STATE_UNKNOWN");
                    //Console.Read();
                    Process.GetCurrentProcess().Kill();
                }
            line = "";
            while ((line = cmd.StandardOutput.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                output.Add(line);
            }
            return output;
        }
    }
}
