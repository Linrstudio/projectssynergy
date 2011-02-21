using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using System.Drawing;

namespace WebInterface
{
    public class Control
    {
        public Scene scene = null;
        public Control() { }
        public Control(string _Name, float _X, float _Y, float _Width, float _Height) { X = _X; Y = _Y; Width = _Width; Height = _Height; Name = _Name; }
        // note that a scene is 1 by 1 in size
        float x, y, width, height;
        [Browsable(true), CategoryAttribute("Transform")]
        public float X { get { return x; } set { x = value; } }
        [Browsable(true), CategoryAttribute("Transform")]
        public float Y { get { return y; } set { y = value; } }
        [Browsable(true), CategoryAttribute("Transform")]
        public float Width { get { return width; } set { width = value; } }
        [Browsable(true), CategoryAttribute("Transform")]
        public float Height { get { return height; } set { height = value; } }

        string name;
        [Browsable(true), CategoryAttribute("Control")]
        public string Name
        {
            get { return name; }
            set
            {
                if (scene != null) foreach (Control c in scene.Controls) if (c.name == value) return;
                name = value;
            }
        }

        public virtual void Load(XElement _Data) { }
        public virtual void Save(XElement _Data) { }

        public class Prototype
        {
            public string ControlName;
            public string GroupName;
            public bool UserCanAdd;
            public Type Type;

            public Prototype(string _ControlName, string _GroupName, Type _Type, bool _UserCanAdd)
            {
                ControlName = _ControlName;
                GroupName = _GroupName;
                Type = _Type;
                UserCanAdd = _UserCanAdd;
            }
            public override string ToString()
            {
                return ControlName;
            }
        }
    }

    public class Switch : Control
    {
        public Switch() : base() { }
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
        //is this control waiting for its state to change
        public bool Loading;
    }
}
