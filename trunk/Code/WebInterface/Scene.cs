using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
