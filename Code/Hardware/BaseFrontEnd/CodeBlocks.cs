using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace BaseFrontEnd
{
    public class SetDebugLed1 : CodeBlock
    {
        public SetDebugLed1(KismetSequence _Sequence)
            : base(_Sequence, 2, 2)
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

    public class SetDebugLed2 : CodeBlock
    {
        public SetDebugLed2(KismetSequence _Sequence)
            : base(_Sequence, 2, 3)
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

    public class GetHour : CodeBlock
    {
        public GetHour(KismetSequence _Sequence)
            : base(_Sequence, 2, 6)
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
    public class GetMinute : CodeBlock
    {
        public GetMinute(KismetSequence _Sequence)
            : base(_Sequence, 2, 7)
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
    public class GetSecond : CodeBlock
    {
        public GetSecond(KismetSequence _Sequence)
            : base(_Sequence, 2, 8)
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
    public class GetDay : CodeBlock
    {
        public GetDay(KismetSequence _Sequence)
            : base(_Sequence, 2, 9)
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

    public class Compare : CodeBlock
    {
        public Compare(KismetSequence _Sequence)
            : base(_Sequence, 4, 5)
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
            _Graphics.DrawString("Compare", new Font("Arial", 10), Brushes.Black, x , y, sf);
            base.Draw(_Graphics);
        }
    }

    public class ConstantByte : CodeBlock
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

        public ConstantByte(KismetSequence _Sequence)
            : base(_Sequence, 3, 1)
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

    public class ConstantWeekDay : CodeBlock
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


        public ConstantWeekDay(KismetSequence _Sequence)
            : base(_Sequence, 3, 14)
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

    public class BlockAdd : CodeBlock
    {
        public BlockAdd(KismetSequence _Sequence)
            : base(_Sequence, 4, 10)
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
            _Graphics.DrawString("Add", new Font("Arial", 10), Brushes.Black, x + width / 2, y + 10, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockSubstract : CodeBlock
    {
        public BlockSubstract(KismetSequence _Sequence)
            : base(_Sequence, 4, 11)
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
            _Graphics.DrawString("Substract", new Font("Arial", 10), Brushes.Black, x + width / 2, y + 10, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockMultiply : CodeBlock
    {
        public BlockMultiply(KismetSequence _Sequence)
            : base(_Sequence, 4, 12)
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
            _Graphics.DrawString("Multiply", new Font("Arial", 10), Brushes.Black, x + width / 2, y + 10, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockDivide : CodeBlock
    {
        public BlockDivide(KismetSequence _Sequence)
            : base(_Sequence, 4, 13)
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
            _Graphics.DrawString("Divide", new Font("Arial", 10), Brushes.Black, x + width / 2, y + 10, sf);
            base.Draw(_Graphics);
        }
    }
    public class BlockBitMask : CodeBlock
    {
        public BlockBitMask(KismetSequence _Sequence)
            : base(_Sequence, 4, 15)
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

    public class BlockGetVariable : CodeBlock
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
            : base(_Sequence, 3, 17)
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
            base.Assamble();
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
    public class BlockSetVariable : CodeBlock
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
            : base(_Sequence, 3, 16)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Sequence.Event.device.eeprom.GetVariableAddress(name), Inputs[0].Connected.RegisterIndex };
            base.Assamble();
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

    public class PushEvent : CodeBlock
    {
        public PushEvent(KismetSequence _Sequence)
            : base(_Sequence, 0, 0)
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
