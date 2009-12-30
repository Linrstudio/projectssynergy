using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Framework;

public class plugin
{
    [DllImport("k8055.dll")]
    private static extern int OpenDevice(int CardAddress);

    [PluginEntry]
    public static void Main()
    {
        Log.Write("My Plugin", "K8055 plugin loaded");

        int result = OpenDevice(0);// open channel zero
        switch (result)
        {
            case -1:
                Log.Write("My Plugin", "Could not connect to K8055, no devices will be added");// return;
                break;
        }

#if true
        //add digital outputs
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 1", 1));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 2", 2));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 3", 3));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 4", 4));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 5", 5));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 6", 6));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 7", 7));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalOutput("digital out 8", 8));
#endif
#if true
        //add digital inputs
        NetworkManager.LocalNode.AddNetworkClass(new DigitalInput("digital in 1", 1));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalInput("digital in 2", 2));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalInput("digital in 3", 3));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalInput("digital in 4", 4));
        NetworkManager.LocalNode.AddNetworkClass(new DigitalInput("digital in 5", 5));
#endif
#if true
        //add analog outputs
        NetworkManager.LocalNode.AddNetworkClass(new AnalogOutput("analog out 1", 1));
        NetworkManager.LocalNode.AddNetworkClass(new AnalogOutput("analog out 2", 2));
#endif
#if false
        //add analog inputs
        NetworkManager.LocalNode.AddNetworkClass(new AnalogInput("analog in 1", 1));
        NetworkManager.LocalNode.AddNetworkClass(new AnalogInput("analog in 2", 2));
#endif
    }

    [PluginTick]
    public static void Update()
    {
        //Console.WriteLine("Tick!");
    }
}

public class DigitalOutput : NetworkClassLocal
{
    [DllImport("k8055.dll")]
    private static extern void SetDigitalChannel(int Channel);
    [DllImport("k8055.dll")]
    private static extern void ClearDigitalChannel(int Channel);

    public int Channel;
    public DigitalOutput(string _Name, int _Channel) : base(_Name) { Channel = _Channel; enabled = false; inversed = false; }

    [NetworkField("HUD_ON")]
    public bool enabled;

    [NetworkField("Inversed")]
    public bool inversed;

    [NetworkFieldChanged("On")]
    public void OnOnChanged(bool _Oldval, bool _Newval)
    {
        if (_Oldval != _Newval) updatehardware();
    }

    [NetworkFieldChanged("Inversed")]
    public void OnInversedChanged(bool _Oldval, bool _Newval)
    {
        if (_Oldval != _Newval) updatehardware();
    }

    void updatehardware()
    {
        if (enabled != inversed) SetDigitalChannel(Channel); else ClearDigitalChannel(Channel);
        Log.Write("My Plugin", "{0} is now {1}", Name, enabled != inversed ? "On" : "Off");
    }

}

public class DigitalInput : NetworkClassLocal
{
    [DllImport("k8055.dll")]
    private static extern int ReadDigitalChannel(int Channel);

    public int Channel;
    public DigitalInput(string _Name, int _Channel) : base(_Name) { Channel = _Channel; On = false; inversed = false; ReadDigitalChannel(Channel); }

    [NetworkField("On",true)]
    public bool On;

    [NetworkField("Inversed")]
    public bool inversed;

    public override void Update()
    {
		//Console.WriteLine("Cosmo is vet Stru");
        bool read = (ReadDigitalChannel(Channel) != 0) != inversed;
        //Log.Write("My Plugin", "bla {0}", ReadDigitalChannel(Channel));
        if (read != On)
        {
            On = read;
            Log.Write("My Plugin", "{0} is now {1}", Name, On != inversed ? "On" : "Off");
        }
        base.Update();
    }
}

public class AnalogOutput : NetworkClassLocal
{
    [DllImport("k8055.dll")]
    private static extern void OutputAnalogChannel(int Channel, int Data);

    public int Channel;
    public AnalogOutput(string _Name, int _Channel) : base(_Name) { Channel = _Channel;}

    [NetworkField("Value")]
    public byte Value;

    [NetworkFieldChanged("Value")]
    public void OnInversedChanged(byte _Oldval, byte _Newval)
    {
        if (_Oldval != _Newval) updatehardware();
    }

    [NetworkMethod("UpdateHardware")]
    void updatehardware()
    {
        OutputAnalogChannel(Channel, Value);
        Log.Write("My Plugin", "{0} is now {1}", Name, Value);
    }
}

public class AnalogInput : NetworkClassLocal
{
    [DllImport("k8055.dll")]
    private static extern int ReadAnalogChannel(int Channel);

    public int Channel;
    public AnalogInput(string _Name, int _Channel) : base(_Name) { Channel = _Channel; }

    [NetworkField("Value",true)]
    public byte Value;

    public override void Update()
    {
        //Console.WriteLine("Cosmo is vet Stru");
        byte read = (byte)ReadAnalogChannel(Channel);
        if (Math.Abs(read - Value) > 2)
        {
            Value = read;
            Log.Write("My Plugin", "{0} is now {1}", Name, Value);
        }
        base.Update();
    }
}
