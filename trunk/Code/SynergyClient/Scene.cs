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
        string Path;
        public string Name;
        public Scene(string _Path)
        {
            Path = _Path;
            XElement element = XElement.Load(Path);
            try
            {
                BackGroundImagePath = (string)element.Element("BackgroundImage").Value;
                BackgroundImage = Image.FromFile(BackGroundImagePath);
            } catch { }
            Name = (string)element.Element("SceneName").Value;
            foreach (XElement e in element.Elements("Device"))
            {
                Devices.Add(SceneDevice.GetDevice(e));
            }
        }
        public void Save()
        {
            Save(Path);
        }
        public void Save(string _Path)
        {
            XElement root = new XElement("SceneGraph");
            root.Add(new XElement("BackgroundImage", BackGroundImagePath));
            root.Add(new XElement("SceneName", Name));
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
        public static Dictionary<string, Scene> LoadScenes(string _Directory)
        {
            Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
            foreach (string file in System.IO.Directory.GetFiles(_Directory))
            {
                if (System.IO.Path.GetExtension(file) == ".xml")
                {
                    Scene s = new Scene(file);
                    if (s != null) scenes.Add(s.Name, s);
                }
            }
            return scenes;
        }
    }
}
