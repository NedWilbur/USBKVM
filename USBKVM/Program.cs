using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.CompilerServices;

namespace USBKVM
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "USBKVM";
            Console.WriteLine(@" __   __  _______  _______  ___   _  __   __  __   __ ");
            Console.WriteLine(@"|  | |  ||       ||  _    ||   | | ||  | |  ||  |_|  |");
            Console.WriteLine(@"|  | |  ||  _____|| |_|   ||   |_| ||  |_|  ||       |");
            Console.WriteLine(@"|  |_|  || |_____ |       ||      _||       ||       |");
            Console.WriteLine(@"|       ||_____  ||  _   | |     |_ |       ||       |");
            Console.WriteLine(@"|       | _____| || |_|   ||    _  | |     | | ||_|| |");
            Console.WriteLine(@"|_______||_______||_______||___| |_|  |___|  |_|   |_|" + Environment.NewLine);
            Console.WriteLine(@"                   https://github.com/NedWilbur/USBKVM" + Environment.NewLine);

            Data.Import();
            Events.Start();
            
            Console.WriteLine("USBKVM Running");
            Console.WriteLine("See docs to run in tray");
            Console.WriteLine("Press any key to close" + Environment.NewLine);

            Exit(); // keeps it open
        }

        internal static void RunControlMyMonitor(string args, bool waitForExit = false)
        {
            Process process = new Process { StartInfo = { FileName = "ControlMyMonitor.exe", Arguments = args,  } };
            process.Start();

            if (waitForExit)
                process.WaitForExit();
        }

        internal static void Exit()
        {
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}