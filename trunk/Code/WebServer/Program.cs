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

            scene1.ClickablePoints.Add(new Scene.ClickableRegion(280.0f / 320, 350.0f / 480, 0.2f, "Charger"));
            scene1.ClickablePoints.Add(new Scene.ClickableRegion(280.0f / 320, 450.0f / 480, 0.2f, "Kitchen"));

            scene1.Background = Image.FromStream(new System.IO.FileStream("background.png",System.IO.FileMode.Open));

            Scene.Scenes.Add(scene1);

            Scene.Scenes.Add(new Scene("landscape", scene1));

            // Create a listener.
            listener = new HttpListener();
            //listener.AuthenticationSchemes = AuthenticationSchemes.Basic;//might be phun

            string LocalIP = Dns.GetHostName();
            Console.WriteLine("HostName:" + LocalIP);
            LocalIP = Dns.GetHostByName(LocalIP).AddressList[0].ToString();
            Console.WriteLine("HostIP:" + LocalIP);
            listener.Prefixes.Add("http://" + LocalIP + ":1000/");

            listener.Start();
            Console.WriteLine("Listening...");
            Thread thread = new Thread(new ThreadStart(listen));
            thread.Start();
            while (true)
            {
                Thread.Sleep(1000);
            }
            listener.Stop();
        }

        public static Interface1 interface1 = new Interface1();
        public static Interface2 interface2 = new Interface2();

        static void listen()
        {
            while (true)
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
                else// if (iface == interface2.Name)
                {
                    buffer = interface2.HandleCommand(parser);
                }

                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
        }
    }
}
