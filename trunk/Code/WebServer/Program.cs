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
        public static Scene scene = new Scene();

        public static Size Resolution = new Size(240, 320);

        public static Image backgroundimage;
        public static Image On;
        public static Image Off;

        public static HttpListener listener;
        static void Main(string[] args)
        {
            if (!HttpListener.IsSupported) Console.WriteLine("HTTPListener class is not supported on your machine.");

            scene.BackgroundImage = Image.FromFile("background.png");
            scene.DefaultOnImage = Image.FromFile("On.png");
            scene.DefaultOffImage = Image.FromFile("Off.png");


            scene.ClickablePoints.Add(new Scene.ClickableRegion(40.0f / 320, 40.0f / 480, 0.2f, "Button1"));
            scene.ClickablePoints.Add(new Scene.ClickableRegion(200.0f / 320, 300.0f / 480, 0.2f, "Button2"));

            scene.ClickablePoints.Add(new Scene.ClickableRegion(160.0f / 320, 120.0f / 480, 0.2f, "Button3"));
            scene.ClickablePoints.Add(new Scene.ClickableRegion(200.0f / 320, 200.0f / 480, 0.2f, "Button4"));


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

        static void listen()
        {
            while (true)
            {
                try
                {
                    // Note: The GetContext method blocks while waiting for a request. 
                    HttpListenerContext context = listener.GetContext();
                    //check username/password
                    if (context.User != null && context.User.Identity is HttpListenerBasicIdentity)
                    {
                        HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
                        if (identity.Name != "Roeny" && identity.Password != "Baloony") continue;
                    }

                    HttpListenerRequest request = context.Request;
                    string parameters = request.RawUrl.Replace("?", "");
                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;
                    //handle button press
                    foreach (Scene.ClickableRegion p in scene.ClickablePoints)
                    {
                        if (parameters == "/" + p.Target)
                        {
                            p.Pressed = !p.Pressed;
                            Console.WriteLine(p.Target + " Pressed");
                        }
                    }

                    string ResponseString = scene.GetHTML(new Scene.Settings());
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(ResponseString);

                    if (parameters.Contains("background"))
                    {
                        Image bitmap = scene.GetImage(new Scene.Settings());
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        buffer = stream.ToArray();
                    }
                    // Get a response stream and write the response to it.
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
