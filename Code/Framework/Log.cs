using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class Log
    {
        public string Name;
        static Queue<Log.Line> WriteQueue = new Queue<Line>();
        public List<Log.Line> Entries = new List<Line>();
        public static List<Log.Line> AllEntries = new List<Line>();

        public Log(string _Name)
        {
            Name = _Name;
        }

        public static void Write(string _Log, string _Format, params object[] _Arguments)
        {
            Write(new Line(_Log,string.Format(_Format, _Arguments), Line.Type.Message));
        }

        public static void Write(string _Log, Log.Line.Type _Type, string _Format, params object[] _Arguments)
        {
            Write(new Log.Line(_Log, string.Format(_Format, _Arguments), _Type));
        }

        public static void Write(Log.Line _Line)
        {
            lock (WriteQueue)
            {
                WriteQueue.Enqueue(_Line);
            }
        }

        public static void Update()
        {
            lock (WriteQueue)
            {
                while (WriteQueue.Count > 0)
                {
                    Log.Line line = WriteQueue.Dequeue();
                    if (!log.ContainsKey(line.LogName))
                    {
                        Log l = new Log(line.LogName);
                        log.Add(line.LogName, l);
                        if (OnLogAdded != null) OnLogAdded(l);
                    }
                    log[line.LogName].Entries.Add(line);
                    AllEntries.Add(line);
                    if (OnLogWrite != null) OnLogWrite(line);
                    Console.WriteLine("{0} : {1}", line.LogName, line.Message);
                }
            }
        }

        public static Dictionary<string, Log> log = new Dictionary<string, Log>();

        public delegate void OnLogAddedHandler(Log _AddedLog);
        public static event OnLogAddedHandler OnLogAdded;

        public delegate void OnLogWriteHandler(Log.Line _LogLine);
        public static event OnLogWriteHandler OnLogWrite;

        public class Line
        {
            public Line(string _LogName, string _Message, Type _Type)
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
    }
}
