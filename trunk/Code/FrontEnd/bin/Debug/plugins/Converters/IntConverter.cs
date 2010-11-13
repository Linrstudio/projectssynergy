using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "IntConverter plugin loaded");
    }
}

public class IntConverter : Converter
{
	public override Type Converts { get { return typeof(int); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		_TargetStream.Write(BitConverter.GetBytes((int)_Object));
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		return BitConverter.ToInt32(_TargetStream.Read(4), 0);
	}
}
