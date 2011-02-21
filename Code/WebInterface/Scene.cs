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
                AddControl(c);
            }
        }

        public void Save(XElement _Data)
        {
            _Data.SetAttributeValue("Name", name);
            foreach (Control c in Controls)
            {
                XElement element = new XElement("Control");
                element.SetAttributeValue("Name", c.Name);
                element.SetAttributeValue("Type", c.GetType().Name);
                element.SetAttributeValue("X", c.X);
                element.SetAttributeValue("Y", c.Y);
                element.SetAttributeValue("Width", c.Width);
                element.SetAttributeValue("Height", c.Height);
                XElement save = new XElement("Data");
                c.Save(save);
                element.Add(save);
                _Data.Add(element);
            }
        }

        bool LoginEnabled = false;//do users need to login ?
        List<Credentials> Credentials = new List<Credentials>();

        List<Control> controls = new List<Control>();
        public Control[] Controls { get { return controls.ToArray(); } }
        public void AddControl(Control _Control)
        {
            _Control.scene = this;
            controls.Add(_Control);
        }
        public Control GetControl(string _Name)
        {
            foreach (Control c in Controls)
                if (c.Name == _Name)
                    return c;
            return null;
        }
    }
}
