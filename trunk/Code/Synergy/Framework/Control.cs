using System;
using System.Collections.Generic;
using System.Text;
using Synergy;

namespace Synergy
{
    public abstract class Control
    {
        string name;
        public string Name { get { return name; } }
        //parent and children
        public Control Parent;
        List<Control> Children = new List<Control>();
        //position and size, but you dont need to wory about that
        public Float3x3 Transformation;
        public delegate void Render();
        public Render OnRender;
        protected int depth;

        public void AddChild(Control _Control)
        {
            foreach (Control c in Children) if (c.Name == _Control.Name) return;
            _Control.Parent = this;
            Children.Add(_Control);
        }

        public Control GetChild(string _Name)
        {
            foreach (Control c in Children) if (c.Name == _Name) return c;
            return null;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Controller"></param>
        /// <returns>returns true if this or any child controls used the input of the controller</returns>
        public bool HandleControllerInput(UITouchEvent _Event)
        {
            bool anyhandled = false;
            if (OnControllerInput != null)
            {
                foreach (Delegate d in OnControllerInput.GetInvocationList())
                {
                    try
                    {
                        bool result = ((bool)d.DynamicInvoke(_Event));
                        if (result) anyhandled = result;
                    }
                    catch (System.Reflection.TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }

                }
            }
            //might want to change this priority
            // if we dont need the Input try one of our children
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                if (Children[i].HandleControllerInput(_Event)) break;
            }
            return anyhandled;
        }

        public delegate bool OnControllerInputHandler(UITouchEvent _Event);
        public event OnControllerInputHandler OnControllerInput = null;

        public void DrawChildren()
        {
            //update depths
            List<Control> newchildren = new List<Control>(Children);
            Children.Clear();
            while (newchildren.Count > 0)
            {
                int bestdeppth = int.MaxValue;
                Control best = null;
                foreach (Control c in newchildren) { if (c.depth <= bestdeppth) { bestdeppth = c.depth; best = c; } }
                newchildren.Remove(best);
                best.depth = Children.Count;
                Children.Add(best);
            }
            //and draw children
            foreach (Control child in Children) child.Draw();
        }

        public virtual void Update()
        {
            foreach (Control child in Children) child.Update();
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

        public void BringToFront()
        {
            if (Parent == null) return;
            depth = 0;
            foreach (Control c in Parent.Children)
            {
                if (c == this) continue;
                if (c.depth >= depth) depth = c.depth + 1;
            }
        }
    }
}
