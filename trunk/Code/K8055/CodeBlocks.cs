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
    public class BlockEventSwitchToggle : BaseBlockEvent
    {
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }
        string switchname = "";

        public BlockEventSwitchToggle()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            IsEvent = true;
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }

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
            Name = "Set state";
        }

        [DllImport("K8055D.dll")]
        private static extern void SetDigitalChannel(int Channel);
        [DllImport("K8055D.dll")]
        private static extern void ClearDigitalChannel(int Channel);

        public override void Load(XElement _Data) { outputid = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = outputid.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            K8055.Initialize();
            if ((bool)GetInput(DataInputs[0]))
                SetDigitalChannel(outputid);
            else
                ClearDigitalChannel(outputid);
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

        [DllImport("K8055D.dll")]
        private static extern int ReadDigitalChannel(int Channel);

        public override void Load(XElement _Data) { inputid = int.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = inputid.ToString(); }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public override void HandleTrigger(TriggerInput _Input) { throw new NotImplementedException(); }
        public override object HandleOutput(DataOutput _Output)
        {
            K8055.Initialize();
            ReadDigitalChannel(inputid);
            return false;
        }
    }

    public class K8055
    {
        [DllImport("K8055D.dll")]
        private static extern int OpenDevice(int CardAddress);
        static bool Initialized = false;
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
    }

    public class K8055CodeBlocks
    {
        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalOutput set state", "K8055", "i like u", typeof(BlockDigitalOutputSetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("DigitalInput get state", "K8055", "i like u", typeof(BlockDigitalInputGetState)));
        }
    }
}
