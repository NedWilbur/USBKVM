using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;

namespace USBKVM
{
    internal class Program
    {
        private static string ThisPC { get; } = Environment.MachineName;
        public static string PC1_Name { get; set; }
        public static string PC2_Name { get; set; }
        private static List<string> Monitors { get; set; }
        private static List<string> PC1_Inputs { get; set; }
        private static List<string> PC2_Inputs { get; set; }

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello.");

            ImportSettings();

            // start USBHub watchers (insert & remove)
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += DeviceInsertedEvent;
            insertWatcher.Start();
            Console.WriteLine("Connect listener started");

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += DeviceRemovedEvent;
            removeWatcher.Start();
            Console.WriteLine("Disconnect listener started");

            // keep open
            Console.WriteLine("Running. Press any key to close.");
            Console.ReadLine();
        }

        private static void ImportSettings()
        {
            Console.WriteLine("Importing settings");
            try
            {
                XDocument settingsXml = XDocument.Load("Settings.xml");

                PC1_Name = settingsXml.Root.Element("PC1").Attribute("Name").Value;
                PC2_Name = settingsXml.Root.Element("PC2").Attribute("Name").Value;

                Monitors = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("Name").Value).ToList();
                PC1_Inputs = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("PC1_Input").Value).ToList();
                PC2_Inputs = settingsXml.Root.Elements("Monitor").Select(monitor => monitor.Attribute("PC2_Input").Value).ToList();

                Console.WriteLine("Settings imported");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // see https://stackoverflow.com/questions/620144/detecting-usb-drive-insertion-and-removal-using-windows-service-and-c-sharp
        private static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("USBHub Connected");
            SwitchToPC(true);
        }

        private static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("USBHub Disconnected");
            SwitchToPC(false);
        }

        private static void RunControlMyMonitor(string monitor, string input)
        {
            Console.WriteLine($"Switching '{monitor}' to '{input}'");
            Process process = new Process {StartInfo = {FileName = "ControlMyMonitor.exe", Arguments = $"/SetValueIfNeeded {monitor} 60 {input}" } };
            process.Start();
        }

        private static void SwitchToPC(bool USBConnected)
        {
            List<string> targetPcInputs = null;

            if (ThisPC == PC1_Name && USBConnected)
                targetPcInputs = PC1_Inputs;
            else if (ThisPC == PC1_Name && !USBConnected)
                targetPcInputs = PC2_Inputs;
            else if (ThisPC == PC2_Name && USBConnected)
                targetPcInputs = PC2_Inputs;
            else if (ThisPC == PC2_Name && !USBConnected)
                targetPcInputs = PC1_Inputs;
            else
                throw new Exception("Unable to find matching PC name. Please check the Settings.xml");

            for (int i = 0; i < Monitors.Count; i++)
                RunControlMyMonitor(Monitors[i], targetPcInputs[i]);
        }
    }
}
