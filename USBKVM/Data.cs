using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace USBKVM
{
    internal static class Data
    {
        internal static string ThisPC { get; } = Environment.MachineName;
        internal static string PC1_Name { get; private set; }
        internal static string PC2_Name { get; private set; }
        internal static List<string> Monitors { get; private set; }
        internal static List<string> PC1_Inputs { get; private set; }
        internal static List<string> PC2_Inputs { get; private set; }
        internal static bool SwitchOnStart { get; private set; }

        internal static void Import()
        {
            Console.Write("Importing Settings.xml... ");
            try
            {
                XDocument settingsXml = XDocument.Load("Settings.xml");

                PC1_Name = settingsXml.Root.Element("PC1").Attribute("Name").Value;
                PC2_Name = settingsXml.Root.Element("PC2").Attribute("Name").Value;

                Monitors = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("Name").Value).ToList();
                PC1_Inputs = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("PC1_Input").Value).ToList();
                PC2_Inputs = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("PC2_Input").Value).ToList();

                SwitchOnStart = bool.Parse(settingsXml.Root.Element("SwitchOnStart").Attribute("value").Value);

                Console.Write("Done!" + Environment.NewLine);

                ValidateMonitorNames();
            }
            catch (Exception e)
            {
                Console.Write("Error!" + Environment.NewLine + e);
                Program.Exit();
            }
        }

        private static void ValidateMonitorNames()
        {
            bool error = false;

            // Compare monitors in Settings.xml versus detected monitors
            Program.RunControlMyMonitor("/smonitors DetectedMonitors.txt", true);
            string detectedMonitors = File.ReadAllText("DetectedMonitors.txt");
            foreach (string monitor in Monitors)
            {
                if (detectedMonitors.Contains(monitor)) 
                    continue;

                error = true;
                Console.WriteLine($"ERROR: Monitor '{monitor}' not detected!");
            }

            // Monitor name error
            if (error)
            {
                Console.WriteLine("Ensure all monitors in Settings.xml are using unique names from the monitor's detected below:" + Environment.NewLine);
                Console.WriteLine("--------------- DETECTED MONITORS ---------------");
                Console.WriteLine(detectedMonitors);
            }

            // Validate all input values are numbers
            int i;
            foreach (string input in PC1_Inputs)
            {
                if (int.TryParse(input, out i))
                    continue;

                error = true;
                Console.WriteLine($"ERROR: '{input}' in 'PC1_Inputs' is not a number.");
            }

            foreach (string input in PC2_Inputs)
            {
                if (int.TryParse(input, out i))
                    continue;

                error = true;
                Console.WriteLine($"ERROR: '{input}' in 'PC2_Inputs' is not a number.");
            }

            // If Error
            if (error)
            {
                Console.WriteLine("ERROR: Settings invalid. Fix Settings.xml and run again");
                Console.WriteLine("For help, visit: https://github.com/NedWilbur/USBKVM");
                Program.Exit();
            }
        }
    }
}
