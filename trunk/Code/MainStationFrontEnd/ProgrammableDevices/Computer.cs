using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;
using DesktopCodeBlocks;
using WebInterface;

namespace MainStationFrontEnd
{
    public class ComputerSequenceManager : SequenceManager
    {
        public override CodeBlock CreateCodeBlock(Prototype _Prototype)
        {
            return (CodeBlock)Activator.CreateInstance(_Prototype.BlockType);
        }
    }

    public class Computer : ProgrammableDevice
    {
        public Computer()
            : base()
        {
            Manager = new ComputerSequenceManager();
            DesktopCodeBlock.AddAllPrototypes(Manager);
            WebInterfaceCodeBlocks.AddAllPrototypes(Manager);

            Manager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            Manager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            Sequence = new Sequence(Manager);
        }

        public override void Load(System.Xml.Linq.XElement _Data)
        {
            throw new NotImplementedException();
        }

        public override System.Xml.Linq.XElement Save()
        {
            throw new NotImplementedException();
        }

        public void Compile()
        {
            Sequence.Save().Save("c:/sequence.xml");
        }
    }
}
