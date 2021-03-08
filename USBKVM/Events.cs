using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Reflection.Metadata.Ecma335;

namespace USBKVM
{
    internal static class Events
    {
        /// <summary>
        /// A timer that starts when a event is triggered. Prevents event from firing monitor switch multiple times rapidly.
        /// </summary>
        private static Stopwatch LastEventStopwatch { get; } = new Stopwatch();
        private const int MinimumTimeBetweenEvents = 3000;

        // More Info: https://stackoverflow.com/questions/620144/detecting-usb-drive-insertion-and-removal-using-windows-service-and-c-sharp
        internal static void Start()
        {
            Console.Write("Starting USB listeners... ");

            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += DeviceInsertedEvent;
            insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += DeviceRemovedEvent;
            removeWatcher.Start();

            LastEventStopwatch.Start();
            Console.Write("Done!" + Environment.NewLine);
        }

        private static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            if (LastEventStopwatch.ElapsedMilliseconds <= MinimumTimeBetweenEvents) return;
            LastEventStopwatch.Restart();

            Console.WriteLine("USBHub Connected");
            SwitchToPC(true);
        }

        private static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            if (LastEventStopwatch.ElapsedMilliseconds <= MinimumTimeBetweenEvents) return;
            LastEventStopwatch.Restart();

            Console.WriteLine("USBHub Disconnected");
            SwitchToPC(false);
        }

        internal static void SwitchToPC(bool USBConnected)
        {
            List<string> targetPcInputs;

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
            {
                Console.Write($"\tSwitching '{Data.Monitors[i]}' to '{targetPcInputs[i]}'... ");
                Program.RunControlMyMonitor($"/SetValueIfNeeded {Data.Monitors[i]} 60 {targetPcInputs[i]}");
                Console.Write("Done!" + Environment.NewLine);
            }
        }
    }
}
