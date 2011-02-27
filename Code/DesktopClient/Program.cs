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
using MainStation;

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

            Application.Idle += new EventHandler(Application_Idle);
            Form1 window = new Form1();
            //window.Show();
            tray = window.notifyIcon1;

            TCPListener listener = new TCPListener(7331);
            Reload();
            while (true)
            {
                Update();
                System.Threading.Thread.Sleep(10);
            }
            //Application.Run(window);
        }

        static void Reload()
        {
            ProductDataBase.Load("Products.xml");
            if (System.IO.File.Exists("project.xml"))
            {
                XElement project = XElement.Load("project.xml");
                SynergySequence.SequenceManager sequencemanager = new ComputerSequenceManager();

                DesktopCodeBlocks.DesktopCodeBlock.AddAllPrototypes(sequencemanager);

                sequencemanager.AddDataType(new SequenceManager.DataType("int", System.Drawing.Color.Blue));
                sequencemanager.AddDataType(new SequenceManager.DataType("bool", System.Drawing.Color.Green));

                DesktopClient.sequence = new DesktopSequence();
                DesktopClient.sequence.Manager = sequencemanager;
                DesktopClient.sequence.Load(project.Element("Sequence"));
                DesktopClient.MainStation = null;
                if (project.Element("MainStation") != null)
                {
                    DesktopClient.MainStation = new MainStation.MainStation();
                    DesktopClient.MainStation.Load(project.Element("MainStation"));
                    MainStation.MainStation.Connect();
                    byte[] data = MainStation.MainStationCompiler.Compile(DesktopClient.MainStation);

                    if (MainStation.MainStation.EEPROMWriteVerify(data))
                        tray.ShowBalloonTip(1000, "", "MainStation updated", ToolTipIcon.None);
                    else
                        tray.ShowBalloonTip(1000, "", "Failed to update mainstation", ToolTipIcon.None);
                }

                foreach (WebInterface.WebInterface i in WebInterface.WebInterface.WebInterfaces)
                {
                    i.Stop();
                }

                WebInterface.WebInterface.WebInterfaces.Clear();
                foreach (XElement element in project.Elements("WebInterface"))
                {
                    WebInterface.WebInterface wi = new WebInterface.WebInterface(element);
                    wi.Start();
                    WebInterface.WebInterface.WebInterfaces.Add(wi);
                }
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
                    foreach (CodeBlock b in DesktopClient.sequence.CodeBlocks)
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
            Update();
        }
        static void Update()
        {
            bool didanything = true;
            while (didanything)
            {
                didanything = false;
                if (DesktopClient.sequence != null)
                {
                    foreach (CodeBlock c in DesktopClient.sequence.CodeBlocks) ((DesktopCodeBlock)c).Update();

                    //take one event and execute it
                    if (DesktopClient.sequence.EventsPending() > 0)
                    {
                        DesktopClient.sequence.InvokeFirstEvent();
                        didanything = true;
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
                }
                //do mainstation stuff

                if (DesktopClient.MainStation != null)
                {
                    //bool found = MainStation.MainStation.Connected();
                    bool found = true;
                    if (!found) MainStation.MainStation.Connect();
                    if (found != DesktopClient.MainStation.Found)
                    {
                        DesktopClient.MainStation.Found = found;
                        foreach (TCPListener listener in TCPListener.Listeners)
                        {
                            foreach (TCPConnection c in listener.Connections)
                            {
                                if (DesktopClient.MainStation.Found)
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
                        if (c == null) { continue; }//no sure why this can happen
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
                            didanything = true;
                        }
                    }
                }
            }
        }
    }
}
