using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebServer
{
    public class Scene
    {
        public class Settings
        {
            public Settings()
            {
                Width = 320;
                Height = 480;
                Detail = 1;
            }
            public Settings(int _Width, int _Height, int _Detial)
            {
                Width = _Width;
                Height = _Height;
                Detail = _Detial;
            }
            public int Width, Height, Detail;
        }

        public class ClickableRegion
        {
            public ClickableRegion(float _X, float _Y, float _Radius, string _Target)
            {
                X = _X;
                Y = _Y;
                Radius = _Radius;
                Target = _Target;
            }
            public float X, Y, Radius;
            public string Target;
            public bool Pressed;
            public Image On = null;
            public Image Off = null;

            public int GetX(Settings _Settings)
            {
                return (int)(X * _Settings.Width);
            }

            public int GetY(Settings _Settings)
            {
                return (int)(Y * _Settings.Height);
            }

            public int GetRadius(Settings _Settings)
            {
                return (int)(Radius * _Settings.Width);
            }
        }

        public Image BackgroundImage;
        public Image DefaultOnImage;
        public Image DefaultOffImage;

        public List<ClickableRegion> ClickablePoints = new List<ClickableRegion>();

        public string GetHTML(Settings _Settings)
        {
            string Areas = "";
            foreach (Scene.ClickableRegion p in ClickablePoints)
            {
                Areas += string.Format("<img src=\"{0}\" onclick=\"toggleLight(\'{1}\'); return false;\" id=\"light_{1}\" style=\"position: absolute; left:{2}px; top:{3}px; z-index: 3; display: block\" />",
                    p.Pressed ? "?res=lampon" : "?res=lampoff", p.Target.Trim(),
                    p.GetX(_Settings), p.GetY(_Settings), p.GetRadius(_Settings) / 2, p.GetRadius(_Settings) / 2);
            }
            string response = System.IO.File.ReadAllText("index.htm");
            response = response.Replace("<<<<AREAS>>>>", Areas);
            string backgroundname = "?res=background" + "?width=" + _Settings.Width + "?height=" + _Settings.Height + "?detail=" + _Settings.Detail;
            response = response.Replace("<<<<BGNAME>>>>", backgroundname);
            response = response.Replace("<<<<WIDTH>>>>", _Settings.Width.ToString());
            response = response.Replace("<<<<HEIGHT>>>>", _Settings.Height.ToString());
            return response;
        }

        public Image GetImage(Settings _Settings)
        {
            Bitmap b = new Bitmap(_Settings.Width / _Settings.Detail, _Settings.Height / _Settings.Detail);
            Graphics graphics = Graphics.FromImage(b);
            graphics.DrawImage(BackgroundImage, 0, 0, b.Width, b.Height);
            /*
            foreach (ClickableRegion p in ClickablePoints)
            {
                Image on = p.On != null ? p.On : DefaultOnImage;
                Image off = p.Off != null ? p.Off : DefaultOffImage;
                graphics.DrawImage(p.Pressed ? on : off,
                    new RectangleF(
                        (p.GetX(_Settings) - p.GetRadius(_Settings) / 2.0f) / _Settings.Detail,
                        (p.GetY(_Settings) - p.GetRadius(_Settings) / 2.0f) / _Settings.Detail,
                        p.GetRadius(_Settings) / _Settings.Detail, p.GetRadius(_Settings) / _Settings.Detail));
            }
            */
            return (Image)b;
        }
    }
}
