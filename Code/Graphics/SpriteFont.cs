using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

using SynergyTemplate;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynergyGraphics
{
    public class SpriteFont
    {
        public struct CharBounds
        {
            public Float2 Pos;
            public Float2 Size;
        }

        TextureGPU spritefont;
        Dictionary<char, CharBounds> bounds = new Dictionary<char, CharBounds>();
        public SpriteFont(string _TextureFileName, string _MetricsFileName)
        {
            spritefont = new TextureGPU(_TextureFileName);

            XElement file = XElement.Load(_MetricsFileName);

            foreach (XElement element in file.Elements("character"))
            {
                char charindex = (char)byte.Parse(element.Attribute("key").Value);
                CharBounds chr = new CharBounds();
                int x = int.Parse(((string)element.Element("x").Value).Replace(",", "."));
                int y = int.Parse(((string)element.Element("y").Value).Replace(",", "."));
                int width = int.Parse(((string)element.Element("width").Value).Replace(",", "."));
                int height = int.Parse(((string)element.Element("height").Value).Replace(",", "."));
                chr.Pos = new Float2((float)x, (float)y);
                chr.Pos.X /= (float)spritefont.Size.X;
                chr.Pos.Y /= (float)spritefont.Size.Y;
                chr.Size = new Float2((float)width, (float)height);
                chr.Size.X /= (float)spritefont.Size.X;
                chr.Size.Y /= (float)spritefont.Size.Y;
                bounds.Add(charindex, chr);
            }
        }

        public float StringWidth(string _String)
        {
            float width = 0;
            if (_String == null) _String = "";
            foreach (char c in _String)
            {
                width += bounds[c].Size.X;
            }
            return width;
        }

        public float CharWidth(char _Chr)
        {
            return bounds[_Chr].Size.X;
        }

        public void DrawMakeFit(Float2 _Pos, Float2 _Size, string _String, bool _VCenter)
        {
            float stringwidth = StringWidth(_String);

            Float2 b = new Float2(stringwidth, bounds['B'].Size.Y * ((float)spritefont.Size.Y / (float)spritefont.Size.X));
            b *= _Size.Y / b.Y;
            if (b.X > _Size.X) b *= _Size.X / b.X;

            //if (_VCenter) _Pos.Y += (_Size.Y - b.Y) / 2;

            Draw(_Pos, b, _String);
        }

        public void Draw(Float2 _Pos, float _Width, string _String)
        {
            float stringwidth = StringWidth(_String);
            float charheight = (bounds['B'].Size.Y / stringwidth) * ((float)spritefont.Size.Y / (float)spritefont.Size.X) * _Width;
            Draw(_Pos, new Float2(_Width, charheight), _String);
        }

        public void Draw(Float2 _Pos, Float2 _Size, string _String)
        {
            float stringwidth = StringWidth(_String);

            float charheight = _Size.Y;

            //Float2 Delta = new Float2(_Size / stringwidth, 0);
            Float2 Pos = _Pos;

            Graphics.defaultshader.SetParameter("DiffuseMap", spritefont);
            Graphics.defaultshader.Begin();
            foreach (char c in _String)
            {
                CharBounds chr = bounds[c];

                float charwidth = (CharWidth(c) / stringwidth) * _Size.X;

                Graphics.DrawRectangle(
                    _Pos,
                    _Pos + new Float2(charwidth, 0),
                    _Pos + new Float2(0, charheight),
                    _Pos + new Float2(charwidth, charheight),

                    chr.Pos,
                    chr.Pos + new Float2(chr.Size.X, 0),
                    chr.Pos + new Float2(0, chr.Size.Y),
                    chr.Pos + chr.Size, 0.5f);

                _Pos.X += charwidth;
            }
            Graphics.defaultshader.End();
        }
    }
}
