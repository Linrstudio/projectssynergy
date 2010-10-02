using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synergy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynergyGraphics
{
    public class RenderTarget
    {
        internal RenderTarget2D rendertarget;

        public Int2 Size { get { return new Int2(rendertarget.Width, rendertarget.Height); } }

        public RenderTarget(Int2 _Size)
        {
            rendertarget = new RenderTarget2D(Graphics.device, _Size.X, _Size.Y, 0, SurfaceFormat.Color);
        }

        public TextureGPU Resolve()
        {
            Texture2D tex = null;
            try
            {
                tex = rendertarget.GetTexture();
            }
            catch
            {
                return null; 
            }
            return new TextureGPU(tex);
        }
    }
}
