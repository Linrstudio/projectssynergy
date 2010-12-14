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
        public BaseBlockConditions(KismetSequence _Sequence) : base(_Sequence) { }
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
        public BaseBlockMath(KismetSequence _Sequence)
            : base(_Sequence)
        {

        }

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

    public class BaseBlockConstant : CodeBlock
    {
        public BaseBlockConstant(KismetSequence _Sequence) : base(_Sequence) { }

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
        public BaseBlockVariable(KismetSequence _Sequence) : base(_Sequence) { }

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
        public BaseBlockOther(KismetSequence _Sequence) : base(_Sequence) { }
    }

    public class BaseBlockBranch : CodeBlock
    {
        public BaseBlockBranch(KismetSequence _Sequence) : base(_Sequence) { }
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
        public BaseBlockEvent(KismetSequence _Sequence) : base(_Sequence) { }
    }

    public class BlockScheduleEvent : BaseBlockEvent
    {
        [Browsable(true), CategoryAttribute("Constant")]
        public TimeSpan time
        {
            get
            {
                if (Sequence is KismetSequenceScheduleEvent)
                {
                    EEPROM.ScheduleEntry entry = ((KismetSequenceScheduleEvent)Sequence).ScheduleEntry;
                    return new TimeSpan(entry.Hours, entry.Minutes, entry.Seconds);
                }
                return new TimeSpan();
            }
            set
            {
                if (Sequence is KismetSequenceScheduleEvent)
                {
                    EEPROM.ScheduleEntry entry = ((KismetSequenceScheduleEvent)Sequence).ScheduleEntry;
                    entry.Hours = value.Hours;
                    entry.Minutes = value.Minutes;
                    entry.Seconds = value.Seconds;
                }
            }
        }

        public BlockScheduleEvent(KismetSequence _Sequence)
            : base(_Sequence)
        {
            IsScope = true;
            width = 120;
            height = 200;
        }

        public override void SetValues(string _Values)
        {
            Inputs.Clear();
            Outputs.Clear();

            if (Outputs.Count == 0)
                Outputs.Add(new Output(this, "", null));//add one output if there are non available
            UpdateConnectors();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawScope(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            if (Sequence is KismetSequenceScheduleEvent)
            {
                _Graphics.DrawString(((KismetSequenceScheduleEvent)Sequence).ScheduleEntry.Name, new Font("Arial", 10), Brushes.Black, X, Y, sf);
            }
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawScopeShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public class BlockLocalEvent : BaseBlockEvent
    {
        public BlockLocalEvent(KismetSequence _Sequence)
            : base(_Sequence)
        {
            IsScope = true;
            width = 120;
            height = 200;
        }

        public override void SetValues(string _Values)
        {
            Inputs.Clear();
            Outputs.Clear();
            if (Sequence is KismetSequenceDeviceEvent)
            {
                EEPROM.Device.Event deviceevent = ((KismetSequenceDeviceEvent)Sequence).DeviceEvent;
                ProductDataBase.Device.Event eventtype = deviceevent.eventtype;
                if (eventtype != null)
                    foreach (var i in eventtype.Outputs)
                    {
                        Outputs.Add(new Output(this, i.Name, GetDataType(i.Type)));
                    }
            }

            if (Outputs.Count == 0)
                Outputs.Add(new Output(this, "", null));//add one output if there are non available
            UpdateConnectors();
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            DrawScope(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            if (Sequence is KismetSequenceDeviceEvent)
            {
                _Graphics.DrawString(((KismetSequenceDeviceEvent)Sequence).DeviceEvent.eventtype.Name, new Font("Arial", 10), Brushes.Black, X, Y, sf);
            }
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawScopeShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public class BlockRemoteEvent : BaseBlockOther
    {
        public ProductDataBase.Device device;
        public ProductDataBase.Device.RemoteEvent remoteevent;
        public ushort deviceid;

        [Browsable(true), CategoryAttribute("Constant")]
        public ushort DeviceID
        {
            get { return deviceid; }
            set { deviceid = value; }
        }

        public BlockRemoteEvent(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
        }

        public override void SetValues(string _Values)
        {
            Inputs.Clear();
            Outputs.Clear();
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
            List<byte> code = new List<byte>();
            //load all inputs to the EP registers
            byte idx = 0;
            code.AddRange(CodeInstructions.Load8((byte)(MainStation.EPBufferOffset + idx), remoteevent.ID));
            idx++;
            foreach (Input i in Inputs)
            {
                code.AddRange(CodeInstructions.Mov(i.Connected.Register.Index, (byte)(MainStation.EPBufferOffset + idx), (byte)i.Connected.Register.Size));
                idx += (byte)i.datatype.RegistersNeeded;
            }
            code.AddRange(CodeInstructions.EPSend(deviceid, idx));

            Code = code.ToArray();
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
            _Graphics.DrawString(string.Format("{0}\n({1})", remoteevent.Name, deviceid), new Font("Arial", 10), Brushes.Black, X, Y, sf);
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
            : base(_Sequence)
        {
            width = 100;
            height = 25;
            Inputs.Add(new Input(this, "On?", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.SetLED(Inputs[0].Connected.Register.Index);
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

    public class BlockGetTime : BaseBlockVariable
    {
        public BlockGetTime(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Time", GetDataType("time")));
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
    }

    public class BlockGetHour : BaseBlockVariable
    {
        public BlockGetHour(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Hour", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.GetHour(Outputs[0].Register.Index);
        }

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
            : base(_Sequence)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Minute", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.GetMinute(Outputs[0].Register.Index);
        }

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
            : base(_Sequence)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Second", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.GetSecond(Outputs[0].Register.Index);
        }

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
            : base(_Sequence)
        {
            width = 75;
            height = 25;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Current Weekday", GetDataType("int")));
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
    }

    public class BlockMathEquals : BaseBlockConditions
    {
        public BlockMathEquals(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = CodeInstructions.Equals(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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

    public class BlockBoolEquals : BaseBlockConditions
    {
        public BlockBoolEquals(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A", GetDataType("bool")));
            Inputs.Add(new Input(this, "B", GetDataType("bool")));
            Outputs.Add(new Output(this, "A equals B", GetDataType("bool")));

            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.Equals(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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

    public class BlockMathDiffers : BaseBlockConditions
    {
        public BlockMathDiffers(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = CodeInstructions.Differs(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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

    public class BlockBoolDiffers : BaseBlockConditions
    {
        public BlockBoolDiffers(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A", GetDataType("bool")));
            Inputs.Add(new Input(this, "B", GetDataType("bool")));
            Outputs.Add(new Output(this, "A differs from B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.Differs(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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

    public class BlockMathSmallerThan : BaseBlockConditions
    {
        public BlockMathSmallerThan(KismetSequence _Sequence)
            : base(_Sequence)
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
    }

    public class BlockMathLargerThan : BaseBlockConditions
    {
        public BlockMathLargerThan(KismetSequence _Sequence)
            : base(_Sequence)
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
    }

    public class BlockBoolAnd : BaseBlockMath
    {
        public BlockBoolAnd(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A", GetDataType("bool")));
            Inputs.Add(new Input(this, "B", GetDataType("bool")));
            Outputs.Add(new Output(this, "A and B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.And(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("And", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockBoolOr : BaseBlockMath
    {
        public BlockBoolOr(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A", GetDataType("bool")));
            Inputs.Add(new Input(this, "B", GetDataType("bool")));
            Outputs.Add(new Output(this, "A or B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.Or(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
        }

        public override void Draw(Graphics _Graphics)
        {
            base.Draw(_Graphics);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Or", new Font("Arial", 10), Brushes.Black, X, Y, sf);
        }
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

        public override string GetValues()
        {
            return val.ToString();
        }

        public override void SetValues(string _Values)
        {
            val = ushort.Parse(_Values);
        }

        public BlockMathConstant(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Constant", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.Load(Outputs[0].Register.Index, Value);
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

    public class BlockBoolConstant : BaseBlockConstant
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

        public BlockBoolConstant(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Constant", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.Load(Outputs[0].Register.Index, (ushort)(Value ? 65535 : 0));
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

    public class BlockTimeConstant : BaseBlockConstant
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

        public BlockTimeConstant(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 50;
            height = 50;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Constant Time", GetDataType("time")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = new byte[] { BlockID, Outputs[0].RegisterIndex, 0, (byte)time.Hours, (byte)time.Minutes, (byte)time.Seconds, (byte)((int)weekday) };
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

    public class BlockMathAdd : BaseBlockMath
    {
        public BlockMathAdd(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = CodeInstructions.Add(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);

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

    public class BlockBoolXOR : BaseBlockMath
    {
        public BlockBoolXOR(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "A", GetDataType("bool")));
            Inputs.Add(new Input(this, "B", GetDataType("bool")));
            Outputs.Add(new Output(this, "A Xor B", GetDataType("bool")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            Code = CodeInstructions.Xor(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);

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
    }
    public class BlockMathSubstract : BaseBlockMath
    {
        public BlockMathSubstract(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = CodeInstructions.Substract(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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
    public class BlockMathMultiply : BaseBlockMath
    {
        public BlockMathMultiply(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = CodeInstructions.Multiply(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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
    }
    public class BlockMathDivide : BaseBlockMath
    {
        public BlockMathDivide(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = CodeInstructions.Divide(Inputs[0].Connected.Register.Index, Inputs[1].Connected.Register.Index, Outputs[0].Register.Index);
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
    }

    public class BlockMathGetVariable : BaseBlockVariable
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

        public BlockMathGetVariable(KismetSequence _Sequence)
            : base(_Sequence)
        {
            height = 50;
            width = 100;
            Inputs.Add(new Input(this, "", null));
            Outputs.Add(new Output(this, "Value", GetDataType("int")));
            UpdateConnectors();
        }

        public override void Assamble()
        {
            //Code = new byte[] { BlockID, EEPROM.GetVariableAddress(name), Outputs[0].RegisterIndex };
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
    public class BlockMathSetVariable : BaseBlockVariable
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

        public BlockMathSetVariable(KismetSequence _Sequence)
            : base(_Sequence)
        {
            width = 100;
            height = 50;
            Inputs.Add(new Input(this, "Value", GetDataType("int")));
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
    }

    public class BlockBoolIf : BaseBlockBranch
    {
        public BlockBoolIf(KismetSequence _Sequence)
            : base(_Sequence)
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
            Code = new byte[] { 0x80, Inputs[0].Connected.Register.Index, (byte)target };
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
}
