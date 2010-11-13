using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "ByteConverter plugin loaded");
    }
}

public class ByteConverter : Converter
{
	public override Type Converts { get { return typeof(byte); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		_TargetStream.Write((byte)_Object);
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		return _TargetStream.Read();
	}
}