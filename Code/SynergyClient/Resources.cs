using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SynergyClient
{
    public class Resources
    {
        public static Dictionary<string, Image> images;
        public static void Load()
        {
            images = new Dictionary<string, Image>();
            images.Add("light on", Image.FromFile("Images/light on.png"));
            images.Add("light off", Image.FromFile("Images/light off.png"));
        }
    }
}
