using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace WebInterface
{
    public class Credentials
    {
        string Username;
        string Password;
    }

    public class Scene
    {
        string name = "";

        public string BackgroundImage;//path

        /// <summary>
        /// Name of this scene
        /// </summary>
        public string Name { get { return name; } set { name = value; } }

        public Scene() { }
        public Scene(string _Name) { Name = _Name; }
        public Scene(XElement _Data)
        {
            name = _Data.Attribute("Name").Value;
            foreach (XElement element in _Data.Elements("Control"))
            {
                Control c = (Control)Activator.CreateInstance(null, "WebInterface." + element.Attribute("Type").Value).Unwrap();
                c.Name = element.Attribute("Name").Value;
                c.X = float.Parse(element.Attribute("X").Value);
                c.Y = float.Parse(element.Attribute("Y").Value);
                c.Width = float.Parse(element.Attribute("Width").Value);
                c.Height = float.Parse(element.Attribute("Height").Value);
                c.Load(element.Element("Data"));
                Controls.Add(c);
            }
        }

        bool LoginEnabled = false;//do users need to login ?
        List<Credentials> Credentials = new List<Credentials>();

        public List<Control> Controls = new List<Control>();
        public Control GetControl(string _Name)
        {
            foreach (Control c in Controls)
                if (c.Name == _Name)
                    return c;
            return null;
        }
    }
}
