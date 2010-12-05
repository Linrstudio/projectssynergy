using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WebServer
{
    public class Interface1 : WebInterface
    {
        public Interface1() : base("1") { }
        public override byte[] HandleCommand(URLParser _Parameters)
        {
            string command = _Parameters.Get("command");
            string resource = _Parameters.Get("res");

            Settings settings = new Settings();
            try
            {
                settings.Width = int.Parse(_Parameters.Get("width"));
            }
            catch { }
            try
            {
                settings.Height = int.Parse(_Parameters.Get("height"));
            }
            catch { }
            try
            {
                settings.Detail = int.Parse(_Parameters.Get("mip"));
            }
            catch { }

            if (command.ToLower() != null)
            {

            }
            return new byte[] { };
        }
    }
}
