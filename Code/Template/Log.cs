using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyTemplate
{
    public class Log
    {
        public string Name;
        static Queue<Line> LineQueue = new Queue<Line>();
        static Queue<Variable> VariableQueue = new Queue<Variable>();

        public List<Line> Lines = new List<Line>();
        public List<Variable> Variables= new List<Variable>();

        public static List<Line> AllLines = new List<Line>();
        public static List<Variable> AllVariables = new List<Variable>();

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
            string result = "";
            try
            {
                 result = string.Format(_Format, _Arguments);
            }
            catch { result = _Format; }
            Write(new Log.Line(_Log, result, _Type));
        }

        public static void Write(Log.Line _Line)
        {
            lock (LineQueue)
            {
                LineQueue.Enqueue(_Line);
            }
        }

        public static void Write(Log.Variable _Variable)
        {
            lock (VariableQueue)
            {
                VariableQueue.Enqueue(_Variable);
            }
        }

        public static void Update()
        {
            lock (LineQueue)
            {
                while (LineQueue.Count > 0)
                {
                    Line line = LineQueue.Dequeue();
                    if (!log.ContainsKey(line.LogName))
                    {
                        Log l = new Log(line.LogName);
                        log.Add(line.LogName, l);
                        if (OnLogAdded != null) OnLogAdded(l);
                    }
                    log[line.LogName].Lines.Add(line);
                    AllLines.Add(line);
                    if (OnWriteLine != null) OnWriteLine(line);
                }
            }
            lock (VariableQueue)
            {
                while (VariableQueue.Count > 0)
                {
                    Variable variable = VariableQueue.Dequeue();
                    if (!log.ContainsKey(variable.LogName))
                    {
                        Log l = new Log(variable.LogName);
                        log.Add(variable.LogName, l);
                        if (OnLogAdded != null) OnLogAdded(l);
                    }
                    Variable found = null;
                    foreach (Variable v in log[variable.LogName].Variables)
                    {
                        if (v.VariableName == variable.VariableName) found = v;
                    }

                    if (found != null)
                    {
                        found.Time = DateTime.Now;
                        found.Value = variable.Value;
                        found.VariableName = variable.VariableName;
                    }
                    else
                    {
                        log[variable.LogName].Variables.Add(variable);
                        AllVariables.Add(variable);
                    }
                    if (OnWriteVariable != null) OnWriteVariable(variable);
                }
            }
        }

        public static Dictionary<string, Log> log = new Dictionary<string, Log>();

        public delegate void OnLogAddedHandler(Log _AddedLog);
        public static event OnLogAddedHandler OnLogAdded;

        public delegate void OnWriteLineHandler(Line _Line);
        public static event OnWriteLineHandler OnWriteLine;

        public delegate void OnWriteVariableHandler(Variable _Variable);
        public static event OnWriteVariableHandler OnWriteVariable;

        public class Variable
        {
            public DateTime Time;
            public string VariableName;
            public string LogName;
            public object Value;

            public Variable(string _LogName, string _VariableName, object _Value)
            {
                VariableName = _VariableName;
                LogName = _LogName;
                Value = _Value;
                Time = DateTime.Now;
            }

            public string ToString()
            {
                return Time.ToShortDateString() + " | " + VariableName + " = " + Value.ToString();
            }
        }

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
