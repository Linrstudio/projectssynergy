using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;
using DesktopCodeBlocks;

namespace WebInterface
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
            Name = "Switch toggle";
        }

        public override void Load(XElement _Data) { SwitchName = _Data.Value; }
        public override void Save(XElement _Data) { _Data.Value = SwitchName; }

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input) { Trigger(TriggerOutputs[0]); }
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

        public override void HandleInput(CodeBlock.DataInput _Input, object _Data) { throw new NotImplementedException(); }
        public override void HandleTrigger(TriggerInput _Input)
        {
            Control c = WebInterface.FindControl(switchname);
            if (c != null && c is Switch)
            {
                ((Switch)c).State = (bool)GetInput(DataInputs[0]);
                System.Diagnostics.Debug.WriteLine("Toggled " + switchname);
            }
        }
        public override object HandleOutput(DataOutput _Output) { throw new NotImplementedException(); }
    }

    public class BlockSwitchGetState : BaseBlockInstruction
    {
        [Browsable(true)]
        public string SwitchName
        {
            get { return switchname; }
            set { switchname = value.ToLower(); }
        }
        string switchname = "";

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
            Control c = WebInterface.FindControl(switchname);
            if (c != null && c is Switch)
                return ((Switch)c).State;
            return false;
        }
    }

    public class WebInterfaceCodeBlocks
    {
        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch toggle", "WebInterface", "i like u", typeof(BlockEventSwitchToggle)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch set state", "WebInterface", "i like u", typeof(BlockSwitchSetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch get state", "WebInterface", "i like u", typeof(BlockSwitchGetState)));
        }
    }
}
