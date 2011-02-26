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
using System.Speech;
using WebInterface;

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using SynergySequence;
using DesktopCodeBlocks;

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

    public class BlockGenericEventInvoke : BaseBlockInstruction
    {
        string name = "";
        [Browsable(true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public BlockGenericEventInvoke()
        {
            width = 100;
            height = 50;
            TriggerInputs.Add(new TriggerInput(this, "Trigger"));
            UpdateConnectors();
            //TriggerInputs[0].X += width / 3;
        }
        public override void Load(XElement _Data) { name= _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = name; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            if (DesktopClient.DesktopClient.MainStation == null) return;
            byte eventid = 0;
            ushort deviceid = 0;
            foreach (CodeBlock c in DesktopClient.DesktopClient.MainStation.Sequence.CodeBlocks)
            {
                if (c is MainStationCodeBlocks.BlockGenericEvent)
                {
                    eventid = ((MainStationCodeBlocks.BlockGenericEvent)c).GetEventID();

                }
            }
            if (eventid != 0)
            {
                MainStation.MainStation.InvokeLocalEvent(deviceid, eventid, 0);
            }
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF[]{
                    new PointF(-width/2,height/2),
                    new PointF(-width/3,0),
                    new PointF(-width/2,-height/2),
                    new PointF(width/3,-height/2),
                    new PointF(width/2,0),
                    new PointF(width/3,height/2),
                }
            );
        }
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

    public class BlockSpeak : BaseBlockInstruction
    {
        string text = "";
        [Browsable(true), CategoryAttribute("Constant")]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public BlockSpeak()
        {
            width = 100;
            height = 50;
            TriggerInputs.Add(new TriggerInput(this, ""));
            UpdateConnectors();
        }

        public override void Save(XElement _Data) { _Data.Value = text; }
        public override void Load(XElement _Data) { text = _Data.Value; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            var sam = new System.Speech.Synthesis.SpeechSynthesizer();
            sam.SpeakAsync(text);
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockEventDelay : BaseBlockEvent
    {
        bool Running = false;
        float val;
        [Browsable(true), CategoryAttribute("Delay")]
        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public DateTime invokedate;
        public BlockEventDelay()
        {
            width = 100;
            height = 50;
            DataOutputs.Add(new DataOutput(this, "Running ?", "bool"));
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            TriggerInputs.Add(new TriggerInput(this, "Start"));
            TriggerInputs.Add(new TriggerInput(this, "Abort"));
            UpdateConnectors();
        }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }
        public override void Load(XElement _Data) { val = float.Parse(_Data.Value); }

        public override void Trigger(TriggerOutput _Output)
        {
            Running = false;
            base.Trigger(_Output);
        }

        public override void Update()
        {
            if (Running && DateTime.Now > invokedate)
            {
                ((DesktopSequence)Sequence).AddEvent(new DesktopSequence.Event(this));
            }
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            if (_Input == TriggerInputs[0])
            {
                invokedate = DateTime.Now.AddSeconds(val);
                Running = true;
            }
            if (_Input == TriggerInputs[1])
            {
                Running = false;
            }
        }

        public override object HandleOutput(DataOutput _Output)
        {
            return Running;
        }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Delay " + Value.ToString() + "s", new Font("Arial", 15, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }
    public class BlockEventSwitchToggle : BaseBlockEvent
    {
        string switchname = "";
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }

        public BlockEventSwitchToggle()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            Name = "Switch toggle";
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input) { Trigger(TriggerOutputs[0]); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockSwitchToggle : BaseBlockInstruction
    {
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }
        string switchname = "";


        public BlockSwitchToggle()
        {
            width = 100;
            height = 40;
            TriggerInputs.Add(new TriggerInput(this, "Toggle"));
            UpdateConnectors();
            Name = "Switch";
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }
        bool toggle = false;
        public override void Finalize()
        {
            if (toggle)
            {
                Control c = WebInterface.WebInterface.FindControl(switchname);
                if (c != null && c is Switch)
                {
                    ((Switch)c).State = !((Switch)c).State;
                }
            }
            toggle = false;
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            toggle = true;
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockControlSwitch : BaseBlockInstruction
    {
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }
        string switchname = "";

        public BlockControlSwitch()
        {
            width = 100;
            height = 100;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            TriggerInputs.Add(new TriggerInput(this, "Toggle"));
            UpdateConnectors();
            Name = "Switch";
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            if (_Input == null)
                Trigger(TriggerOutputs[0]);
            if (_Input == TriggerInputs[0])
            {
                Control c = WebInterface.WebInterface.FindControl(switchname);
                if (c != null && c is Switch)
                {
                    ((Switch)c).State = !((Switch)c).State;
                }
            }
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockSwitchSetState : BaseBlockInstruction
    {
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }
        string switchname = "";

        public BlockSwitchSetState()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, ""));
            DataInputs.Add(new DataInput(this, "State", "bool"));
            UpdateConnectors();
            Name = "Set state";
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }

        bool update = false;
        bool targetstate;
        public override void Finalize()
        {
            if (update)
            {
                Control c = WebInterface.WebInterface.FindControl(switchname);
                if (c != null && c is Switch)
                {
                    ((Switch)c).State = targetstate;
                }
            }
            update = false;
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            update = true;
            targetstate = (bool)GetInput(DataInputs[0]);
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockSwitchGetState : BaseBlockInstruction
    {
        string switchname = "";
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }


        public BlockSwitchGetState()
        {
            width = 100;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "State", "bool"));
            UpdateConnectors();
            Name = "Get state";
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            Control c = WebInterface.WebInterface.FindControl(switchname);
            if (c != null && c is Switch)
                return ((Switch)c).State;
            return false;
        }
    }
    public class BlockEventInputToggle : BaseBlockEvent
    {
        [Browsable(true)]
        public int InputID
        {
            get { return inputid; }
            set { inputid = Math.Max(1, Math.Min(5, value)); }
        }
        int inputid = 1;

        bool laststate = false;

        public BlockEventInputToggle()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            Name = "Input Toggle";
        }

        public override void Load(XElement _Data) { inputid = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = inputid.ToString(); }

        public override void Update()
        {
            K8055.Initialize();//FIXME might not be smart to initialize this often
            bool curstate = K8055.GetInput(inputid - 1);
            if (laststate != curstate)
            {
                ((DesktopSequence)Sequence).AddEvent(new DesktopSequence.Event(this));
                laststate = curstate;
            }
        }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input) { Trigger(TriggerOutputs[0]); }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockDigitalOutputSetState : BaseBlockInstruction
    {
        [Browsable(true)]
        public int OutputID
        {
            get { return outputid; }
            set { outputid = Math.Max(1, Math.Min(8, value)); }
        }
        int outputid = 1;

        public BlockDigitalOutputSetState()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, ""));
            DataInputs.Add(new DataInput(this, "State", "bool"));
            UpdateConnectors();
            Name = "K8055 set output";
        }

        public override void Load(XElement _Data) { outputid = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = outputid.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            K8055.Initialize();
            K8055.SetOutput(outputid - 1, (bool)GetInput(DataInputs[0]));
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockDigitalOutputGetState : BaseBlockInstruction
    {
        [Browsable(true)]
        public int Channel
        {
            get { return channel; }
            set { channel = Math.Max(1, Math.Min(8, value)); }
        }
        int channel = 1;

        public BlockDigitalOutputGetState()
        {
            width = 100;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "State", "bool"));
            UpdateConnectors();
            Name = "k8055 get output";
        }

        public override void Load(XElement _Data) { channel = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = channel.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output) { return K8055.GetOutput(channel - 1); }
    }

    public class BlockDigitalOutputToggleState : BaseBlockInstruction
    {
        [Browsable(true)]
        public int Channel
        {
            get { return channel; }
            set { channel = Math.Max(1, Math.Min(8, value)); }
        }
        int channel = 1;

        public BlockDigitalOutputToggleState()
        {
            width = 100;
            height = 25;
            TriggerInputs.Add(new TriggerInput(this, ""));
            UpdateConnectors();
            Name = "k8055 toggle output";
        }

        public override void Load(XElement _Data) { channel = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = channel.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            K8055.Initialize();
            K8055.ToggleOutput(channel - 1);
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockDigitalInputGetState : BaseBlockInstruction
    {
        [Browsable(true)]
        public int InputID
        {
            get { return inputid; }
            set { inputid = Math.Max(1, Math.Min(5, value)); }
        }
        int inputid = 1;

        public BlockDigitalInputGetState()
        {
            width = 100;
            height = 25;
            DataOutputs.Add(new DataOutput(this, "State", "bool"));
            UpdateConnectors();
            Name = "Get state";
        }

        public override void Load(XElement _Data) { inputid = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = inputid.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            K8055.Initialize();
            return K8055.GetInput(inputid - 1);
        }
    }

    public class K8055
    {
        [DllImport("K8055D.dll")]
        private static extern int OpenDevice(int CardAddress);
        [DllImport("K8055D.dll")]
        private static extern void SetDigitalChannel(int Channel);
        [DllImport("K8055D.dll")]
        private static extern void ClearDigitalChannel(int Channel);
        [DllImport("K8055D.dll")]
        private static extern int ReadDigitalChannel(int Channel);


        static bool Initialized = false;
        static bool[] OutputStates = new bool[8];
        public static void Initialize()
        {
            if (!Initialized)
            {
                if (OpenDevice(0) == 0)//success
                {
                    Initialized = true;
                }
            }
        }
        public static void ToggleOutput(int _Idx)
        {
            SetOutput(_Idx, !OutputStates[_Idx]);
        }
        public static void SetOutput(int _Idx, bool _State)
        {
            OutputStates[_Idx] = _State;
            if (OutputStates[_Idx])
                SetDigitalChannel(_Idx + 1);
            else
                ClearDigitalChannel(_Idx + 1);
        }
        public static bool GetInput(int _Idx)
        {
            return ReadDigitalChannel(_Idx + 1) != 0;
        }
        public static bool GetOutput(int _Idx)
        {
            return OutputStates[_Idx];
        }
    }


    public abstract class DesktopCodeBlock : CodeBlock
    {
        public virtual void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public abstract object HandleOutput(CodeBlock.DataOutput _Output);
        public abstract void HandleTrigger(CodeBlock.TriggerInput _Input);
        public virtual void Trigger(CodeBlock.TriggerOutput _Output)
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

        public virtual void Finalize() { }

        /// <summary>
        /// gives the codeblock a chance to for example invoke a event himself ( a schedule event ? )
        /// </summary>
        public virtual void Update() { }

        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Invoke Event", "Generic", "im in like with u", typeof(BlockGenericEventInvoke)));
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

            _Manager.AddPrototype(new SequenceManager.Prototype("Speak", "Generic", "i like u", typeof(DesktopCodeBlocks.BlockSpeak)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Switch toggle", "WebInterface", "i like u", typeof(BlockEventSwitchToggle)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch toggle state", "WebInterface", "i like u", typeof(BlockSwitchToggle)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch set state", "WebInterface", "i like u", typeof(BlockSwitchSetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch get state", "WebInterface", "i like u", typeof(BlockSwitchGetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch", "WebInterface", "i like u", typeof(BlockControlSwitch)));

            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput set", "K8055", "i like u", typeof(BlockDigitalOutputSetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput get", "K8055", "i like u", typeof(BlockDigitalOutputGetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput toggle", "K8055", "i like u", typeof(BlockDigitalOutputToggleState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalInput get state", "K8055", "i like u", typeof(BlockDigitalInputGetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalInputToggled", "K8055", "i like u", typeof(BlockEventInputToggle)));

        }
    }
}
