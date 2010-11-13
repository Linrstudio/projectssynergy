using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "ByteArrayConverter plugin loaded");
    }
}

public class ByteArrayConverter : Converter
{
	public override Type Converts { get { return typeof(byte[]); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		byte[] obj = (byte[])_Object;
		Converter.Converters[typeof(ushort)].WriteObject((ushort)obj.Length, _TargetStream);
		_TargetStream.Write((byte[])_Object);
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		ushort len = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
		return _TargetStream.Read(len);
	}
}
