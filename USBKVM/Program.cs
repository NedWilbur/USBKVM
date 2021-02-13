using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

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
            Console.WriteLine("Press any key to close" + Environment.NewLine);
            Console.ReadKey(); // keep open
        }
    }
}