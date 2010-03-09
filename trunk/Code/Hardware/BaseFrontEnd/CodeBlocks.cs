using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace BaseFrontEnd
{
    //Archetypes
    public class BaseBlockConditions : CodeBlock
    {
        public BaseBlockConditions(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
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

    public class BaseBlockMath : CodeBlock
    {
        public BaseBlockMath(KismetSequence _Sequence, byte _BlockID)
            : base(_Sequence, _BlockID)
        {

        }
    }

    public class BaseBlockConstant : CodeBlock
    {
        public BaseBlockConstant(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }

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

    public class BaseBlockVariable : CodeBlock
    {
        public BaseBlockVariable(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }

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

    public class BaseBlockOther : CodeBlock
    {
        public BaseBlockOther(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
    }

    public class BaseBlockBranch : CodeBlock
    {
        public BaseBlockBranch(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
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

    public class BaseBlockEvent : CodeBlock
    {
        public BaseBlockEvent(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
    }

    public class BlockSetDebugLed1 : BaseBlockOther
    {
        public BlockSetDebugLed1(KismetSequence _Sequence)
            : base(_Sequence, 2)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this, "On?"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex };
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
    }

    public class BlockSetDebugLed2 : BaseBlockOther
    {
        public BlockSetDebugLed2(KismetSequence _Sequence)
            : base(_Sequence, 3)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this, "On?"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex };
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
            _Graphics.DrawString("Debug Led 2", new Font("Arial", 10), Brushes.Black, X, Y, sf);
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

    public class BlockGetHour : BaseBlockVariable
    {
        public BlockGetHour(KismetSequence _Sequence)
            : base(_Sequence, 6)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Current Hour"));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Hour", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }
    public class BlockGetMinute : BaseBlockVariable
    {
        public BlockGetMinute(KismetSequence _Sequence)
            : base(_Sequence, 7)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Current Minute"));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Minute", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }
    public class BlockGetSecond : BaseBlockVariable
    {
        public BlockGetSecond(KismetSequence _Sequence)
            : base(_Sequence, 8)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Current Second"));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Second", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }
    public class BlockGetDay : BaseBlockVariable
    {
        public BlockGetDay(KismetSequence _Sequence)
            : base(_Sequence, 9)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Current Weekday"));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Day", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockEquals : BaseBlockConditions
    {
        public BlockEquals(KismetSequence _Sequence)
            : base(_Sequence, 5)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A equals B"));

            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Equals", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockDiffers : BaseBlockConditions
    {
        public BlockDiffers(KismetSequence _Sequence)
            : base(_Sequence, 18)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A differs from B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Differs", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockSmallerThan : BaseBlockConditions
    {
        public BlockSmallerThan(KismetSequence _Sequence)
            : base(_Sequence, 21)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A smaller than B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
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
    }

    public class BlockLargerThan : BaseBlockConditions
    {
        public BlockLargerThan(KismetSequence _Sequence)
            : base(_Sequence, 22)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A larger than B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
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
    }

    public class BlockConstantByte : BaseBlockConstant
    {
        ushort val;

        [Browsable(true), CategoryAttribute("Constant")]
        public ushort Value
        {
            get { return val; }
            set { val = value; }
        }

        public override string GetValues()
        {
            return val.ToString();
        }

        public override void SetValues(string _Values)
        {
            val = ushort.Parse(_Values);
        }

        public BlockConstantByte(KismetSequence _Sequence)
            : base(_Sequence, 1)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Constant"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            byte[] val = Utilities.FromShort(Value);
            Code = new byte[] { BlockID, Outputs[0].RegisterIndex, val[0], val[1] };
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 20, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockConstantWeekDay : BaseBlockConstant
    {
        DayOfWeek val;

        [Browsable(true), CategoryAttribute("Constant")]
        public DayOfWeek Value
        {
            get { return val; }
            set { val = value; }
        }


        public override string GetValues()
        {
            return ((int)val).ToString();
        }

        public override void SetValues(string _Values)
        {
            val = (DayOfWeek)int.Parse(_Values);
        }

        public BlockConstantWeekDay(KismetSequence _Sequence)
            : base(_Sequence, 1)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Constant Weekday"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Outputs[0].RegisterIndex, 0, (byte)((int)Value) };
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockAdd : BaseBlockMath
    {
        public BlockAdd(KismetSequence _Sequence)
            : base(_Sequence, 10)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A + B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
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
    }
    public class BlockSubstract : BaseBlockMath
    {
        public BlockSubstract(KismetSequence _Sequence)
            : base(_Sequence, 11)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A-B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
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
    }
    public class BlockMultiply : BaseBlockMath
    {
        public BlockMultiply(KismetSequence _Sequence)
            : base(_Sequence, 12)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A*B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }


        public override void Draw(Graphics _Graphics)
        {
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Multiply", new Font("Arial", 10), Brushes.Black, X, Y, sf);
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }
    public class BlockDivide : BaseBlockMath
    {
        public BlockDivide(KismetSequence _Sequence)
            : base(_Sequence, 13)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A/B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }


        public override void Draw(Graphics _Graphics)
        {
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Divide", new Font("Arial", 10), Brushes.Black, X, Y, sf);
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }
    public class BlockBitMask : BaseBlockMath
    {
        public BlockBitMask(KismetSequence _Sequence)
            : base(_Sequence, 15)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A"));
            Inputs.Add(new Input(this, "B"));
            Outputs.Add(new Output(this, "A and B"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }


        public override void Draw(Graphics _Graphics)
        {
            DrawBlock(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Bitwise And", new Font("Arial", 10), Brushes.Black, X, Y, sf);
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawBlockShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public class BlockGetVariable : BaseBlockVariable
    {
        string name;

        [Browsable(true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string GetValues()
        {
            return name;
        }

        public override void SetValues(string _Values)
        {
            name = _Values;
        }

        public BlockGetVariable(KismetSequence _Sequence)
            : base(_Sequence, 17)
        {
            height = 50;
            width = 100;
            Inputs.Add(new Input(this, ""));
            Outputs.Add(new Output(this, "Value"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Sequence.Event.device.eeprom.GetVariableAddress(name), Outputs[0].RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Get Variable", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y - 8, sf);
            _Graphics.DrawString(Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y + 8, sf);
        }
    }
    public class BlockSetVariable : BaseBlockVariable
    {
        string name;

        [Browsable(true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public override string GetValues()
        {
            return name;
        }

        public override void SetValues(string _Values)
        {
            name = _Values;
        }

        public BlockSetVariable(KismetSequence _Sequence)
            : base(_Sequence, 16)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "Value"));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Sequence.Event.device.eeprom.GetVariableAddress(name), Inputs[0].Connected.RegisterIndex };
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
    }

    public class DefaultEvent : BaseBlockEvent
    {
        public DefaultEvent(KismetSequence _Sequence)
            : base(_Sequence, 0)
        {
            IsScope = true;
            width = 120;
            height = 200;
            Outputs.Add(new Output(this, ""));
            UpdateConnectors();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawScope(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Sequence.Event.eventtype.Name, new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawScopeShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public class BlockIf : BaseBlockBranch
    {
        public BlockIf(KismetSequence _Sequence)
            : base(_Sequence, 19)
        {
            IsScope = true;
            width = 120;
            height = 100;
            Inputs.Add(new Input(this, "Condition"));
            Outputs.Add(new Output(this, ""));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            CodeBlock cwhi = GetChildWithHighestIndex();
            int target = (byte)(cwhi.address + cwhi.Code.Length);
            Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, (byte)target };
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("If", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockIfNot : BaseBlockBranch
    {
        public BlockIfNot(KismetSequence _Sequence)
            : base(_Sequence, 20)
        {
            IsScope = true;
            width = 120;
            height = 100;
            Inputs.Add(new Input(this, "Condition"));
            Outputs.Add(new Output(this, ""));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            CodeBlock cwhi = GetChildWithHighestIndex();
            int target = (byte)(cwhi.address + cwhi.Code.Length);
            Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex, (byte)target };
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("If not", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }
}
