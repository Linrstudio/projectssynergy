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

namespace K8055
{
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
            IsEvent = true;
            Name = "Input Toggle";
        }

        public override void Load(XElement _Data) { inputid = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = inputid.ToString(); }

        public override void Update()
        {
            bool curstate = K8055.GetInput(inputid);
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

    public class K8055CodeBlocks
    {
        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput set", "K8055", "i like u", typeof(BlockDigitalOutputSetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput get", "K8055", "i like u", typeof(BlockDigitalOutputGetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput toggle", "K8055", "i like u", typeof(BlockDigitalOutputToggleState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalInput get state", "K8055", "i like u", typeof(BlockDigitalInputGetState)));
        }
    }
}
