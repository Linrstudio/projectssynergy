using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "FloatConverter plugin loaded");
    }
}

public class FloatConverter : Converter
{
	public override Type Converts { get { return typeof(float); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		_TargetStream.Write(BitConverter.GetBytes((Single)_Object));
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		return BitConverter.ToSingle(_TargetStream.Read(4), 0);
	}
}
