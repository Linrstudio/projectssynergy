using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("Converter", "StringArrayConverter plugin loaded");
    }
}

public class StringArrayConverter : Converter
{
	public override Type Converts { get { return typeof(string[]); } }
	public override void WriteObject(object _Object, ByteStream _TargetStream)
	{
		string[] strings = (string[])_Object;
		Converter.Converters[typeof(ushort)].WriteObject(strings.Length, _TargetStream);
		foreach (string s in strings)
		{
			Converter.Write(s, _TargetStream);
		}
		foreach (char c in (string)_Object) _TargetStream.Write((byte)c);
	}
	public override object ReadObject(ByteStream _TargetStream)
	{
		ushort len = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
		string[] strings = new string[len];
		for (int i = 0; i < len; i++)
		{
			strings[i] = (string)Converter.Read(_TargetStream);
		}
		return strings;
	}
}
