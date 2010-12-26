using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;
using DesktopCodeBlocks;
using WebInterface;

namespace DesktopClient
{
    public class ComputerSequenceManager : SequenceManager
    {
        public override CodeBlock CreateCodeBlock(Prototype _Prototype)
        {
            return (CodeBlock)Activator.CreateInstance(_Prototype.BlockType);
        }
    }

    static class Program
    {
        static Sequence sequence;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SynergySequence.SequenceManager sequencemanager = new ComputerSequenceManager();
            DesktopCodeBlocks.DesktopCodeBlock.AddAllPrototypes(sequencemanager);
            WebInterface.WebInterfaceCodeBlocks.AddAllPrototypes(sequencemanager);
            sequencemanager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            sequencemanager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            sequence = new Sequence();
            sequence.Manager = sequencemanager;
            sequence.Load(XElement.Load("c:/sequence.xml"));

            WebInterface.Scene scene1 = new WebInterface.Scene("scene1");
            scene1.Controls.Add(new WebInterface.Switch("MySwitch1", 0.1f, 0.1f, 0.2f, 0.2f));
            scene1.Controls.Add(new WebInterface.Switch("MySwitch2", 0.5f, 0.5f, 0.3f, 0.3f));

            WebInterface.Scene scene2 = new WebInterface.Scene("scene2");
            scene2.Controls.Add(new WebInterface.Switch("MySwitch1", 0.4f, 0.7f, 0.2f, 0.2f));
            scene2.Controls.Add(new WebInterface.Switch("MySwitch2", 0.2f, 0.3f, 0.3f, 0.3f));

            WebInterface.WebInterface.scenes.Add(scene1);
            WebInterface.WebInterface.scenes.Add(scene2);
            WebInterface.WebInterface.Init();

            Application.Idle += new EventHandler(Application_Idle);
            Form window = new Form1();
            window.Show();
            Application.Run(window);
        }

        static void Application_Idle(object sender, EventArgs e)
        {
            foreach (WebInterface.Scene scene in WebInterface.WebInterface.scenes)
            {
                foreach (WebInterface.Control c in scene.Controls)
                {
                    if (c is WebInterface.Switch)
                    {
                        WebInterface.Switch sw = (WebInterface.Switch)c;
                        while (sw.CommandsPending())
                        {
                            WebInterface.Switch.Command command = sw.DequeueCommand();
                            foreach (CodeBlock b in sequence.CodeBlocks)
                            {
                                if (b is BlockEventSwitchToggle && ((BlockEventSwitchToggle)b).SwitchName.ToLower() == c.Name.ToLower())
                                    ((DesktopCodeBlock)b).Trigger(b.TriggerOutputs[0]);
                            }
                        }
                    }
                }
            }
        }
    }
}
