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
            Console.WriteLine(@" __   __  _______  _______  ___   _  __   __  __   __ ");
            Console.WriteLine(@"|  | |  ||       ||  _    ||   | | ||  | |  ||  |_|  |");
            Console.WriteLine(@"|  | |  ||  _____|| |_|   ||   |_| ||  |_|  ||       |");
            Console.WriteLine(@"|  |_|  || |_____ |       ||      _||       ||       |");
            Console.WriteLine(@"|       ||_____  ||  _   | |     |_ |       ||       |");
            Console.WriteLine(@"|       | _____| || |_|   ||    _  | |     | | ||_|| |");
            Console.WriteLine(@"|_______||_______||_______||___| |_|  |___|  |_|   |_|");
            Console.WriteLine(Environment.NewLine + "Hello");

            Data.Import();
            Events.Start();
            
            Console.WriteLine("Running! (Press any key to close)" + Environment.NewLine);
            Console.ReadLine(); // keep open
        }
    }
}
