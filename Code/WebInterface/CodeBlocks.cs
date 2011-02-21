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
                Control c = WebInterface.FindControl(switchname);
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
                Control c = WebInterface.FindControl(switchname);
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
                Control c = WebInterface.FindControl(switchname);
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
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch toggle state", "WebInterface", "i like u", typeof(BlockSwitchToggle)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch set state", "WebInterface", "i like u", typeof(BlockSwitchSetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch get state", "WebInterface", "i like u", typeof(BlockSwitchGetState)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Switch", "WebInterface", "i like u", typeof(BlockControlSwitch)));
        }
    }
}
