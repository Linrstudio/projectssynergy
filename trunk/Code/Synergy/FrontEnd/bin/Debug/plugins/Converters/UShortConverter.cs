using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "UShortConverter plugin loaded");
    }
}

public class UShortConverter : Converter
{
	public override Type Converts { get { return typeof(ushort); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		ushort obj = (ushort)_Object;
		_TargetStream.Write(BitConverter.GetBytes((ushort)_Object));
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		return BitConverter.ToUInt16(_TargetStream.Read(2), 0);
	}
}
