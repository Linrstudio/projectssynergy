using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    struct LogLine
    {
        string Message;
    }

    class Log
    {
        public static Dictionary<string, Log> log = new Dictionary<string, Log>();

        public string Name;
        public List<string> Content;
        public void Print(string _Format, params object[] _Arguments)
        {

            Content.Add(string.Format(_Format, _Arguments));
        }
    }
}
