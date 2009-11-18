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
            images.Add("DigitalOn", Image.FromFile("Images/DigitalOn.png"));
            images.Add("DigitalOff", Image.FromFile("Images/DigitalOff.png"));
            images.Add("DigitalNotFound", Image.FromFile("Images/DigitalNotFound.png"));
            images.Add("Options", Image.FromFile("Images/Options.png"));
            images.Add("OptionsPressed", Image.FromFile("Images/OptionsPressed.png"));
            images.Add("AnalogTick", Image.FromFile("Images/AnalogTick.png"));
            images.Add("AnalogIndicator", Image.FromFile("Images/AnalogIndicator.png"));
            images.Add("AnalogBackground", Image.FromFile("Images/AnalogBackground.png"));
        }
    }
}
