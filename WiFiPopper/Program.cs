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
            status();
            Console.ReadKey();
            return;
            while (true)
            {
                string arg1;
                if (args == null || args.Length == 0)
                    arg1 = Console.ReadLine();
                else
                    arg1 = args[0];
                if (arg1 == "-wifidown")
                {
                    if (status() == false)
                    {
                        Console.WriteLine("already off");
                        continue;
                    }
                    plink("root@192.168.1.2 -pw admin \". /etc/profile; wifi down\"", true, true);
                    if (status())
                        Console.WriteLine("failure");
                    else Console.WriteLine("success");
                    //Console.WriteLine("status: " + status().ToString());
                }
                if (arg1 == "-wifiup")
                {
                    if (status() == true)
                    {
                        Console.WriteLine("already on");
                        continue;
                    }
                    plink("root@192.168.1.2 -pw admin \". /etc/profile; wifi up\"", true, true);
                    if (!status())
                        Console.WriteLine("failure");
                    else Console.WriteLine("success");
                }
                if (arg1 == "-status")
                    Console.WriteLine($"status: {status()}");
            }
        }
        static string wifiName = "MyRV_";
        static bool status()
        {
            List<string> output = plink("root@192.168.1.2 -pw admin iwconfig", true, true);
            foreach (string s in output)
                if (s.Contains(wifiName))
                    return true;
            return false;

        }
        static List<string> errLines;
        static List<string> plink(string args, bool skipStdErr = true, bool printOutput = false, string plinkLoc = "")
        {
            if (string.IsNullOrEmpty(plinkLoc))
                plinkLoc = @"C:\Program Files\PuTTY\plink";
            List<string> output = new List<string>();
            errLines = new List<string>();
            ProcessStartInfo startInfo = new ProcessStartInfo(plinkLoc);
            //argument location on different computers
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.Arguments = "root@192.168.1.2 -pw admin iwconfig";
            Process cmd = Process.Start(startInfo);
            Process process = Process.Start(startInfo);
            string line;
            //if (!skipStdErr)
                while ((line = cmd.StandardError.ReadLine()) != null)
                {
                    if (printOutput)
                        Console.WriteLine(line);
                    errLines.Add(line);
                }

            line = "";
            while ((line = cmd.StandardOutput.ReadLine()) != null)
            {
                if (printOutput)
                    Console.WriteLine(line);
                output.Add(line);
            }
            return output;
        }
    }
}
