using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebInterface
{
    public class URLParser
    {
        public Dictionary<string, string> Parameters;
        public URLParser(string _Source)
        {
            try
            {
                Parameters = new Dictionary<string, string>();
                string[] split = _Source.Split('?')[1].Split('&');
                foreach (string s in split)
                {
                    string[] spl = s.Split('=');
                    if (spl.Length == 2)
                    {
                        if (spl[0].Length != 0 && spl[1].Length != 0)
                            Parameters.Add(spl[0], spl[1].Trim());
                    }
                }
            }
            catch { }
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
                str += (str == "" ? "?" : "&") + k + "=" + Parameters[k];
            }
            return str;
        }
    }
}
