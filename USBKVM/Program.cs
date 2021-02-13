﻿using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Xml.Linq;

namespace USBKVM
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello.");

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
            Console.WriteLine("Press any key to close");
            Console.ReadLine();
        }

        // see https://stackoverflow.com/questions/620144/detecting-usb-drive-insertion-and-removal-using-windows-service-and-c-sharp
        private static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("USBHub Connected");
        }

        private static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("USBHub Disconnected");
        }
    }
}
