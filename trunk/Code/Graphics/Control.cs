using System;
using System.Collections.Generic;
using System.Text;
using Synergy;

namespace SynergyGraphics
{
    public class Control
    {
        public string name;
        public string Name { get { return name; } }
        //parent and children
        public Control Parent;
        public Dictionary<string, Control> Children = new Dictionary<string, Control>();
        //position and size, but you dont need to wory about that
        public Float3x3 Transformation = new Float3x3();
        public delegate void Render();
        public Render OnRender;

        public void AddChild(Control _Control)
        {
            if (!Children.ContainsKey(_Control.name)) Children.Add(_Control.name, _Control);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Name">Name of the Control</param>
        /// <param name="_Parent">Parent Control</param>
        public Control(string _Name, Control _Parent)
        {
            name = _Name;
            Parent = _Parent;
            if (Parent != null) Parent.AddChild(this);
        }

        public void DrawChildren()
        {
            foreach (Control child in Children.Values) child.Draw();
        }

        public virtual void Update()
        {
            foreach (Control c in Children.Values) c.Update();
        }
        /// <summary>
        /// Draws this control and all its children
        /// </summary>
        public virtual void Draw()
        {
            if (OnRender != null)
            {
                OnRender();
            }
            DrawChildren();
        }

        public Float3x3 GetTransformation()
        {
            if (Parent != null) return Parent.Transformation * Transformation;
            return Transformation;
        }
    }
}
