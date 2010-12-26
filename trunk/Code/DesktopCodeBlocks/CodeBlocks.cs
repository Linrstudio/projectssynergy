using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;

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
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { System.Diagnostics.Debug.WriteLine(GetInput(DataInputs[0])); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockSetDebugLed1 : BaseBlockOther
    {
        public BlockSetDebugLed1()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, "Invoke"));
            DataInputs.Add(new DataInput(this, "On?", "bool"));
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

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }

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

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }

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

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Output) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            float a = (float)GetInput(DataInputs[0]);
            float b = (float)GetInput(DataInputs[1]);
            return a * b;
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

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            return !(bool)GetInput(DataInputs[0]);
        }
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
        Time time;
        public BlockEventSchedule()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            IsEvent = true;
        }

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input) { Trigger(TriggerOutputs[0]); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }
}
