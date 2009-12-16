using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Framework;

public class plugin
{
    [PluginEntry]
    public static void Main()
    {
        Log.Write("Wiimote Plugin", "Bas meuk UI loaded");

    }

    [PluginTick]
    public static void Update()
    {
        
    }
}
