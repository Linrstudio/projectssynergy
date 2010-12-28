using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergySequence;

namespace MainStationCodeBlocks
{
    public abstract class MainStationCodeBlock : CodeBlock
    {
        public static void AddAllPrototypes(SynergySequence.SequenceManager _Manager)
        {
            _Manager.AddPrototype(new SequenceManager.Prototype("Schedule", "Generic Events", "i like u", typeof(BlockEventSchedule)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Invert", "Bool", "", typeof(BlockBoolInvert)));
            _Manager.AddPrototype(new SequenceManager.Prototype("Constant", "Bool", "", typeof(BlockBoolConstant)));
        }
    }
}
