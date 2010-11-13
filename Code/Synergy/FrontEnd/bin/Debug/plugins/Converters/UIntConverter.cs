using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "UIntConverter plugin loaded");
    }
}

public class UIntConverter : Converter
{
	public override Type Converts { get { return typeof(UInt32); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		_TargetStream.Write(BitConverter.GetBytes((uint)_Object));
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		return BitConverter.ToUInt32(_TargetStream.Read(4), 0);
	}
}
