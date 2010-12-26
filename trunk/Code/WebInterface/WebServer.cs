using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WebInterface
{
    public class WebInterface
    {
        public void Start()
        {
            if (!HttpListener.IsSupported) Console.WriteLine("HTTPListener class is not supported on your machine.");

        }
        public List<Scene> Scenes = new List<Scene>();
    }
}
