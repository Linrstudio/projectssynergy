using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace WebInterface
{
    public class WebInterface
    {
        public static List<WebInterface> WebInterfaces = new List<WebInterface>();

        public ushort Port = 8080;

        HttpListener httplistener;
        Thread httplistenerthread;

        public List<Scene> scenes = new List<Scene>();

        public string boreme()
        {
            return Guid.NewGuid().ToString();
        }

        public static Control FindControl(string _Name)
        {
            foreach (WebInterface i in WebInterfaces)
            {
                Control c = i.GetControl(_Name);
                if (c != null) return c;
            }
            return null;
        }

        public Control GetControl(string _Name)
        {
            foreach (Scene s in scenes)
            {
                foreach (Control c in s.Controls)
                {
                    if (c.Name.ToLower() == _Name.ToLower()) return c;
                }
            }
            return null;
        }

        public WebInterface(XElement _Data)
        {
            Stop();
            Port = ushort.Parse(_Data.Attribute("Port").Value);

            foreach (XElement element in _Data.Elements("Scene"))
            {
                Scene scene = new Scene(element);
                scenes.Add(scene);
            }

            if (!HttpListener.IsSupported) Console.WriteLine("HTTPListener class is not supported on your machine.");
            httplistener = new HttpListener();

            string LocalIP = Dns.GetHostName();
            Console.WriteLine("HostName:" + LocalIP);
            LocalIP = Dns.GetHostByName(LocalIP).AddressList[0].ToString();
            Console.WriteLine("HostIP:" + LocalIP);
            httplistener.Prefixes.Add("http://" + LocalIP + ":" + Port + "/");
            httplistener.Prefixes.Add("http://localhost:" + Port + "/");
            try
            {

                httplistener.Start();
            }
            catch { System.Windows.Forms.MessageBox.Show("Failed to start HTTPListener at port " + Port.ToString()); }

            httplistenerthread = new Thread(new ThreadStart(httplistenermain));
            httplistenerthread.Start();
        }

        public WebInterface()
        {
            Stop();
            if (!HttpListener.IsSupported) Console.WriteLine("HTTPListener class is not supported on your machine.");
            httplistener = new HttpListener();

            string LocalIP = Dns.GetHostName();
            Console.WriteLine("HostName:" + LocalIP);
            LocalIP = Dns.GetHostByName(LocalIP).AddressList[0].ToString();
            Console.WriteLine("HostIP:" + LocalIP);
            httplistener.Prefixes.Add("http://" + LocalIP + ":" + Port + "/");
            httplistener.Prefixes.Add("http://localhost:" + Port + "/");
            try
            {

                httplistener.Start();
            }
            catch { System.Windows.Forms.MessageBox.Show("Failed to start HTTPListener at port " + Port.ToString()); }

            httplistenerthread = new Thread(new ThreadStart(httplistenermain));
            httplistenerthread.Start();
        }

        public void Stop()
        {
            try
            {
                if (httplistener != null && httplistener.IsListening)
                    httplistener.Stop();
            }
            catch { }

            if (httplistenerthread != null)
                httplistenerthread.Abort();

            scenes.Clear();
            httplistener = null;
            httplistenerthread = null;
        }

        public void httplistenermain()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = httplistener.GetContext();
                    //check username/password
                    /*
                    if (context.User != null && context.User.Identity is HttpListenerBasicIdentity)
                    {
                        HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
                        if (identity.Name != "Roeny" && identity.Password != "Baloony") continue;
                    }
                    */

                    byte[] buffer = System.Text.Encoding.ASCII.GetBytes("invalid request!");//response

                    HttpListenerResponse response = context.Response;
                    URLParser parser = new URLParser(context.Request.Url.Query);

                    string scene = parser.Get("scene");
                    string res = parser.Get("res");
                    string command = parser.Get("command");
                    string control = parser.Get("control");
                    int width = 0;
                    try { width = int.Parse(parser.Get("width")); }
                    catch { }
                    int height = 0;
                    try { height = int.Parse(parser.Get("height")); }
                    catch { }

                    if (res != "")
                    {
                        buffer = System.IO.File.ReadAllBytes(res);//FIXME HUGE security issue
                    }
                    else
                    {
                        foreach (Scene s in scenes)
                        {
                            if (s.Name.ToLower() == scene.ToLower())
                            {
                                if (command != "")
                                {
                                    Control ctrl = s.GetControl(control);
                                    if (ctrl is Switch)
                                    {
                                        Switch swit = (Switch)ctrl;
                                        switch (command)
                                        {
                                            case "ToggleState":
                                                swit.Loading = true;
                                                swit.EnqueueCommand(Switch.Command.Toggle);
                                                while (swit.Loading) Thread.Sleep(10);
                                                buffer = System.Text.Encoding.ASCII.GetBytes(swit.State ? "1" : "0");
                                                break;
                                            case "GetState":
                                                buffer = System.Text.Encoding.ASCII.GetBytes(swit.State ? "1" : "0");
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    //send html code for this scene

                                    string rs = System.IO.File.ReadAllText("main.txt");

                                    URLParser bgsrc = new URLParser("");
                                    bgsrc.Parameters.Add("scene", scene);
                                    bgsrc.Parameters.Add("res", "scene1/background.jpg");
                                    //bgsrc.Parameters.Add("boreme", boreme());
                                    rs = rs.Replace("__BGSRC__", bgsrc.ToString());
                                    rs = rs.Replace("__SCENE__", scene.ToString());

                                    if (width == 0 || height == 0)
                                    {
                                        rs = rs.Replace("__WIDTH__", "100%");
                                        rs = rs.Replace("__HEIGHT__", "100%");
                                    }
                                    else
                                    {
                                        rs = rs.Replace("__WIDTH__", width.ToString());
                                        rs = rs.Replace("__HEIGHT__", height.ToString());
                                    }
                                    string controls = "";
                                    string pageonload = "";
                                    foreach (Control c in s.Controls)
                                    {
                                        pageonload += "GetImage('" + c.Name + "');";
                                        if (c is Switch)
                                        {
                                            Switch ctrl = (Switch)c;

                                            // src=\"" + imgstr + "\";
                                            rs += "<img  href=\"javascript:;\" onclick=\"ToggleSwitch('" + ctrl.Name + "');\" id=\"control_" + ctrl.Name + "\" ";
                                            rs += "style=\"position:absolute;";
                                            if (width == 0 || height == 0)
                                            {
                                                rs += "left:" + (ctrl.X - ctrl.Width / 2) * 100 + "%;";
                                                rs += "top:" + (ctrl.Y - ctrl.Height / 2) * 100 + "%;";
                                                rs += "width:" + ctrl.Width * 100 + "%;";
                                                rs += "height:" + ctrl.Height * 100 + "%;";
                                            }
                                            else
                                            {
                                                rs += "left:" + (ctrl.X - ctrl.Width / 2) * width + ";";
                                                rs += "top:" + (ctrl.Y - ctrl.Height / 2) * height + ";";
                                                rs += "width:" + ctrl.Width * width + ";";
                                                rs += "height:" + ctrl.Height * height + ";";
                                            }
                                            rs += "z-index:3; display: block\">";
                                        }
                                    }
                                    rs = rs.Replace("__PAGEONLOAD__", pageonload);
                                    rs = rs.Replace("__CONTROLS__", controls);
                                    rs += "</body>";
                                    rs += "</html>";

                                    buffer = System.Text.Encoding.ASCII.GetBytes(rs);//response
                                }
                            }
                        }
                    }
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
    }
}
