using System;
using System.Collections.Generic;
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

        internal static void Import()
        {
            Console.Write("Importing settings... ");
            try
            {
                XDocument settingsXml = XDocument.Load("Settings.xml");

                PC1_Name = settingsXml.Root.Element("PC1").Attribute("Name").Value;
                PC2_Name = settingsXml.Root.Element("PC2").Attribute("Name").Value;

                Monitors = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("Name").Value).ToList();
                PC1_Inputs = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("PC1_Input").Value).ToList();
                PC2_Inputs = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("PC2_Input").Value).ToList();

                Console.Write("Done!" + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.Write("Error!" + Environment.NewLine + e);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}
