using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Framework;

public class plugin
{
    public static List<DigitalOutput> outputs = new List<DigitalOutput>();
	public static List<DigitalInput> inputs = new List<DigitalInput>();
	
    [PluginEntry]
    public static void Main()
    {
        Console.WriteLine("w00tie iam in yer assembly");
        outputs.Add(new DigitalOutput("digital out 1", 1));
        outputs.Add(new DigitalOutput("digital out 2", 2));
        outputs.Add(new DigitalOutput("digital out 3", 3));
        outputs.Add(new DigitalOutput("digital out 4", 4));
        outputs.Add(new DigitalOutput("digital out 5", 5));
        outputs.Add(new DigitalOutput("digital out 6", 6));
        outputs.Add(new DigitalOutput("digital out 7", 7));
        outputs.Add(new DigitalOutput("digital out 8", 8));

		inputs.Add(new DigitalInput("digital in 1", 1));
		inputs.Add(new DigitalInput("digital in 2", 2));
		inputs.Add(new DigitalInput("digital in 3", 3));
		inputs.Add(new DigitalInput("digital in 4", 4));
		inputs.Add(new DigitalInput("digital in 5", 5));
		
        foreach (DigitalOutput o in outputs) NetworkManager.LocalNode.AddNetworkClass(o);
		foreach (DigitalInput o in inputs) NetworkManager.LocalNode.AddNetworkClass(o);
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
    public DigitalOutput(string _Name, int _Channel) : base(_Name) { Channel = _Channel; enabled = true; }

    [NetworkField("On")]
    public bool enabled;

    [NetworkField("Inversed")]
    public bool inversed;

    [NetworkFieldChanged("On")]
    public void OnOnChanged(bool _Oldval, bool _Newval)
    {
        Console.WriteLine("uuh hallo ?");
        if (_Oldval != _Newval) updatehardware();
    }

    void updatehardware()
    {
        if (enabled != inversed) SetDigitalChannel(Channel); else ClearDigitalChannel(Channel);
    }

}

public class DigitalInput : NetworkClassLocal
{
    [DllImport("k8055.dll")]
    private static extern int ReadDigitalChannel(int Channel);

    public int Channel;
    public DigitalInput(string _Name, int _Channel) : base(_Name) { Channel = _Channel; }

    [NetworkField("On")]
    public bool On;

    public override void Update()
    {
		//Console.WriteLine("Cosmo is vet Stru");
        On = ReadDigitalChannel(Channel) != 0;
    }
}
