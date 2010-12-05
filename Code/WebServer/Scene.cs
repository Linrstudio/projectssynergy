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
        string name = "";
        public string Name { get { return name; } }

        //Image background;
        public Image Background;// { get { return background; } }

        public Scene()
        {

        }

        public Scene(string _Name)
        {
            name = _Name;
        }

        public Scene(string _Name, Scene _LandScapeFrom)
        {
            name = _Name;
            foreach (ClickableRegion region in _LandScapeFrom.ClickablePoints)
            {
                ClickableRegion reg = new ClickableRegion();
                reg.X = region.Y;
                reg.Y = region.X;
                reg.Radius = region.Radius;
                reg.Target = region.Target;
                ClickablePoints.Add(reg);
            }
            Background = new Bitmap(_LandScapeFrom.Background.Height, _LandScapeFrom.Background.Width);
            for (int Y = 0; Y < Background.Height; Y++)
            {
                for (int X = 0; X < Background.Width; X++)
                {
                    ((Bitmap)Background).SetPixel(X, Y, ((Bitmap)(_LandScapeFrom.Background)).GetPixel(Y, X));
                }
            }
        }

        public byte[] GetImageResized(int _Width, int _Height)
        {
            Bitmap result = new Bitmap(_Width, _Height);
            Graphics g = Graphics.FromImage(result);
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(Background,0,0, _Width, _Height);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            result.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }

        public static List<Scene> Scenes = new List<Scene>();

        public static Scene GetScene(string _SceneName)
        {
            foreach (Scene s in Scenes)
            {
                if (s.name == _SceneName)
                {
                    return s;
                }
            }
            return Scenes[0];
        }

        public class ClickableRegion
        {
            public ClickableRegion()
            {
            }
            public ClickableRegion(float _X, float _Y, float _Radius, string _Target)
            {
                X = _X;
                Y = _Y;
                Radius = _Radius;
                Target = _Target;
            }
            public float X, Y, Radius;
            public string Target = "";
            public bool Pressed;
            public Image On = null;
            public Image Off = null;

            public int GetX(WebInterface.Settings _Settings)
            {
                return (int)(X * _Settings.Width);
            }

            public int GetY(WebInterface.Settings _Settings)
            {
                return (int)(Y * _Settings.Height);
            }

            public int GetRadius(WebInterface.Settings _Settings)
            {
                return (int)(Radius * _Settings.Width);
            }
        }

        public List<ClickableRegion> ClickablePoints = new List<ClickableRegion>();
    }
}
