using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace BaseFrontEnd
{
    public class SetDebugLed : CodeBlock
    {
        public SetDebugLed()
            : base(2)
        {
            Inputs.Add(new Input(this));
            width = 100;
            height = 50;
        }

        public override void Assamble()
        {
            Code = new byte[] { 2, Inputs[0].Connected.RegisterIndex };
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("DebugLed", new Font("Arial", 10), Brushes.Black, x + width / 2, y + 10, sf);
            base.Draw(_Graphics);
        }
    }

    public class GetHour : CodeBlock
    {
        public GetHour()
            : base(2)
        {
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            width = 100;
            height = 25;
        }

        public override void Assamble() { Code = new byte[] { 6, Inputs[0].Connected.RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Hour", new Font("Arial", 10), Brushes.Black, x + width / 2, y + height / 2, sf);
            base.Draw(_Graphics);
        }
    }

    public class GetMinute : CodeBlock
    {
        public GetMinute()
            : base(2)
        {
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            width = 100;
            height = 25;
        }

        public override void Assamble() { Code = new byte[] { 7, Inputs[0].Connected.RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Minute", new Font("Arial", 10), Brushes.Black, x + width / 2, y + height / 2, sf);
            base.Draw(_Graphics);
        }
    }

    public class GetSecond : CodeBlock
    {
        public GetSecond()
            : base(2)
        {
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            width = 100;
            height = 25;
        }

        public override void Assamble() { Code = new byte[] { 8, Inputs[0].Connected.RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Second", new Font("Arial", 10), Brushes.Black, x + width / 2, y + height / 2, sf);
            base.Draw(_Graphics);
        }
    }

    public class GetDay : CodeBlock
    {
        public GetDay()
            : base(2)
        {
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));
            width = 100;
            height = 25;
        }

        public override void Assamble() { Code = new byte[] { 9, Inputs[0].Connected.RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawRectangle(Pens.Black, new Rectangle(x, y, width, height));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Day", new Font("Arial", 10), Brushes.Black, x + width / 2, y + height / 2, sf);
            base.Draw(_Graphics);
        }
    }

    public class Compare : CodeBlock
    {
        public Compare()
            : base(4)
        {
            Inputs.Add(new Input(this));
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));

            width = 100;
            height = 75;
        }

        public override void Assamble()
        {
            try
            {
                Code = new byte[] { 5, Inputs[0].Connected.RegisterIndex, Inputs[1].Connected.RegisterIndex, Outputs[0].RegisterIndex };
            }
            catch { }
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawLines(Pens.Black, new Point[]
            {
            new Point(x,y),
            new Point(x+100,y+75/2),
            new Point(x,y+75),
            new Point(x,y)
            });
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Compare", new Font("Arial", 10), Brushes.Black, x + width / 3, y + height / 2, sf);
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

        public ConstantByte()
            : base(3)
        {
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));

            width = 50;
            height = 50;
        }

        public override void Assamble()
        {
            Code[0] = 1;
            Code[1] = Outputs[0].RegisterIndex;
            Code[2] = Value;
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawEllipse(Pens.Black, new Rectangle(x, y, 50, 50));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 20, FontStyle.Bold), Brushes.Black, x + 25, y + 25, sf);

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

        public ConstantWeekDay()
            : base(3)
        {
            Inputs.Add(new Input(this));
            Outputs.Add(new Output(this));

            width = 50;
            height = 50;
        }

        public override void Assamble()
        {
            Code[0] = 1;
            Code[1] = Outputs[0].RegisterIndex;
            Code[2] = (byte)((int)Value);
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawEllipse(Pens.Black, new Rectangle(x, y, 50, 50));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Value.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, x + 25, y + 25, sf);

            base.Draw(_Graphics);
        }
    }

    public class PushEvent : CodeBlock
    {
        public PushEvent()
            : base(0)
        {
            Outputs.Add(new Output(this));

            width = 100;
            height = 200;
        }



        public override void Draw(Graphics _Graphics)
        {
            _Graphics.DrawLines(Pens.Black,
                new Point[]{
            new Point(x + 50, y),
            new Point(x + 100, y+100),
            new Point(x + 50, y+200),
            new Point(x , y+100),
            new Point(x + 50, y)
                });

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("On Push", new Font("Arial", 10), Brushes.Black, x + width / 2, y + height / 2, sf);
            base.Draw(_Graphics);
        }
    }
}
