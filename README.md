# USBKVM
This is an app that turns a basic USB switch into a full automatic monitor switch (minus 'hotkey' switching).

USBKVM heavily utilizes [ControlMyMonitor](https://www.nirsoft.net/utils/control_my_monitor.html) to switch the inputs programmatically. USBKVM simply adds logic around it.

## How does it work
USBKVM watches for USBHub connect/disconnects from the machine. When detected, it will switch all configured monitors to the desired inputs. 

For example, if the USB switch is toggled from PC1 to PC2, USBKVM will switch all configured monitors to the set inputs for PC2. Toggle the USB switch back to PC1 and all monitors will be set back to PC1's configured inputs.

## Why
I have two computers connected to the same set of monitors. A traditional KVM switch that supports USB and 3+ video outputs are stupid expensive. With USBKVM, we can make a [Cheap USB switch](https://www.amazon.com/gp/product/B07XDT6K82) do the same thing (minus hot-key switching). 

## Requirements
- USB Hub Switch ([Example](https://www.amazon.com/gp/product/B07XDT6K82))
- Windows (while it is written in .NET Core, it uses `WqlEventQuery`)
- Two machines, connected to the *same* set of monitors but in *different* inputs
   - Example: PC1 is plugged into the `HDMI1` ports on all three monitors, while PC2 is plugged into the `DVI2` ports on all three monitors
   - The number of monitors does not matter. This will work with 1 or 100+ monitors.
   
## Download
See [Releases](https://github.com/NedWilbur/USBKVM/releases)

## Settings
The settings can be adjusted in `Settings.xml`. Open this in your favorite file editor.
| Setting Name | Description |
| - | - |
| `PC*.Name` | Name of PC1 and PC2. This can be retrieved by running `hostname` in command prompt. |
| `Monitor.Name` | Name of the monitor. This can be retrieved from running `ControlMyMonitor.exe` (included). See [ControlMyMonitor](https://www.nirsoft.net/utils/control_my_monitor.html) docs for more information on getting the monitor name (`Command-Line Options` section). |
| `Monitor.PC*_Input` | The input number that is used for the monitor on PC1. This value can be retrieved from running `ControlMyMonitor.exe` (included) and looking at the current value of `60` (Input select)  |

Add/Remove `<Monitor Name="MONITOR-NAME" PC1_Input="00" PC2_Input="00"></Monitor>` for each monitor you are using.

### Triple Monitor Example
```xml
<Settings>
  <PC1 Name="DESKTOP"></PC1>
  <PC2 Name="NEDLAPTOP"></PC2>
  <Monitor Name="PVJVW4CI15LL" PC1_Input="17" PC2_Input="16"></Monitor>
  <Monitor Name="VVF203600230" PC1_Input="15" PC2_Input="17"></Monitor>
  <Monitor Name="PVJVW4CI1E7L" PC1_Input="17" PC2_Input="16"></Monitor>
</Settings>
```

## Using
Once configured, run `USBKVM.exe`. If configured properly, pressing the switch button on your USB switch will trigger USBKVM to automatically switch all monitor inputs to the machine the USB device is currently connected to. It can be run on only one of the two machines but works better when run on both simultaneously (incase one machine goes to sleep/turns off). It is also recommended to have this run on startup (create a shortcut to the app and place it in the `shell:startup` folder.

## Run In Tray
Since this is a console application, I cannot make it minimize to tray. However, you can use other third-party tools to achieve this. I use [RBTray](https://github.com/benbuck/rbtray).
