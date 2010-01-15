using System;
using System.Collections.Generic;
using System.Text;
using SynergyTemplate;

namespace SynergyGraphics
{
    public class Control
    {
        public static Shader SelectionShader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Selected.fx"));

        public string name;
        public string Name { get { return name; } }
        public Control Parent;
        public Dictionary<string, Control> Children = new Dictionary<string, Control>();

        public Float2 Position;
        public Float2 Size = new Float2(1, 1);
        public Float2 Scale = new Float2(1, 1);
        public Int2 Resolution = new Int2(800, 600);

        public delegate void Render();
        public Render OnRender;

        public bool NeedsRefresh = true;

        public delegate void OnRefreshHandler();
        public event OnRefreshHandler OnRefresh = null;


        public TextureGPU Texture;

        RenderTarget Target;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Name">Name of the Control</param>
        /// <param name="_Parent">Parent Control</param>
        public Control(string _Name, Control _Parent)
        {
            name = _Name;
            Parent = _Parent;
            if (Parent != null) Parent.Children.Add(_Name, this);
        }
        /// <summary>
        /// Refreshes this control
        /// </summary>
        public virtual void Refresh()
        {
            if (Target == null || Resolution != Target.Size)
            {
                Target = new RenderTarget(Resolution);
                Log.Write("Control", "RenderTarget for Control {0} refreshed", Name);
            }
            foreach (Control child in Children.Values)
            {
                if (child.Texture == null)
                    child.Refresh();
            }

            Graphics.SetRenderTarget(Target);
            Graphics.Clear(new Float4(0, 0, 0, 0));
            if (OnRender != null)
            {
                Graphics.defaultshader.SetParameter("Scale", Scale);
                OnRender();
            }
            else
            {
                Graphics.SetAlphaBlending(true);
                DrawChildren();
            }
            Graphics.SetRenderTarget(null);
            TextureGPU tex = Target.Resolve();
            if (tex != null) Texture = tex;
            if (Parent != null) Parent.NeedsRefresh = true;
            if (OnRefresh != null) OnRefresh();
        }

        public void DrawChildren()
        {
            Graphics.defaultshader.SetParameter("Scale", Scale);
            foreach (Control child in Children.Values)
            {
                child.Draw();
            }
        }

        public virtual void Update()
        {
            if (Parent != null)
            {
                Float2 asp = Size / Parent.Size;
                Resolution.X = (int)((float)Parent.Resolution.X * asp.X);
                Resolution.Y = (int)((float)Parent.Resolution.Y * asp.Y);
            }
            if (Target == null || Resolution != Target.Size)
                NeedsRefresh = true;

            if (NeedsRefresh)
                Refresh();
            NeedsRefresh = false;
            foreach (Control c in Children.Values) c.Update();
        }

        public virtual void Draw()
        {
            if (Texture == null) Refresh();
            Graphics.defaultshader.SetParameter("Scale", Scale);
            Graphics.defaultshader.SetParameter("DiffuseMap", Texture);
            Graphics.defaultshader.Begin();
            Graphics.DrawRectangle(
                Position,
                Position + new Float2(Size.X, 0),
                Position + new Float2(0, Size.Y),
                Position + Size, 0.5f);
            Graphics.defaultshader.End();
        }
    }
}
