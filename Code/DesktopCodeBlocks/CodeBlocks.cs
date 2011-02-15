using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;
using MainStation;

namespace DesktopCodeBlocks
{
    //Archetypes
    public abstract class BaseBlockInstruction : DesktopCodeBlock
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

    public abstract class BaseBlockData : DesktopCodeBlock
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

    public abstract class BaseBlockOther : DesktopCodeBlock
    {
    }

    public abstract class BaseBlockEvent : DesktopCodeBlock
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

    public class BlockMathPrint : BaseBlockOther
    {
        public BlockMathPrint()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, "Invoke"));
            DataInputs.Add(new DataInput(this, "Value", "int"));
            UpdateConnectors();
        }

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
            _Graphics.DrawString("Debug Led 1", new Font("Arial", 10), Brushes.Black, X, Y, sf);
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

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { System.Diagnostics.Debug.WriteLine(GetInput(DataInputs[0])); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockMathEquals : BaseBlockInstruction
    {

        public BlockMathEquals()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", "int"));
            DataInputs.Add(new DataInput(this, "B", "int"));
            TriggerInputs.Add(new TriggerInput(this, "Trigger"));
            TriggerOutputs.Add(new TriggerOutput(this, "True"));
            TriggerOutputs.Add(new TriggerOutput(this, "False"));

            UpdateConnectors();
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { if (GetInput(DataInputs[0]) == GetInput(DataInputs[1]))Trigger(TriggerOutputs[0]); else Trigger(TriggerOutputs[1]); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockMathMultiply : BaseBlockInstruction
    {
        public BlockMathMultiply()
        {
            width = 100;
            height = 50;
            Name = "Multiply";
            DataInputs.Add(new DataInput(this, "A", "int"));
            DataInputs.Add(new DataInput(this, "B", "int"));
            DataOutputs.Add(new DataOutput(this, "C", "int"));

            UpdateConnectors();
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            float a = (float)GetInput(DataInputs[0]);
            float b = (float)GetInput(DataInputs[1]);
            return a * b;
        }
    }

    public class BlockMathAdd : BaseBlockInstruction
    {
        public BlockMathAdd()
        {
            width = 100;
            height = 50;
            Name = "Add";
            DataInputs.Add(new DataInput(this, "A", "int"));
            DataInputs.Add(new DataInput(this, "B", "int"));
            DataOutputs.Add(new DataOutput(this, "C", "int"));

            UpdateConnectors();
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            float a = (float)GetInput(DataInputs[0]);
            float b = (float)GetInput(DataInputs[1]);
            return a + b;
        }
    }

    public class BlockMathSubstract : BaseBlockInstruction
    {
        public BlockMathSubstract()
        {
            width = 100;
            height = 50;
            Name = "Substract";
            DataInputs.Add(new DataInput(this, "A", "int"));
            DataInputs.Add(new DataInput(this, "B", "int"));
            DataOutputs.Add(new DataOutput(this, "C", "int"));

            UpdateConnectors();
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            float a = (float)GetInput(DataInputs[0]);
            float b = (float)GetInput(DataInputs[1]);
            return a - b;
        }
    }

    public class BlockMathDivide : BaseBlockInstruction
    {
        public BlockMathDivide()
        {
            width = 100;
            height = 50;
            Name = "Divide";
            DataInputs.Add(new DataInput(this, "A", "int"));
            DataInputs.Add(new DataInput(this, "B", "int"));
            DataOutputs.Add(new DataOutput(this, "C", "int"));

            UpdateConnectors();
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            float a = (float)GetInput(DataInputs[0]);
            float b = (float)GetInput(DataInputs[1]);
            return a / b;
        }
    }

    public class BlockMathConstant : BaseBlockData
    {
        float val;

        [Browsable(true), CategoryAttribute("Constant")]
        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public BlockMathConstant()
        {
            width = 50;
            height = 50;
            DataOutputs.Add(new DataOutput(this, "Constant", "int"));
            UpdateConnectors();
        }
        public override void Load(XElement _Data) { val = float.Parse(_Data.Value.ToString()); }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output) { return val; }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockBoolConstant : BaseBlockData
    {
        bool val;
        [Browsable(true), CategoryAttribute("Constant")]
        public bool Value
        {
            get { return val; }
            set { val = value; }
        }

        public BlockBoolConstant()
        {
            width = 50;
            height = 50;
            DataOutputs.Add(new DataOutput(this, "Constant", "bool"));
            UpdateConnectors();
        }

        public override void Load(XElement _Data) { val = bool.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output) { return val; }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 15, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockBoolInvert : BaseBlockInstruction
    {
        public BlockBoolInvert()
        {
            width = 100;
            height = 25;
            DataInputs.Add(new DataInput(this, "", "bool"));
            DataOutputs.Add(new DataOutput(this, "", "bool"));
            UpdateConnectors();
            Name = "Invert";
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            return !(bool)GetInput(DataInputs[0]);
        }
    }

    public class BlockBoolEquals : BaseBlockInstruction
    {
        public BlockBoolEquals()
        {
            width = 100;
            height = 25;
            DataInputs.Add(new DataInput(this, "", "bool"));
            DataInputs.Add(new DataInput(this, "", "bool"));
            DataOutputs.Add(new DataOutput(this, "", "bool"));
            UpdateConnectors();
            Name = "Invert";
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            return ((bool)GetInput(DataInputs[0]) == (bool)GetInput(DataInputs[1]));
        }
    }

    public class BlockIf : BaseBlockInstruction
    {
        public BlockIf()
        {
            width = 100;
            height = 25;
            DataInputs.Add(new DataInput(this, "Condition", "bool"));
            TriggerInputs.Add(new TriggerInput(this, ""));
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            Name = "If";
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input)
        {
            if ((bool)GetInput(DataInputs[0]))
            {
                Trigger(TriggerOutputs[0]);
            }
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockIfNot : BaseBlockInstruction
    {
        public BlockIfNot()
        {
            width = 100;
            height = 25;
            DataInputs.Add(new DataInput(this, "Condition", "bool"));
            TriggerInputs.Add(new TriggerInput(this, ""));
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            Name = "If not";
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input)
        {
            if (!(bool)GetInput(DataInputs[0]))
            {
                Trigger(TriggerOutputs[0]);
            }
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockInvokeRemote : BaseBlockInstruction
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

        public BlockInvokeRemote()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, ""));
            UpdateConnectors();
            Name = "Invoke Remote Event";
        }

        public override void Load(XElement _Data) { deviceid = ushort.Parse(_Data.Attribute("DeviceID").Value); eventid = byte.Parse(_Data.Attribute("EventID").Value); }
        public override void Save(XElement _Data) { _Data.SetAttributeValue("DeviceID", deviceid); _Data.SetAttributeValue("EventID", eventid); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input)
        {
            MainStation.MainStation.InvokeLocalEvent(deviceid, eventid, 0);
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockMathVariable : BaseBlockData
    {
        static Dictionary<string, float> variables = new Dictionary<string, float>();
        string name;

        [Browsable(true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public BlockMathVariable()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "Value", "int"));
            DataOutputs.Add(new DataOutput(this, "Value", "int"));
            UpdateConnectors();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(name, new Font("Arial", 20, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }

        public override void Load(XElement _Data) { name = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = name; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { if (!variables.ContainsKey(name))variables.Add(name, (float)_Data); else variables[name] = (float)_Data; }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output) { if (variables.ContainsKey(name)) return variables[name]; else return 0; }
    }

    public class BlockEventSchedule : BaseBlockEvent
    {
        DateTime Time;
        public BlockEventSchedule()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input) { Trigger(TriggerOutputs[0]); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockEventDelay : BaseBlockEvent
    {
        float val;
        [Browsable(true), CategoryAttribute("Delay")]
        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public bool Enabled = false;
        public DateTime invokedate;

        public BlockEventDelay()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            TriggerInputs.Add(new TriggerInput(this, "Start"));
            TriggerInputs.Add(new TriggerInput(this, "Abort"));
            UpdateConnectors();
        }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }
        public override void Load(XElement _Data) { val = float.Parse(_Data.Value); }

        public override void Update()
        {
            if (Enabled && DateTime.Now > invokedate)
            {
                ((DesktopSequence)Sequence).AddEvent(new DesktopSequence.Event(this));
                Enabled = false;
            }
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            if (_Input == TriggerInputs[0])
            {
                invokedate = DateTime.Now.AddSeconds(val);
                Enabled = true;
            }
            if (_Input == TriggerInputs[1])
                Enabled = false;
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Delay " + Value.ToString() + "s", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public abstract class DesktopCodeBlock : CodeBlock
    {
        public virtual void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public abstract object HandleOutput(CodeBlock.DataOutput _Output);
        public abstract void HandleTrigger(CodeBlock.TriggerInput _Input);
        public void Trigger(CodeBlock.TriggerOutput _Output)
        {
            foreach (CodeBlock.TriggerInput i in _Output.Connected)
            {
                ((DesktopCodeBlock)i.Owner).HandleTrigger(i);
            }
        }

        public object GetInput(CodeBlock.DataInput _Input)
        {
            return ((DesktopCodeBlock)_Input.Connected.Owner).HandleOutput(_Input.Connected);
        }

        /// <summary>
        /// gives the codeblock a chance to for example invoke a event himself ( a schedule event ? )
        /// </summary>
        public virtual void Update() { }

        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Delay", "Generic", "im in like with u", typeof(DesktopCodeBlocks.BlockEventDelay)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Boolean", "im in like with u", typeof(DesktopCodeBlocks.BlockBoolConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invert", "Boolean", "im in like with u", typeof(DesktopCodeBlocks.BlockBoolInvert)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Equals", "Boolean", "im in like with u", typeof(DesktopCodeBlocks.BlockBoolEquals)));
            _Manager.AddPrototype(new SequenceManager.Prototype("If", "Boolean", "im in like with u", typeof(DesktopCodeBlocks.BlockIf)));
            _Manager.AddPrototype(new SequenceManager.Prototype("IfNot", "Boolean", "im in like with u", typeof(DesktopCodeBlocks.BlockIfNot)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Multiply", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathMultiply)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Divide", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathMultiply)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Add", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathMultiply)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Substract", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathMultiply)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Print", "mygroup", "i like u", typeof(DesktopCodeBlocks.BlockMathPrint)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Invoke Remote", "Generic", "i like u", typeof(DesktopCodeBlocks.BlockInvokeRemote)));
        }
    }
}
