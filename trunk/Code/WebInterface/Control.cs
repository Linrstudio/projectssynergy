using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebInterface
{
    public class Control
    {
        public Control() { }
        public Control(string _Name, float _X, float _Y, float _Width, float _Height) { X = _X; Y = _Y; Width = _Width; Height = _Height; Name = _Name; }
        // note that a scene is 1 by 1 in size
        public float X, Y, Width, Height;
        public string Name;
    }

    public class Switch : Control
    {
        public Switch(string _Name, float _X, float _Y, float _Width, float _Height) : base(_Name, _X, _Y, _Width, _Height) { }
        public string ImageOn = "switchon.png";
        public string ImageOff = "switchoff.png";
        public string ImageLoading = "switchloading.gif";
        public bool State;

        public enum Command { Toggle };
        Queue<Command> commands = new Queue<Command>();
        public Command DequeueCommand() { lock (commands) { return commands.Dequeue(); } }
        public void EnqueueCommand(Command _Command) { lock (commands) { commands.Enqueue(_Command); } }
        public bool CommandsPending() { lock (commands) { return commands.Count > 0; } }
    }
}
