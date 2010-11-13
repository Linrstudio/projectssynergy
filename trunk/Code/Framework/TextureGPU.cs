using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Synergy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synergy
{
    public class TextureGPUResource : Resource
    {
        TextureGPU data = null;
        public TextureGPUResource(string _ResourceName)
            : base(_ResourceName)
        {
            Update();
        }
        public override object Get()
        {
            if (status == ResourceStatus.Loaded)
                return data;
            else
            {
                //Update();
                return null;
            }
        }
        public override void Update()
        {
            if (status == ResourceStatus.Pending)
            {
                status = ResourceStatus.Loading;
                new System.Threading.Thread(new ThreadStart(Load)).Start();
            }
        }
        void Load()
        {
            try
            {
#if false//SIMULATE LOADING
                for (int i = 0; i < 100; i++)
                {
                    System.Threading.Thread.Sleep(100);
                    LoadingProgress = (float)i / 100.0f;
                }
                data = new TextureGPU(ResourceName);
#else
                LoadingProgress = 0;
                data = new TextureGPU(ResourceName);
                LoadingProgress = 1;
#endif
            }
            catch
            {
                status = ResourceStatus.Failed;
            }
            finally
            {
                status = ResourceStatus.Loaded;
            }
        }
    }
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
        public TextureGPU(Int2 _Size)
        {
            size = _Size;
            //texture = new Texture2D(Graphics.device, size.X, size.Y, Graphics.ImageMipLevelDepth, TextureUsage.None, SurfaceFormat.Rgba32);
            texture = new Texture2D(Graphics.device, size.X, size.Y);
        }
        internal TextureGPU(Texture2D _Source)
        {
            texture = _Source;
            size = new Int2(texture.Width, texture.Height);
        }

        ~TextureGPU()
        {
            if (texture != null)
            {
                //texture.Dispose(); //ermm yea,, no, i dont gett it
                texture = null;
            }
        }
    }
}
