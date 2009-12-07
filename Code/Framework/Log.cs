using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class LogLine
    {
        public LogLine(string _LogName,string _Message, Type _Type)
        {
            LogName = _LogName;
            Message = _Message;
            type = _Type;
            Time = DateTime.Now;
        }

        public enum Type { Message, Warning, Error }

        public string LogName;
        public Type type;
        public DateTime Time;
        public int HitCount;
        public string Message;

        public string ToString()
        {
            return Time.ToShortTimeString() + " | " + Message + (HitCount > 0 ? " (" + HitCount + ")" : "");
        }
    }

    public class Log
    {
        public string Name;
        static Queue<LogLine> WriteQueue = new Queue<LogLine>();
        public List<LogLine> Entries = new List<LogLine>();

        public Log(string _Name)
        {
            Name = _Name;
        }

        public static void Write(string _Log, string _Format, params object[] _Arguments)
        {
            Write(new LogLine(_Log,string.Format(_Format, _Arguments), LogLine.Type.Message));
        }

        public static void Write(string _Log, LogLine.Type _Type, string _Format, params object[] _Arguments)
        {
            Write(new LogLine(_Log, string.Format(_Format, _Arguments), _Type));
        }

        public static void Write(LogLine _Line)
        {
            if (!log.ContainsKey(_Line.LogName)) 
            {
                Log l = new Log(_Line.LogName);
                log.Add(_Line.LogName, l);
                if (OnLogAdded != null) OnLogAdded(l);
            }
            log[_Line.LogName].Entries.Add(_Line);
            if (OnLogWrite != null) OnLogWrite(_Line);
            Console.WriteLine("{0} : {1}", _Line.LogName, _Line.Message);
        }

        public static void Update()
        {
            lock (WriteQueue)
            {
                while (WriteQueue.Count > 0)
                {
                    LogLine line = WriteQueue.Dequeue();
                    if (!log.ContainsKey(line.LogName))
                    {
                        Log l = new Log(line.LogName);
                        log.Add(line.LogName, l);
                        if (OnLogAdded != null) OnLogAdded(l);
                    }
                    log[line.LogName].Entries.Add(line);
                    if (OnLogWrite != null) OnLogWrite(line);
                    Console.WriteLine("{0} : {1}", line.LogName, line.Message);
                }
            }
        }

        public static Dictionary<string, Log> log = new Dictionary<string, Log>();

        public delegate void OnLogAddedHandler(Log _AddedLog);
        public static event OnLogAddedHandler OnLogAdded;

        public delegate void OnLogWriteHandler(LogLine _LogLine);
        public static event OnLogWriteHandler OnLogWrite;
    }
}
