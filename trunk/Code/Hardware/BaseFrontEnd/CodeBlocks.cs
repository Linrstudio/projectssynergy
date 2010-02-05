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
    }

    public class BaseBlockMath : CodeBlock
    {
        public BaseBlockMath(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
    }

    public class BaseBlockConstant : CodeBlock
    {
        public BaseBlockConstant(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
    }

    public class BaseBlockVariable : CodeBlock
    {
        public BaseBlockVariable(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
    }

    public class BaseBlockOther : CodeBlock
    {
        public BaseBlockOther(KismetSequence _Sequence, byte _BlockID) : base(_Sequence, _BlockID) { }
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
            Inputs.Add(new Input(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("DebugLed1", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }

    public class BlockSetDebugLed2 : BaseBlockOther
    {
        public BlockSetDebugLed2(KismetSequence _Sequence)
            : base(_Sequence, 3)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Inputs[0].Connected.RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("DebugLed2", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }

    public class BlockGetHour : BaseBlockVariable
    {
        public BlockGetHour(KismetSequence _Sequence)
            : base(_Sequence, 6)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Hour", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockGetMinute : BaseBlockVariable
    {
        public BlockGetMinute(KismetSequence _Sequence)
            : base(_Sequence, 7)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Minute", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockGetSecond : BaseBlockVariable
    {
        public BlockGetSecond(KismetSequence _Sequence)
            : base(_Sequence, 8)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Second", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockGetDay : BaseBlockVariable
    {
        public BlockGetDay(KismetSequence _Sequence)
            : base(_Sequence, 9)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Day", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }

    public class BlockEquals : BaseBlockConditions
    {
        public BlockEquals(KismetSequence _Sequence)
            : base(_Sequence, 5)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));

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
            _Graphics.DrawLines(Pens.Black, new Point[]
            {
            new Point(x-50,y-75/2),
            new Point(x+50,y),
            new Point(x-50,y+75/2),
            new Point(x-50,y-75/2)
            });
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Equals", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }

    public class BlockDiffers : BaseBlockConditions
    {
        public BlockDiffers(KismetSequence _Sequence)
            : base(_Sequence, 18)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));

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
            _Graphics.DrawLines(Pens.Black, new Point[]
            {
            new Point(x-50,y-75/2),
            new Point(x+50,y),
            new Point(x-50,y+75/2),
            new Point(x-50,y-75/2)
            });
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Differs", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }

    public class BlockConstantByte : BaseBlockConstant
    {
        byte val;

        [Browsable(true), CategoryAttribute("Constant")]
        public byte Value
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
            val = byte.Parse(_Values);
        }

        public BlockConstantByte(KismetSequence _Sequence)
            : base(_Sequence, 1)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Outputs[0].RegisterIndex, Value };
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawEllipse(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 20, FontStyle.Bold), Brushes.Black, x, y, sf);

            base.Draw(_Graphics);
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
            : base(_Sequence, 14)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Outputs[0].RegisterIndex, (byte)((int)Value) };
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawEllipse(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, x, y, sf);

            base.Draw(_Graphics);
        }
    }

    public class BlockAdd : BaseBlockMath
    {
        public BlockAdd(KismetSequence _Sequence)
            : base(_Sequence, 10)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
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
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Add", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockSubstract : BaseBlockMath
    {
        public BlockSubstract(KismetSequence _Sequence)
            : base(_Sequence, 11)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
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
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Substract", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockMultiply : BaseBlockMath
    {
        public BlockMultiply(KismetSequence _Sequence)
            : base(_Sequence, 12)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
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
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Multiply", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockDivide : BaseBlockMath
    {
        public BlockDivide(KismetSequence _Sequence)
            : base(_Sequence, 13)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
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
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Divide", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockBitMask : BaseBlockMath
    {
        public BlockBitMask(KismetSequence _Sequence)
            : base(_Sequence, 15)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
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
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("BitMask", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
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
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Sequence.Event.device.eeprom.GetVariableAddress(name), Outputs[0].RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Get Variable", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, x, y - 8, sf);
            _Graphics.DrawString(Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, x, y + 8, sf);

            base.Draw(_Graphics);
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
            Inputs.Add(new Input(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Sequence.Event.device.eeprom.GetVariableAddress(name), Inputs[0].Connected.RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x - width / 2, y - height / 2, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Set Variable", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, x, y - 8, sf);
            _Graphics.DrawString(Name, new Font("Arial", 10, FontStyle.Bold), Brushes.Black, x, y + 8, sf);

            base.Draw(_Graphics);
        }
    }

    public class PushEvent : BaseBlockEvent
    {
        public PushEvent(KismetSequence _Sequence)
            : base(_Sequence, 0)
        {
            width = 100;
            height = 200;
            Outputs.Add(new Output(this));
            UpdateConnectors();
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawLines(Pens.Black,
                new Point[]{
            new Point(x    ,y-100),
            new Point(x+50 ,y    ),
            new Point(x    ,y+100),
            new Point(x-50 ,y    ),
            new Point(x    ,y-100)
                });

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("On Push", new Font("Arial", 10), Brushes.Black, x, y, sf);
            base.Draw(_Graphics);
        }
    }
}
