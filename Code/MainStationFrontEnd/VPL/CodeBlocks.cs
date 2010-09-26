using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace MainStationFrontEnd
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

    public class BlockRemoteEvent : BaseBlockOther
    {
        ProductDataBase.Device device;
        ProductDataBase.Device.RemoteEvent remoteevent;
        ushort deviceid;

        [Browsable(true), CategoryAttribute("Constant")]
        public ushort DeviceID
        {
            get { return deviceid; }
            set { deviceid = value; }
        }

        public BlockRemoteEvent(KismetSequence _Sequence)
            : base(_Sequence, 2)
        {
            width = 100;
            height = 25;
        }

        public override void SetValues(string _Values)
        {
            Inputs.Clear();
            string[] split = _Values.Split(' ');
            device = ProductDataBase.GetDeviceByID(ushort.Parse(split[0]));
            remoteevent = device.GetRemoteEventByID(byte.Parse(split[1]));
            deviceid = ushort.Parse(split[2]);
            foreach (var i in remoteevent.Inputs)
            {
                Inputs.Add(new Input(this, i.Name, GetDataType(i.Type)));
            }
            foreach (var i in remoteevent.Outputs)
            {
                Outputs.Add(new Output(this, i.Name, GetDataType(i.Type)));
            }
            if (Inputs.Count == 0) Inputs.Add(new Input(this, "", null));//add one input if there are non available
            UpdateConnectors();
        }

        public override string GetValues()
        {
            return string.Format("{0} {1} {2}", device.ID, remoteevent.ID, deviceid);
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

    public class BlockSetDebugLed1 : BaseBlockOther
    {
        public BlockSetDebugLed1(KismetSequence _Sequence)
            : base(_Sequence, 20)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this, "On?", GetDataType("bool")));
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
            : base(_Sequence, 21)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this, "On?", GetDataType("bool")));
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

    public class BlockGetTime : BaseBlockVariable
    {
        public BlockGetTime(KismetSequence _Sequence)
            : base(_Sequence, 22)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Time", GetDataType("time")));
            UpdateConnectors();
        }

        public override void Assamble() { Code = new byte[] { BlockID, Outputs[0].RegisterIndex }; }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Time", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockGetHour : BaseBlockVariable
    {
        public BlockGetHour(KismetSequence _Sequence)
            : base(_Sequence, 6)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Hour", GetDataType("int")));
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
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Minute", GetDataType("int")));
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
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Second", GetDataType("int")));
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
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Weekday", GetDataType("int")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A equals B", GetDataType("bool")));

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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A differs from B", GetDataType("bool")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A smaller than B", GetDataType("bool")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A larger than B", GetDataType("bool")));
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
            : base(_Sequence, 11)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Constant", GetDataType("int")));
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

    public class BlockConstantBool : BaseBlockConstant
    {
        bool val;

        [Browsable(true), CategoryAttribute("Constant")]
        public bool Value
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
            val = bool.Parse(_Values);
        }

        public BlockConstantBool(KismetSequence _Sequence)
            : base(_Sequence, 11)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Constant", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            byte[] val = Utilities.FromShort(Value ? (ushort)65535 : (ushort)0);
            Code = new byte[] { BlockID, Outputs[0].RegisterIndex, val[0], val[1] };
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
    }

    public class BlockConstantTime : BaseBlockConstant
    {
        TimeSpan time;
        DayOfWeek weekday;

        [Browsable(true), CategoryAttribute("Constant")]
        public TimeSpan Time { get { return time; } set { time = value; } }

        [Browsable(true), CategoryAttribute("Constant")]
        public DayOfWeek WeekDay { get { return weekday; } set { weekday = value; } }

        public override string GetValues()
        {
            return string.Format("{0} {1} {2} {3}", (byte)time.Hours, (byte)time.Minutes, (byte)time.Seconds, (byte)weekday);
        }

        public override void SetValues(string _Values)
        {
            time = new TimeSpan(
            int.Parse(_Values.Split(' ')[0]),
            int.Parse(_Values.Split(' ')[1]),
            int.Parse(_Values.Split(' ')[2]));
            weekday = (DayOfWeek)int.Parse(_Values.Split(' ')[3]);
        }

        public BlockConstantTime(KismetSequence _Sequence)
            : base(_Sequence, 10)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Constant Time", GetDataType("time")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, Outputs[0].RegisterIndex, 0, (byte)time.Hours, (byte)time.Minutes, (byte)time.Seconds, (byte)((int)weekday) };
            base.Assamble();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(string.Format("{0}\n{1}", time, weekday), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockAdd : BaseBlockMath
    {
        public BlockAdd(KismetSequence _Sequence)
            : base(_Sequence, 10)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A + B", GetDataType("int")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A-B", GetDataType("int")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A*B", GetDataType("int")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A/B", GetDataType("int")));
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
            Inputs.Add(new Input(this, "A", GetDataType("int")));
            Inputs.Add(new Input(this, "B", GetDataType("int")));
            Outputs.Add(new Output(this, "A and B", GetDataType("int")));
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
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Value", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, EEPROM.GetVariableAddress(name), Outputs[0].RegisterIndex };
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
            Inputs.Add(new Input(this, "Value", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = new byte[] { BlockID, EEPROM.GetVariableAddress(name), Inputs[0].Connected.RegisterIndex };
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
            Outputs.Add(new Output(this, "", null));
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
            Inputs.Add(new Input(this, "Condition", GetDataType("bool")));
            Outputs.Add(new Output(this, "", null));
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
            Inputs.Add(new Input(this, "Condition", GetDataType("bool")));
            Outputs.Add(new Output(this, "", null));
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
