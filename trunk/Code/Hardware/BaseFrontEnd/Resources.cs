using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;

namespace BaseFrontEnd
{
    public class Resources
    {
        public static Image Background;

        public static Image LoadImage(string _Path)
        {
            FileStream f = new FileStream(_Path,FileMode.Open);
            Image i = Image.FromStream(f);
            f.Close();
            return i;
        }

        public static void Load()
        {
            Background = LoadImage(@".\Images\Background.png");
        }
    }
}
