using System;
using System.Collections.Generic;
using System.Text;
using SynergyTemplate;

namespace SynergyGraphics
{
    public class Control
    {
        public Control Parent;
        public Dictionary<string,Control> Children = new Dictionary<string,Control>();

        public Float2 Position;
        public Float2 Size;
        public Int2 Resolution;

        public delegate void Render();
        public Render OnRender;

        public TextureGPU Texture;

        public virtual void Refresh()
        {
            RenderTarget bob = new RenderTarget(Resolution);
            Graphics.SetRenderTarget(bob);
            if (OnRender != null)
            {
                OnRender();
                Graphics.SetRenderTarget(null);
                TextureGPU tex = bob.Resolve();
                if (tex != null) Texture = tex;
            }
        }
    }
}
