using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Framework;

public class plugin
{
    [PluginEntry]
    public static void Main()
    {
        Log.Write("Wiimote Plugin", "Wiimote plugin loaded");

    }

    [PluginTick]
    public static void Update()
    {
        
    }
}

public class hans : NetworkClassLocal
{
    [NetworkField("janje")]
    public bool jantje;

    [NetworkField("HUD_GROUP")]
    public string group = "Roeny.Kitchen";

    [NetworkField("HUD_ARCHETYPE")]
    public string type = "DIGITAL OUT";

    //archetype specific
    [NetworkField("HUD_ON")]
    public bool ihateyou;
}
