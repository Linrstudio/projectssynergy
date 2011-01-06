using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergySequence;

namespace DesktopCodeBlocks
{
    public abstract class DesktopCodeBlock : CodeBlock
    {
        public virtual void HandleInput(CodeBlock.DataInput _Input, object _Data) { }
        public abstract object HandleOutput(CodeBlock.DataOutput _Output);
        public abstract void HandleTrigger(CodeBlock.TriggerInput _Input);
        public void Trigger(CodeBlock.TriggerOutput _Output)
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

        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Schedule", "Generic Events", "i like u", typeof(DesktopCodeBlocks.BlockEventSchedule)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Boolean", "i like u", typeof(DesktopCodeBlocks.BlockBoolConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invert", "Boolean", "i like u", typeof(DesktopCodeBlocks.BlockBoolInvert)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Equals", "Boolean", "i like u", typeof(DesktopCodeBlocks.BlockBoolEquals)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Set debug LED", "mygroup", "i like u", typeof(DesktopCodeBlocks.BlockSetDebugLed1)));

            _Manager.AddPrototype(new SequenceManager.Prototype("Multiply", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathMultiply)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Math", "i like u", typeof(DesktopCodeBlocks.BlockMathConstant)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Print", "mygroup", "i like u", typeof(DesktopCodeBlocks.BlockMathPrint)));
        }
    }
}
