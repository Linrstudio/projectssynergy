using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using SynergySequence;
using DesktopCodeBlocks;
using WebInterface;
using LazyNetworking;

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
        static DesktopSequence sequence;
        static NotifyIcon tray;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TCPListener listener = new TCPListener("127.0.0.1", 1000);

            Reload();

            Application.Idle += new EventHandler(Application_Idle);
            Form1 window = new Form1();
            window.Show();
            tray = window.notifyIcon1;
            Application.Run(window);
        }

        static void Reload()
        {
            XElement project = XElement.Load("project.xml");
            SynergySequence.SequenceManager sequencemanager = new ComputerSequenceManager();

            DesktopCodeBlocks.DesktopCodeBlock.AddAllPrototypes(sequencemanager);
            WebInterface.WebInterfaceCodeBlocks.AddAllPrototypes(sequencemanager);
            K8055.K8055CodeBlocks.AddAllPrototypes(sequencemanager);

            sequencemanager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
            sequencemanager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

            sequence = new DesktopSequence();
            sequence.Manager = sequencemanager;
            sequence.Load(project.Element("Sequence"));

            XElement element = project.Element("WebInterface");
            if (element != null)
                WebInterface.WebInterface.Init(element);
            else WebInterface.WebInterface.Stop();
        }

        static void Application_Idle(object sender, EventArgs e)
        {
            foreach (CodeBlock c in sequence.CodeBlocks) ((DesktopCodeBlock)c).Update();

            //take one event and execute it
            if (sequence.EventsPending() > 0)
            {
                sequence.InvokeFirstEvent();
            }

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
                                {
                                    new DesktopSequence.Event((DesktopCodeBlock)b).Invoke();
                                }
                            }
                            //FIXME no event found for pressed button
                        }
                        sw.Loading = false;
                    }
                }
            }
            foreach (TCPListener listener in TCPListener.Listeners)
            {
                foreach (TCPConnection c in listener.Connections)
                {
                    string read = c.Read();
                    if (read != null)//do something with packet
                    {
                        switch (read.Split(' ')[0])
                        {
                            case "project":
                                System.IO.File.WriteAllText("project.xml", read.Remove(0, 8));//remove the word project
                                Reload();
                                break;

                        }
                    }
                }
            }
        }
    }
}
