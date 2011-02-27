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

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawShapeShadow(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
            base.DrawShadow(_Graphics);
        }
    }

    public abstract class BaseBlockData : MainStationCodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawConstant(_Graphics);
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawConstantShadow(_Graphics);
            base.DrawShadow(_Graphics);
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
                byte[] code = ((MainStationCodeBlock)i.Owner).Compile(i);
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

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawShapeShadow(_Graphics,
             new PointF(-width / 2, height / 4),
             new PointF(-width / 2, -height / 4),
             new PointF(0, -height / 2),
             new PointF(width / 2, -height / 4),
             new PointF(width / 2, height / 4),
             new PointF(0, height / 2));
            base.DrawShadow(_Graphics);
        }
    }

    public class BlockBoolInvert : BaseBlockInstruction
    {
        public override GetOutputResult GetOutput(DataOutput _Output)
        {
            var v = ((MainStationCodeBlock)DataInputs[0].Connected.Owner).GetOutput(DataInputs[0].Connected);

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
            DataInputs.Add(new DataInput(this, "", "bool"));
            DataOutputs.Add(new DataOutput(this, "", "bool"));
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
                byte[] c = ((MainStationCodeBlock)i.Owner).Compile(i);
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
            DataInputs.Add(new DataInput(this, "", "bool"));
            DataOutputs.Add(new DataOutput(this, "", "bool"));
            UpdateConnectors();
            Name = "If";
        }
    }

    public class BlockBoolConstant : BaseBlockData
    {
        public override GetOutputResult GetOutput(DataOutput _Output)
        {
            var reg = MainStationCompiler.GetRegister(2);
            return new GetOutputResult(CodeInstructions.Load(reg.index, (ushort)(val ? 0xffff : 0)), reg);
        }
        bool val;
        [Browsable(true), CategoryAttribute("Constant")]
        public bool Value { get { return val; } set { val = value; } }

        public BlockBoolConstant()
        {
            width = 50;
            height = 50;
            DataOutputs.Add(new DataOutput(this, "", "bool"));
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
            DataInputs.Add(new DataInput(this, "State", "bool"));
            TriggerInputs.Add(new TriggerInput(this, ""));
            UpdateConnectors();

            Name = "Set Debug LED";
        }

        public override byte[] Compile(TriggerInput _Input)
        {
            var v = ((MainStationCodeBlock)DataInputs[0].Connected.Owner).GetOutput(DataInputs[0].Connected);
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
            DataOutputs.Add(new DataOutput(this, "", "int"));
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
    /*
    public class BlockEventSchedule : BaseBlockEvent
    {
        DateTime moment;

        [Browsable(true)]
        public DateTime Moment
        {
            get { return moment; }
            set { moment = value; }
        }

        public BlockEventSchedule()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
        }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Schedule\n" + moment.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }

        public Event[] Events { get { return new Event[]{new Event(0,get)}} }

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
*/
    public class BlockGenericEvent : BaseBlockEvent
    {
        string name = "";
        [Browsable(true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public byte GetEventID()
        {
            if (Sequence != null)
            {
                byte index = 128;
                foreach (CodeBlock c in Sequence.CodeBlocks)
                {
                    if (c == this) return index;
                    if (c is BlockGenericEvent) index++;
                }
            }
            return 0;
        }
        public BlockGenericEvent()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
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

    public class GetOutputResult
    {
        public GetOutputResult(byte[] _code, MainStation.MainStationCompiler.RegisterEntry _Register) { Code = _code; Register = _Register; }
        public byte[] Code;
        public MainStation.MainStationCompiler.RegisterEntry Register;
    }

    public abstract class MainStationCodeBlockEvent : MainStationCodeBlock
    {

    }

    public class MainStationCodeBlockDevice : MainStationCodeBlocks.BaseBlockEvent
    {
        ushort deviceid;
        [Browsable(true)]
        public ushort DeviceID
        {
            get { return deviceid; }
            set
            {
                deviceid = value;
                foreach (Event e in Events) e.DeviceID = deviceid; //update device id's in events
            }
        }

        public ushort type;

        public void Create()
        {
            ProductDataBase.Device device = ProductDataBase.GetDeviceByID(type);
            int events = 0;
            width = 100;
            height = 200;

            foreach (ProductDataBase.Device.Event e in device.events)
            {
                TriggerOutput newoutput = new TriggerOutput(this, e.Name);
                TriggerOutputs.Add(newoutput);
                events++;
            }

            height = events * 30;

            UpdateConnectors();
        }

        public override Event[] Events
        {
            get
            {
                ProductDataBase.Device device = ProductDataBase.GetDeviceByID(type);
                List<Event> events = new List<Event>();
                int index = 0;
                foreach (ProductDataBase.Device.Event e in device.events)
                {
                    Event newevent = new Event(deviceid, e.ID, TriggerOutputs[index]);
                    events.Add(newevent);
                    index++;
                }
                return events.ToArray();
            }
        }

        public override void Load(XElement _Data) { deviceid = ushort.Parse(_Data.Attribute("DeviceID").Value); type = ushort.Parse(_Data.Attribute("TypeID").Value); Create(); }
        public override void Save(XElement _Data) { _Data.SetAttributeValue("DeviceID", deviceid); _Data.SetAttributeValue("TypeID", type); }


    }

    public class MainStationCodeBlockInvokeRemoteEvent : BaseBlockInstruction
    {
        ushort deviceid;
        [Browsable(true)]
        public ushort DeviceID
        {
            get { return deviceid; }
            set { deviceid = value; }
        }

        byte eventid;
        [Browsable(true)]
        public byte EventID
        {
            get { return eventid; }
            set { eventid = value; }
        }

        public MainStationCodeBlockInvokeRemoteEvent()
        {
            width = 100;
            height = 50;
            TriggerInputs.Add(new TriggerInput(this, "Hellyea!"));
            DataInputs.Add(new DataInput(this, "Arguments", "int"));
            UpdateConnectors();
        }

        public override byte[] Compile(TriggerInput _Input)
        {
            MemoryStream stream = new MemoryStream();
            byte[] code;
            code = CodeInstructions.Load8(0, eventid);
            stream.Write(code, 0, code.Length);
            if (DataInputs[0].Connected != null)
            {
                var ans = ((MainStationCodeBlock)DataInputs[0].Connected.Owner).GetOutput(DataInputs[0].Connected);
                stream.Write(ans.Code, 0, ans.Code.Length);
                code = CodeInstructions.Mov(ans.Register.index, 1, 2);
                stream.Write(code, 0, code.Length);
                code = CodeInstructions.EPSend(deviceid, 3);
                stream.Write(code, 0, code.Length);
            }
            else
            {
                code = CodeInstructions.EPSend(deviceid, 1);
                stream.Write(code, 0, code.Length);
            }
            return stream.ToArray();
        }

        public override void Load(XElement _Data) { deviceid = ushort.Parse(_Data.Attribute("DeviceID").Value); eventid = byte.Parse(_Data.Attribute("EventID").Value); }
        public override void Save(XElement _Data) { _Data.SetAttributeValue("DeviceID", deviceid); _Data.SetAttributeValue("EventID", eventid); }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawShape(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Device " + DeviceID + " Event " + EventID, new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class MainStationCodeBlockRemoteEvent : MainStationCodeBlockEvent
    {
        ushort deviceid;
        [Browsable(true)]
        public ushort DeviceID
        {
            get { return deviceid; }
            set { deviceid = value; }
        }

        byte eventid;
        [Browsable(true)]
        public byte EventID
        {
            get { return eventid; }
            set { eventid = value; }
        }

        public MainStationCodeBlockRemoteEvent()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, "Hellyea!"));
            UpdateConnectors();
        }

        public override byte[] Compile(TriggerInput _Input)
        {
            MemoryStream stream = new MemoryStream();
            foreach (TriggerInput ti in TriggerOutputs[0].Connected)
            {
                byte[] blob = ((MainStationCodeBlock)ti.Owner).Compile(ti);
                stream.Write(blob, 0, blob.Length);
            }
            return stream.ToArray();
        }

        public override void Load(XElement _Data) { deviceid = ushort.Parse(_Data.Attribute("DeviceID").Value); eventid = byte.Parse(_Data.Attribute("EventID").Value); }
        public override void Save(XElement _Data) { _Data.SetAttributeValue("DeviceID", deviceid); _Data.SetAttributeValue("EventID", eventid); }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawShape(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Device " + DeviceID + " Event " + EventID, new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public abstract class MainStationCodeBlock : CodeBlock
    {
        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Event", "Generic Events", "blaat", typeof(BlockGenericEvent)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Remote Event", "Generic Events", "blaat", typeof(MainStationCodeBlockRemoteEvent)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invoke Remote Event", "Generic Events", "i like u", typeof(MainStationCodeBlockInvokeRemoteEvent)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Boolean", "blaat", typeof(MainStationCodeBlocks.BlockBoolConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invert", "Boolean", "blaat", typeof(MainStationCodeBlocks.BlockBoolInvert)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Set LED", "Debug", "i like u", typeof(MainStationCodeBlocks.BlockSetDebugLed)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Integer", "blaat", typeof(MainStationCodeBlocks.BlockIntConstant)));

            _Manager.AddPrototype(new SequenceManager.Prototype("", "", "", typeof(MainStationCodeBlocks.MainStationCodeBlockDevice), false));
        }

        public virtual byte[] Compile(TriggerInput _Input) { throw new NotImplementedException(); }
        public virtual GetOutputResult GetOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }
}
