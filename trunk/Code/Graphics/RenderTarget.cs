using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SynergyTemplate;

namespace SynergyGraphics
{
    public class RenderTarget
    {
        internal RenderTarget2D rendertarget;
        public RenderTarget(Int2 _Size)
        {
            rendertarget = new RenderTarget2D(Graphics.device, _Size.X, _Size.Y, 0, SurfaceFormat.Rgba32);
        }
    }
}
