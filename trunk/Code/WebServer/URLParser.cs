using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer
{
    public class URLParser
    {
        public Dictionary<string, string> Parameters;
        public URLParser(string _Source)
        {
            Parameters = new Dictionary<string, string>();
            string[] split = _Source.Split('?');
            foreach (string s in split)
            {
                string[] spl = s.Split('=');
                if (spl.Length == 2)
                    Parameters.Add(spl[0], spl[1]);
            }
        }

        public string Get(string _Name)
        {
            if (Parameters.ContainsKey(_Name))
                return Parameters[_Name];
            else
                return "";
        }

        public override string ToString()
        {
            string str = "";
            foreach (string k in Parameters.Keys)
            {
                str += "?" + k + "=" + Parameters[k];
            }
            return str;
        }
    }
}
