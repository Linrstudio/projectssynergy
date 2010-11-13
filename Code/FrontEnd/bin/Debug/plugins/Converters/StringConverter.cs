using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Synergy;

public class Program
{
    public static void Main()
    {
        Log.Write("StringConverter Plugin", "StringConverter plugin loaded");
    }
}

public class StringConverter : Converter
{
    public override Type Converts { get { return typeof(string); } }
    public StringConverter()
    {
        Log.Write("My Plugin", "StringConverter plugin loaded");
    }

    public override void WriteObject(object _Object, ByteStream _TargetStream)
    {
        string obj = (string)_Object;
        Converter.Converters[typeof(ushort)].WriteObject((ushort)obj.Length, _TargetStream);
        foreach (char c in (string)_Object) _TargetStream.Write((byte)c);
    }
    public override object ReadObject(ByteStream _TargetStream)
    {
        ushort len = (ushort)Converter.Converters[typeof(ushort)].ReadObject(_TargetStream);
        return System.Text.Encoding.ASCII.GetString(_TargetStream.Read(len));
    }
}
