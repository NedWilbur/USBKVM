using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace USBKVM
{
    internal static class Events
    {
        // More Info: https://stackoverflow.com/questions/620144/detecting-usb-drive-insertion-and-removal-using-windows-service-and-c-sharp
        internal static void Start()
        {
            Console.Write("Starting USBHub connect/disconnect event listeners... ");

            // start USBHub watchers (insert & remove)
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += DeviceInsertedEvent;
            insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += DeviceRemovedEvent;
            removeWatcher.Start();

            Console.Write("Done!" + Environment.NewLine);
        }

        private static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("===> USBHub Connected");
            SwitchToPC(true);
        }

        private static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("=/=> USBHub Disconnected");
            SwitchToPC(false);
        }

        private static void RunControlMyMonitor(string monitor, string input)
        {
            Console.Write($"\tSwitching '{monitor}' to '{input}'... ");
            Process process = new Process { StartInfo = { FileName = "ControlMyMonitor.exe", Arguments = $"/SetValueIfNeeded {monitor} 60 {input}" } };
            process.Start();
            Console.Write("Done!" + Environment.NewLine);

        }

        private static void SwitchToPC(bool USBConnected)
        {
            List<string> targetPcInputs = null;

            if (Data.ThisPC == Data.PC1_Name && USBConnected)
                targetPcInputs = Data.PC1_Inputs;
            else if (Data.ThisPC == Data.PC1_Name && !USBConnected)
                targetPcInputs = Data.PC2_Inputs;
            else if (Data.ThisPC == Data.PC2_Name && USBConnected)
                targetPcInputs = Data.PC2_Inputs;
            else if (Data.ThisPC == Data.PC2_Name && !USBConnected)
                targetPcInputs = Data.PC1_Inputs;
            else
                throw new Exception("Unable to find matching PC name. Please check the Settings.xml");

            for (int i = 0; i < Data.Monitors.Count; i++)
                RunControlMyMonitor(Data.Monitors[i], targetPcInputs[i]);
        }
    }
}
