using System;
using System.Collections.Generic;
using System.Text;
using Synergy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synergy
{
    public class RenderTarget
    {
        public enum SurfaceFormat
        {
            Color = Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color,
            Float4 = Microsoft.Xna.Framework.Graphics.SurfaceFormat.Vector4,
            Float4Half = Microsoft.Xna.Framework.Graphics.SurfaceFormat.HalfVector4
        }

        internal RenderTarget2D rendertarget;

        public Int2 Size { get { return new Int2(rendertarget.Width, rendertarget.Height); } }

        public RenderTarget(Int2 _Size)
        {
            rendertarget = new RenderTarget2D(Graphics.device, _Size.X, _Size.Y, 0, (Microsoft.Xna.Framework.Graphics.SurfaceFormat)SurfaceFormat.Color);
        }

        public RenderTarget(Int2 _Size, SurfaceFormat _Format)
        {
            rendertarget = new RenderTarget2D(Graphics.device, _Size.X, _Size.Y, 0, (Microsoft.Xna.Framework.Graphics.SurfaceFormat)_Format);
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
