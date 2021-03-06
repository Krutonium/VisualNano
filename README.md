# VisualNano
## A nano-like Editor written in C# using ETO.

I was constantly hitting the wrong keys in GUI text editors, after using nano a LOT. Ctrl+O to save, Ctrl+X to close, etc.

So I did the only logical thing and wrote my own text editor with the default key binding of Nano, at least in the parts implemented so far.

So far I've tested it on Windows and Linux, but it should work fine on MacOS as well.

Linux (GTK)

![Image](https://i.imgur.com/UKEKpqU.png)

Windows (WPF or WinForms)

![Image](https://i.imgur.com/GD0CMiJ.png)

## Building

To build this project, you will need any compatible C# runtime - Mono or DotNet 4.7.1 (Possibly older, untested.)

The project can be opened and edited in Rider as well as Visual Studio, and in theory could also be edited in Visual Studio Code.

Simply open the project, restore the nuget projects, and then build as normal.