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

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
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
                byte[] blob = ((MainStationCodeBlock)(ti.Owner)).Compile(ti);
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
            _Manager.AddPrototype(new SequenceManager.Prototype("Remote Event", "Generic Events", "blaat", typeof(MainStationCodeBlockRemoteEvent)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Boolean", "blaat", typeof(MainStationCodeBlocks.BlockBoolConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invert", "Boolean", "blaat", typeof(MainStationCodeBlocks.BlockBoolInvert)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Set LED", "Debug", "i like u", typeof(MainStationCodeBlocks.BlockSetDebugLed)));
        }


        public virtual byte[] Compile(TriggerInput _Input) { throw new NotImplementedException(); }
        public virtual GetOutputResult GetOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }
}
