using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Net;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;

namespace WebServer
{
    class Program
    {
        public static Size Resolution = new Size(240, 320);
        public static HttpListener listener;
        static void Main(string[] args)
        {
            if (!HttpListener.IsSupported) Console.WriteLine("HTTPListener class is not supported on your machine.");

            Scene scene1 = new Scene("portrait");
            scene1.ClickablePoints.Add(new Scene.ClickableRegion(280.0f / 320, 40.0f / 480, 0.2f, "Solder"));
            scene1.ClickablePoints.Add(new Scene.ClickableRegion(40.0f / 320, 40.0f / 480, 0.2f, "Lamp"));
            scene1.ClickablePoints.Add(new Scene.ClickableRegion(40.0f / 320, 120.0f / 480, 0.1f, "Fan"));

            scene1.ClickablePoints.Add(new Scene.ClickableRegion(280.0f / 320, 350.0f / 480, 0.2f, "Charger"));
            scene1.ClickablePoints.Add(new Scene.ClickableRegion(280.0f / 320, 450.0f / 480, 0.2f, "Kitchen"));

            scene1.Background = Image.FromStream(new System.IO.FileStream("background.png", System.IO.FileMode.Open));

            Scene.Scenes.Add(scene1);

            //Scene.Scenes.Add(new Scene("landscape", scene1));

            // Create a listener.
            listener = new HttpListener();
            //listener.AuthenticationSchemes = AuthenticationSchemes.Basic;//might be phun

            string LocalIP = Dns.GetHostName();
            Console.WriteLine("HostName:" + LocalIP);
            LocalIP = Dns.GetHostByName(LocalIP).AddressList[0].ToString();
            Console.WriteLine("HostIP:" + LocalIP);
            listener.Prefixes.Add("http://" + LocalIP + ":80/");

            listener.Start();
            Console.WriteLine("Listening...");
            Thread thread = new Thread(new ThreadStart(listen));
            thread.Start();

            K8055.Initialize(0);

            bool doorbell = false;
            DateTime doorbellexpire = DateTime.Now;
            while (true)
            {
                if (K8055.GetDigital(0))
                {
                    if (!doorbell)
                    {
                        doorbellexpire = DateTime.Now + new TimeSpan(0, 0, 0, 5);
                        doorbell = true;
                    }
                }
                if (doorbell)
                {
                    K8055.SetDigital(4, true);
                    if (doorbellexpire < DateTime.Now)
                        doorbell = false;
                }
                foreach (Scene scene in Scene.Scenes)
                {
                    foreach (Scene.ClickableRegion region in scene.ClickablePoints)
                    {
                        if (region.Target.ToLower() == "lamp")
                        {
                            K8055.SetDigital(2, region.Pressed || doorbell);
                        }
                        if (region.Target.ToLower() == "kitchen")
                        {
                            K8055.SetDigital(3, region.Pressed || doorbell);
                        }
                        if (region.Target.ToLower() == "fan")
                        {
                            K8055.SetDigital(4, region.Pressed || doorbell);
                        }
                        if (region.Target.ToLower() == "charger")
                        {
                            K8055.SetDigital(0, region.Pressed);
                        }
                        if (region.Target.ToLower() == "solder")
                        {
                            K8055.SetDigital(1, region.Pressed);
                        }
                    }
                }


                Thread.Sleep(50);
            }
            listener.Stop();
        }

        public static Interface1 interface1 = new Interface1();
        public static Interface2 interface2 = new Interface2();
        public static Interface3 interface3 = new Interface3();

        static void listen()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    //check username/password
                    if (context.User != null && context.User.Identity is HttpListenerBasicIdentity)
                    {
                        HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
                        if (identity.Name != "Roeny" && identity.Password != "Baloony") continue;
                    }
                    HttpListenerResponse response = context.Response;
                    URLParser parser = new URLParser(context.Request.Url.Query);
                    byte[] buffer;
                    string iface = parser.Get("interface");
                    if (iface == interface1.Name)
                    {
                        buffer = interface1.HandleCommand(parser);
                    }
                    else if (iface == interface2.Name)
                    {
                        buffer = interface2.HandleCommand(parser);
                    }
                    else if (iface == interface3.Name)
                    {
                        buffer = interface3.HandleCommand(parser);
                    }
                    else
                    {//default
                        buffer = interface2.HandleCommand(parser);
                    }

                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
    }
}
