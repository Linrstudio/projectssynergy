using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using SynergyTemplate;

namespace SynergyGraphics
{
    public class TextureGPU
    {
        internal Texture2D texture;
        Int2 size;
        public Int2 Size { get { return size; } }

        public TextureGPU(string _FileName)
        {
            texture = Texture2D.FromFile(Graphics.device, _FileName);
            size = new Int2(texture.Width, texture.Height);
        }
    }
}
