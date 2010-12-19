using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;

namespace MainStationFrontEnd
{
    //Archetypes
    public abstract class BaseBlockConditions : CodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawTriangle(_Graphics);
            base.Draw(_Graphics);
        }
        public override void DrawShadow(Graphics _Graphics)
        {
            DrawTriangleShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public abstract class BaseBlockMath : CodeBlock
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

    public abstract class BaseBlockConstant : CodeBlock
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

    public abstract class BaseBlockVariable : CodeBlock
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

    public abstract class BaseBlockOther : CodeBlock
    {
    }

    public abstract class BaseBlockBranch : CodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawScope(_Graphics);
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawScopeShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public abstract class BaseBlockEvent : CodeBlock
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

    public class BlockSetDebugLed1 : BaseBlockOther
    {
        public BlockSetDebugLed1()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, "Invoke"));
            DataInputs.Add(new DataInput(this, "On?", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.SetLED(Inputs[0].Connected.Register.Index);
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
    }

    public class BlockGetTime : BaseBlockVariable
    {
        public BlockGetTime()
        {
            width = 75;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "Current Time", GetDataType("time")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; 
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Time", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockGetHour : BaseBlockVariable
    {
        public BlockGetHour()
        {
            width = 75;
            height = 25;
            DataInputs.Add(new DataInput(this, "", null));
            DataOutputs.Add(new DataOutput(this, "Current Hour", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.GetHour(Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Hour", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
    public class BlockGetMinute : BaseBlockVariable
    {
        public BlockGetMinute()
        {
            width = 75;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "Current Minute", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.GetMinute(Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Minute", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
    public class BlockGetSecond : BaseBlockVariable
    {
        public BlockGetSecond()
        {
            width = 75;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "Current Second", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.GetSecond(Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Second", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
    public class BlockGetDay : BaseBlockVariable
    {
        public BlockGetDay()
        {
            width = 75;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "Current Weekday", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; 
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Day", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockMathEquals : BaseBlockConditions
    {
        public BlockMathEquals()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            TriggerOutputs.Add(new TriggerOutput(this, "True"));
            TriggerOutputs.Add(new TriggerOutput(this, "False"));

            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Equals(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Equals", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockBoolEquals : BaseBlockConditions
    {
        public BlockBoolEquals()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("bool")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("bool")));
            DataOutputs.Add(new DataOutput(this, "A equals B", GetDataType("bool")));

            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Equals(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Equals", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockMathDiffers : BaseBlockConditions
    {
        public BlockMathDiffers()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A differs from B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Differs(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Differs", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockBoolDiffers : BaseBlockConditions
    {
        public BlockBoolDiffers()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("bool")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("bool")));
            DataOutputs.Add(new DataOutput(this, "A differs from B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Differs(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Differs", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockMathSmallerThan : BaseBlockConditions
    {
        public BlockMathSmallerThan()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A smaller than B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                //Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Smaller Than", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockMathLargerThan : BaseBlockConditions
    {
        public BlockMathLargerThan()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A larger than B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                //Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Larger Than", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockBoolAnd : BaseBlockMath
    {
        public BlockBoolAnd()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("bool")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("bool")));
            DataOutputs.Add(new DataOutput(this, "A and B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.And(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("And", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockBoolOr : BaseBlockMath
    {
        public BlockBoolOr()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("bool")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("bool")));
            DataOutputs.Add(new DataOutput(this, "A or B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Or(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Or", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockMathConstant : BaseBlockConstant
    {
        ushort val;

        [Browsable(true), CategoryAttribute("Constant")]
        public ushort Value
        {
            get { return val; }
            set { val = value; }
        }

        public BlockMathConstant()
        {
            width = 50;
            height = 50;
            DataOutputs.Add(new DataOutput(this, "Constant", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Load(Outputs[0].Register.Index, Value);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 20, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { val = ushort.Parse(_Data.Value.ToString()); }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }
    }

    public class BlockBoolConstant : BaseBlockConstant
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
            DataOutputs.Add(new DataOutput(this, "Constant", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Load(Outputs[0].Register.Index, (ushort)(Value ? 65535 : 0));
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
        public override void Load(XElement _Data) { val = bool.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value=val.ToString(); }
    }

    public class BlockMathAdd : BaseBlockMath
    {
        public BlockMathAdd()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A + B", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Add(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);

        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Add", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockBoolXOR : BaseBlockMath
    {
        public BlockBoolXOR()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("bool")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("bool")));
            DataOutputs.Add(new DataOutput(this, "A Xor B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Xor(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);

        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Exclusive Or", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
    public class BlockMathSubstract : BaseBlockMath
    {
        public BlockMathSubstract()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A-B", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Substract(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }


        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Substract", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
    public class BlockMathMultiply : BaseBlockMath
    {
        public BlockMathMultiply()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A*B", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Multiply(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }


        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Multiply", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
    public class BlockMathDivide : BaseBlockMath
    {
        public BlockMathDivide()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "A", GetDataType("int")));
            DataInputs.Add(new DataInput(this, "B", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "A/B", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = CodeInstructions.Divide(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Divide", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }

    public class BlockMathVariable : BaseBlockConstant
    {
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
            DataInputs.Add(new DataInput(this, "Value", GetDataType("int")));
            DataOutputs.Add(new DataOutput(this, "Value", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = new byte[] { BlockID, EEPROM.GetVariableAddress(name), Inputs[0].Connected.RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Set Variable", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y - 8, sf);
            _Graphics.DrawString(Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y + 8, sf);
        }
        public override void Load(XElement _Data) { name = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = name; }
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

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Schedule", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y - 8, sf);
            _Graphics.DrawString(string.Format("{0:00}:{1:00}", time.time.Hours, time.time.Minutes), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y + 8, sf);
        }
        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
}
