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
        static MainStation.MainStation mainstation = null;
        static DesktopSequence sequence;
        static NotifyIcon tray;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");//IMPORTANT

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TCPListener listener = new TCPListener(1000);

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

            if (project.Element("MainStation") != null)
            {
                mainstation = new MainStation.MainStation();
                mainstation.Load(project.Element("MainStation"));
                MainStation.MainStation.Connect();
                byte[] data = MainStation.MainStationCompiler.Compile(mainstation);
                tray.ShowBalloonTip(1000, "", "Writing to mainstation...", ToolTipIcon.None);
                MainStation.MainStation.EEPROMWriteVerify(data);
            }

            foreach (WebInterface.WebInterface i in WebInterface.WebInterface.WebInterfaces)
            {
                i.Stop();
            }

            WebInterface.WebInterface.WebInterfaces.Clear();
            foreach (XElement element in project.Elements("WebInterface"))
            {
                WebInterface.WebInterface.WebInterfaces.Add(new WebInterface.WebInterface(element));
            }
        }

        static void UpdateControl(WebInterface.Control _Control)
        {
            if (_Control is WebInterface.Switch)
            {
                WebInterface.Switch sw = (WebInterface.Switch)_Control;
                while (sw.CommandsPending())
                {
                    WebInterface.Switch.Command command = sw.DequeueCommand();
                    foreach (CodeBlock b in sequence.CodeBlocks)
                    {
                        if (b is BlockEventSwitchToggle && ((BlockEventSwitchToggle)b).SwitchName.ToLower() == _Control.Name.ToLower())
                        {
                            new DesktopSequence.Event((DesktopCodeBlock)b).Invoke();
                        }
                    }
                    //FIXME no event found for pressed button
                }
                sw.Loading = false;
            }
        }

        static void Application_Idle(object sender, EventArgs e)
        {
            foreach (CodeBlock c in sequence.CodeBlocks) ((DesktopCodeBlock)c).Update();

            //take one event and execute it
            if (sequence.EventsPending() > 0)
            {
                sequence.InvokeFirstEvent();
            }
            foreach (WebInterface.WebInterface iface in WebInterface.WebInterface.WebInterfaces)
            {
                foreach (WebInterface.Scene scene in iface.scenes)
                {
                    foreach (WebInterface.Control c in scene.Controls)
                    {
                        UpdateControl(c);
                    }
                }
            }
            //do mainstation stuff
            if (mainstation != null)
            {
                bool found = MainStation.MainStation.Connected();
                if (!found) MainStation.MainStation.Connect();
                if (found != mainstation.Found)
                {
                    mainstation.Found = found;
                    foreach (TCPListener listener in TCPListener.Listeners)
                    {
                        foreach (TCPConnection c in listener.Connections)
                        {
                            if (mainstation.Found)
                                c.Write("mainstation found");
                            else
                                c.Write("mainstation notfound");
                        }
                    }
                }
            }

            //do network stuff
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
                            case "upload"://uploads firmware to mainstation
                                //int mainstationindex = int.Parse(read.Split[1]);
                                //FIXME
                                break;
                        }
                    }
                }
            }
        }
    }
}
