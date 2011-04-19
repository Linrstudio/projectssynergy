using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.Drawing.Drawing2D;
using SynergySequence;
using MainStation;

namespace MainStationCodeBlocks
{
    public abstract class BaseBlockInstruction : MainStationCodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
            base.Draw(_Graphics);
        }
    }

    public abstract class BaseBlockData : MainStationCodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawConstant(_Graphics);
            base.Draw(_Graphics);
        }
    }

    public abstract class BaseBlockEvent : MainStationCodeBlock
    {
        public class Event
        {
            public Event(ushort _DeviceID, byte _EventID, TriggerOutput _Output) { DeviceID = _DeviceID; EventID = _EventID; Output = _Output; }
            public byte EventID;
            public TriggerOutput Output;
            public ushort DeviceID;
        }
        //maps from event ID to triggeroutput ID
        public abstract Event[] Events { get; }

        public virtual byte[] CompileEvent(Event _Event)
        {
            MemoryStream stream = new MemoryStream();
            foreach (TriggerInput i in _Event.Output.Connected)
            {
                byte[] code = ((MainStationCodeBlock)i.Owner.Owner).Compile(i);
                stream.Write(code, 0, code.Length);
            }
            return stream.ToArray();
        }

        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
             new PointF(-width / 2, height / 4),
             new PointF(-width / 2, -height / 4),
             new PointF(0, -height / 2),
             new PointF(width / 2, -height / 4),
             new PointF(width / 2, height / 4),
             new PointF(0, height / 2));
            base.Draw(_Graphics);
        }
    }

    public class BlockBoolInvert : BaseBlockInstruction
    {
        public override GetOutputResult GetOutput(DataOutput _Output)
        {
            var v = ((MainStationCodeBlock)DataInputs[0].Connected.Owner.Owner).GetOutput(DataInputs[0].Connected);

            var reg = MainStationCompiler.GetRegister(2);
            MemoryStream stream = new MemoryStream();
            byte[] load = CodeInstructions.Load(reg.index, 0xffff);
            byte[] xor = CodeInstructions.Xor(v.Register.index, reg.index, reg.index);
            stream.Write(v.Code, 0, v.Code.Length);
            stream.Write(load, 0, load.Length);
            stream.Write(xor, 0, xor.Length);

            return new GetOutputResult(stream.ToArray(), reg);
        }
        public BlockBoolInvert()
        {
            width = 100;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddDataInput("", "bool");
            capability.AddDataOutput("", "bool");
            UpdateConnectors();
            Name = "Invert";
        }
    }

    public class BlockBoolIf : BaseBlockInstruction
    {
        public override byte[] Compile(TriggerInput _Input)
        {
            MemoryStream bstream = new MemoryStream();
            foreach (TriggerInput i in TriggerOutputs[0].Connected)
            {
                byte[] c = ((MainStationCodeBlock)i.Owner.Owner).Compile(i);
                bstream.Write(c, 0, c.Length);
            }

            MemoryStream stream = new MemoryStream();

            //CodeInstructions.Jump();

            return stream.ToArray();
        }
        public BlockBoolIf()
        {
            width = 100;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddDataInput("", "bool");
            capability.AddDataOutput("", "bool");
            UpdateConnectors();
            Name = "If";
        }
    }

    public class BlockBoolConstant : BaseBlockData
    {
        public override GetOutputResult GetOutput(DataOutput _Output)
        {
            var reg = MainStationCompiler.GetRegister(2);
            return new GetOutputResult(CodeInstructions.Load(reg.index, (byte)(val ? 0xffff : 0)), reg);
        }
        bool val;
        [Browsable(true), CategoryAttribute("Constant")]
        public bool Value { get { return val; } set { val = value; } }

        public BlockBoolConstant()
        {
            width = 50;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddDataOutput("", "bool");
            UpdateConnectors();
            Name = "Constant";
        }

        public override void Load(XElement _Data) { val = bool.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(val.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockSetDebugLed : BaseBlockInstruction
    {
        public BlockSetDebugLed()
        {
            width = 100;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddDataInput("State", "bool");
            capability.AddTriggerInput("");
            UpdateConnectors();

            Name = "Set Debug LED";
        }

        public override byte[] Compile(TriggerInput _Input)
        {
            var v = ((MainStationCodeBlock)DataInputs[0].Connected.Owner.Owner).GetOutput(DataInputs[0].Connected);
            byte[] code = CodeInstructions.SetLED(v.Register.index);

            MemoryStream stream = new MemoryStream();
            stream.Write(v.Code, 0, v.Code.Length);
            stream.Write(code, 0, code.Length);
            return stream.ToArray();
        }
    }

    public class BlockIntConstant : BaseBlockData
    {
        public override GetOutputResult GetOutput(DataOutput _Output)
        {
            var reg = MainStationCompiler.GetRegister(2);
            return new GetOutputResult(CodeInstructions.Load(reg.index, val), reg);
        }
        ushort val;
        [Browsable(true), CategoryAttribute("Constant")]
        public ushort Value { get { return val; } set { val = value; } }

        public BlockIntConstant()
        {
            width = 50;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddDataOutput("", "int");
            UpdateConnectors();
            Name = "Constant";
        }

        public override void Load(XElement _Data) { val = ushort.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(val.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public abstract class BaseBlockLocalEvent : BaseBlockEvent
    {
        public byte GetEventID()
        {
            if (Sequence != null)
            {
                byte index = 128;
                foreach (CodeBlock c in Sequence.CodeBlocks)
                {
                    if (c == this) return index;
                    if (c is BaseBlockLocalEvent) index++;
                }
            }
            Console.WriteLine("GetEventID() Sequence was null!");
            return 0;
        }
    }

    public class BlockGenericEvent : BaseBlockLocalEvent
    {
        string name = "";
        [Browsable(true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public BlockGenericEvent()
        {
            width = 100;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddTriggerOutput("");
            UpdateConnectors();
        }

        public override Event[] Events { get { return new Event[] { new Event(0, GetEventID(), TriggerOutputs[0]) }; } }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Event\n" + name.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }

        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF[]{
                    new PointF(-width/2,height/2),
                    new PointF(-width/3,0),
                    new PointF(-width/2,-height/2),
                    new PointF(width/3,-height/2),
                    new PointF(width/2,0),
                    new PointF(width/3,height/2),
                }
            );
        }

        public override void Load(XElement _Data) { name = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = name; }
    }

    public class BlockDelay : BaseBlockLocalEvent
    {
        public BlockDelay()
        {
            width = 100;
            height = 50;
            Capability capability = new Capability(this);
            capability.AddTriggerOutput("");
            capability.AddTriggerInput("Reset");
            capability.AddDataInput("Delay", "int");
            UpdateConnectors();
        }

        public byte GetTimerID()
        {
            if (Sequence != null)
            {
                byte index = 0;
                foreach (CodeBlock c in Sequence.CodeBlocks)
                {
                    if (c == this) return index;
                    if (c is BlockDelay) index++;
                }
            }
            Console.WriteLine("GetTimerID() Sequence was null!");
            return 0;
        }

        public override Event[] Events { get { return new Event[] { new Event(0, GetEventID(), TriggerOutputs[0]) }; } }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Delay", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }

        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF[]{
                    new PointF(-width/2,height/2),
                    new PointF(-width/3,0),
                    new PointF(-width/2,-height/2),
                    new PointF(width/3,-height/2),
                    new PointF(width/2,0),
                    new PointF(width/3,height/2),
                }
            );
        }

        public override byte[] Compile(CodeBlock.TriggerInput _Input)
        {
            var v = ((MainStationCodeBlock)DataInputs[0].Connected.Owner.Owner).GetOutput(DataInputs[0].Connected);
            MemoryStream stream = new MemoryStream();
            stream.Write(v.Code, 0, v.Code.Length);
            byte[] code = CodeInstructions.SetTimer(GetTimerID(), GetEventID(), v.Register.index);
            stream.Write(code, 0, code.Length);
            return stream.ToArray();
        }

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class GetOutputResult
    {
        public GetOutputResult(byte[] _code, MainStation.MainStationCompiler.RegisterEntry _Register) { Code = _code; Register = _Register; }
        public byte[] Code;
        public MainStation.MainStationCompiler.RegisterEntry Register;
    }

    public class CodeBlockEvent : BaseBlockEvent
    {
        public ushort DeviceID;
        public ushort DeviceType;
        public byte EventID;

        public override BaseBlockEvent.Event[] Events
        {
            get { return new BaseBlockEvent.Event[] { new BaseBlockEvent.Event(DeviceID, EventID, TriggerOutputs[0]) }; }
        }

        public CodeBlockEvent()
        {

        }

        public void Create()
        {
            Capabilities.Clear();
            width = 100;
            height = 50;
            ProductDataBase.Device device = ProductDataBase.GetDeviceByID(DeviceType);
            if (device != null)
            {
                ProductDataBase.Device.Event evnt = device.GetEventByID(EventID);
                if (evnt != null)
                {

                    Capability capability = new Capability(this);
                    capability.AddTriggerOutput("Trigger");

                    foreach (ProductDataBase.Device.Event.Output o in evnt.Outputs)
                    {
                        capability.AddDataOutput(o.Name, o.Type);
                    }
                }
            }

            UpdateConnectors();
        }

        public override byte[] Compile(TriggerInput _Input)
        {
            MemoryStream stream = new MemoryStream();
            foreach (TriggerInput ti in TriggerOutputs[0].Connected)
            {
                byte[] blob = ((MainStationCodeBlock)ti.Owner.Owner).Compile(ti);
                stream.Write(blob, 0, blob.Length);
            }
            return stream.ToArray();
        }

        public override void Load(XElement _Data) { DeviceID = ushort.Parse(_Data.Attribute("DeviceID").Value); DeviceType = ushort.Parse(_Data.Attribute("TypeID").Value); EventID = byte.Parse(_Data.Attribute("EventID").Value); Create(); }
        public override void Save(XElement _Data) { _Data.SetAttributeValue("DeviceID", DeviceID); _Data.SetAttributeValue("TypeID", DeviceType); _Data.SetAttributeValue("EventID", EventID); }

        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF[]{
                    new PointF(-width/2,height/2),
                    new PointF(-width/3,0),
                    new PointF(-width/2,-height/2),
                    new PointF(width/3,-height/2),
                    new PointF(width/2,0),
                    new PointF(width/3,height/2),
                }
            );
            ProductDataBase.Device device = ProductDataBase.GetDeviceByID(DeviceType);
            if (device != null)
            {
                ProductDataBase.Device.Event evnt = device.GetEventByID(EventID);
                if (evnt != null)
                {
                    Name = device.Name + "\n" + evnt.Name;
                    DrawText(_Graphics);
                }
            }
        }
    }

    public class CodeBlockInvokeRemoteEvent : BaseBlockInstruction
    {
        public ushort DeviceID;
        public ushort DeviceType;
        public byte EventID;

        public CodeBlockInvokeRemoteEvent()
        {
            width = 100;
            height = 50;
            UpdateConnectors();
        }


        public void Create()
        {
            Capabilities.Clear();
            width = 100;
            height = 50;
            ProductDataBase.Device device = ProductDataBase.GetDeviceByID(DeviceType);
            if (device != null)
            {
                Capability capability = new Capability(this);

                ProductDataBase.Device.RemoteEvent evnt = device.GetRemoteEventByID(EventID);
                if (evnt != null)
                {
                    foreach (ProductDataBase.Device.RemoteEvent.Output o in evnt.Outputs)
                    {
                        capability.AddDataOutput(o.Name, o.Type);
                    }

                    foreach (ProductDataBase.Device.RemoteEvent.Input i in evnt.Inputs)
                    {
                        if (i.Type == "void")
                            capability.AddTriggerInput(i.Name);
                        else
                            capability.AddDataInput(i.Name, i.Type);
                    }
                    if (capability.TriggerInputs.Count == 0 && (capability.TriggerInputs.Count != 0 || capability.DataOutputs.Count == 0))
                    {
                        capability.AddTriggerInput("Trigger");
                    }
                }
            }

            UpdateConnectors();
        }

        public override byte[] Compile(TriggerInput _Input)
        {
            MemoryStream stream = new MemoryStream();
            byte[] code;
            code = CodeInstructions.Load8(0, EventID);
            stream.Write(code, 0, code.Length);
            byte index = 1;

            foreach (DataInput i in DataInputs)
            {
                GetOutputResult ans = ((MainStationCodeBlock)(i.Connected.Owner.Owner)).GetOutput(i.Connected);
                stream.Write(ans.Code, 0, ans.Code.Length);
                code = CodeInstructions.Mov(ans.Register.index, index, (byte)ans.Register.size); index += (byte)ans.Register.size;
                stream.Write(code, 0, code.Length);
            }
            code = CodeInstructions.EPSend(DeviceID, index);
            stream.Write(code, 0, code.Length);

            return stream.ToArray();
        }

        public override GetOutputResult GetOutput(DataOutput _Output)
        {
            var reg = MainStationCompiler.GetRegister(2);
            MemoryStream stream = new MemoryStream();
            byte[] code;
            code = CodeInstructions.Load(reg.index, EventID); stream.Write(code, 0, code.Length);
            code = CodeInstructions.EPSend(DeviceID, (byte)(reg.size + 1)); stream.Write(code, 0, code.Length);
            code = CodeInstructions.Mov(0, reg.index, (byte)reg.size); stream.Write(code, 0, code.Length);
            return new GetOutputResult(stream.ToArray(), reg);
        }

        public override void Load(XElement _Data) { DeviceID = ushort.Parse(_Data.Attribute("DeviceID").Value); DeviceType = ushort.Parse(_Data.Attribute("TypeID").Value); EventID = byte.Parse(_Data.Attribute("EventID").Value); Create(); }
        public override void Save(XElement _Data) { _Data.SetAttributeValue("DeviceID", DeviceID); _Data.SetAttributeValue("TypeID", DeviceType); _Data.SetAttributeValue("EventID", EventID); }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawShape(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));

            ProductDataBase.Device device = ProductDataBase.GetDeviceByID(DeviceType);
            if (device != null)
            {
                ProductDataBase.Device.RemoteEvent evnt = device.GetRemoteEventByID(EventID);
                if (evnt != null)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    _Graphics.DrawString(device.Name + "\n" + evnt.Name, new Font("Arial", 10), Brushes.Black, X, Y, sf);
                }
            }
        }
    }

    public abstract class MainStationCodeBlock : CodeBlock
    {
        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Delay", "Generic Events", "blaat", typeof(BlockDelay)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Event", "Generic Events", "blaat", typeof(BlockGenericEvent)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Boolean", "blaat", typeof(BlockBoolConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invert", "Boolean", "blaat", typeof(BlockBoolInvert)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Set LED", "Debug", "i like u", typeof(BlockSetDebugLed)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Integer", "blaat", typeof(BlockIntConstant)));

            _Manager.AddPrototype(new SequenceManager.Prototype("", "", "", typeof(CodeBlockEvent), false));
            _Manager.AddPrototype(new SequenceManager.Prototype("", "", "", typeof(CodeBlockInvokeRemoteEvent), false));
        }

        public virtual byte[] Compile(TriggerInput _Input) { throw new NotImplementedException(); }
        public virtual GetOutputResult GetOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }
}
