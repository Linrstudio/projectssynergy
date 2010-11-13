using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "BoolConverter plugin loaded");
    }
}

public class BoolConverter : Converter
{
	public override Type Converts { get { return typeof(bool); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		_TargetStream.Write((byte)(((bool)_Object) ? 255 : 0));
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		return _TargetStream.Read() != 0;
	}
}
