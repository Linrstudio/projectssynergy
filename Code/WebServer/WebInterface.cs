using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;

namespace WebServer
{
    public abstract class WebInterface
    {
        public class Settings
        {
            public Settings()
            {
                Width = 320;
                Height = 480;
                Detail = 1;
            }
            public Settings(int _Width, int _Height, int _Detial, string _SceneName)
            {
                Width = _Width;
                Height = _Height;
                Detail = _Detial;
                SceneName = _SceneName;
            }
            public int Width, Height, Detail;
            public string SceneName;
        }

        string name;
        public string Name { get { return name; } }

        public WebInterface(string _Name)
        {
            name = _Name;
        }

        public abstract byte[] HandleCommand(URLParser _Parameters);

    }
}
