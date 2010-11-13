using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "ByteStreamConverter plugin loaded");
    }
}

public class ByteStreamConverter : Converter
{
	public override Type Converts { get { return typeof(ByteStream); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		ByteStream obj = (ByteStream)_Object;
		Converter.Converters[typeof(ushort)].WriteObject((ushort)obj.Length, _TargetStream);
		_TargetStream.Write(((ByteStream)_Object).CopyAll());
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		ushort length = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
		return new ByteStream(_TargetStream.Read(length));
	}
}