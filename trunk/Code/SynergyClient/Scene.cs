using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Drawing;

namespace SynergyClient
{
    public class Scene
    {
        public Image BackgroundImage = null;
        public string BackGroundImagePath;
        public List<SceneDevice> Devices = new List<SceneDevice>();
        public Scene(string _Path)
        {
            XElement element = XElement.Load(_Path);
            try
            {
                BackGroundImagePath = (string)element.Element("BackgroundImage").Value;
                BackgroundImage = Image.FromFile(BackGroundImagePath);
            } catch { }
            foreach (XElement e in element.Elements("Device"))
            {
                switch (byte.Parse((string)e.Element("Type").Value))
                {
                    case 0:
                        Devices.Add(new DigitalOutSceneDevice(e)); break;
                    case 2:
                        Devices.Add(new AnalogOutSceneDevice(e)); break;
                }
            }
        }
        public void Save(string _Path)
        {
            XElement root = new XElement("SceneGraph");
            root.Add(new XElement("BackgroundImage", BackGroundImagePath));
            foreach (SceneDevice d in Devices)
            {
                root.Add(d.Save());
            }
            root.Save(_Path);
        }
        public SceneDevice GetDevice(float _X,float _Y)
        {
            float dist = 1000000;
            SceneDevice found = null;
            foreach (SceneDevice d in Devices)
            {
                float dx=_X-d.X, dy=_Y-d.Y;
                float dis = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dis < dist && dis < d.Size * 0.5f) { dist = dis; found = d; }
            }
            return found;
        }
    }
}
