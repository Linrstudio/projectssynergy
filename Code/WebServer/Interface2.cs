using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;

namespace WebServer
{
    public class Interface2 : WebInterface
    {
        public Interface2() : base("2") { }
        public override byte[] HandleCommand(URLParser _Parameters)
        {
            string command = _Parameters.Get("command");
            string resource = _Parameters.Get("res");

            Settings settings = new Settings();
            //find scene
            settings.SceneName = _Parameters.Get("scene");
            Scene scene = Scene.GetScene(settings.SceneName);


            try
            {
                settings.Width = int.Parse(_Parameters.Get("width"));
            }
            catch { }
            try
            {
                settings.Height = int.Parse(_Parameters.Get("height"));
            }
            catch { }
            try
            {
                settings.Detail = int.Parse(_Parameters.Get("mip"));
            }
            catch { }

            if (command.ToLower() != null)
            {
                foreach (Scene.ClickableRegion reg in scene.ClickablePoints)
                {
                    if (reg.Target == command)
                    {
                        reg.Pressed = !reg.Pressed;
                        Console.WriteLine("{0} -> {1}", reg.Target, reg.Pressed);
                        return new byte[] { (byte)(reg.Pressed ? '1' : '0') };
                    }
                }
            }
            switch (resource)
            {
                case "background":
                    return scene.GetImageResized(settings.Width / settings.Detail, settings.Height / settings.Detail);
                case "lampon":
                    return System.IO.File.ReadAllBytes("on.png");
                case "lampoff":
                    return System.IO.File.ReadAllBytes("off.png");
                case "lamploading":
                    return System.IO.File.ReadAllBytes("loading.gif");
                default:
                    string Areas = "";
                    foreach (Scene.ClickableRegion p in scene.ClickablePoints)
                    {
                        Areas += string.Format("<img href=\"javascript:;\" src=\"{0}\" onclick=\"toggleLight(\'{1}\'); return false;\" id=\"light_{1}\" style=\"position: absolute; left:{2}px; top:{3}px; width:{4}px; width:{5}px; z-index: 3; display: block\" />",
                            p.Pressed ? "?res=lampon" : "?res=lampoff", p.Target.Trim(),
                            p.GetX(settings) - p.GetRadius(settings) / 2, p.GetY(settings) - p.GetRadius(settings) / 2, p.GetRadius(settings), p.GetRadius(settings));
                    }
                    string backgroundname = "?res=background";
                    backgroundname += "&scene=" + settings.SceneName;
                    backgroundname += "&width=" + settings.Width + "&height=" + settings.Height + "&mip=" + settings.Detail;//add settings needed to generate a background image

                    string response = System.IO.File.ReadAllText("interface2.htm");
                    response = response.Replace("<<<<AREAS>>>>", Areas);
                    response = response.Replace("<<<<BGNAME>>>>", backgroundname);
                    response = response.Replace("<<<<WIDTH>>>>", settings.Width.ToString());
                    response = response.Replace("<<<<HEIGHT>>>>", settings.Height.ToString());
                    return System.Text.Encoding.ASCII.GetBytes(response);
            }
            return null;
        }

        public byte[] GetImageResized(string _FileName, int _Width, int _Height)
        {
            System.IO.FileStream file = new System.IO.FileStream(_FileName, System.IO.FileMode.Open);
            Image image = Image.FromStream(file);
            file.Close();
            Image resized = new Bitmap(image, _Width, _Height);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            resized.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
