using System;
using System.Collections.Generic;
using System.Text;

using SynergyGraphics;
using SynergyTemplate;

namespace SynergyClient
{
    public class ClientResources
    {
        public static SpriteFont font = null;
        public static SpriteFont handwrittenfont = null;

        public static TextureGPU GenericBrowserElementBackground = null;
        public static TextureGPU GenericBrowserHeaderBackground = null;

        public static TextureGPU Background = null;

        public static void Load()
        {
            font = new SpriteFont(@".\content\Arial.png", @".\content\Arial.xml");

            handwrittenfont = new SpriteFont(@".\content\Handwritten.png", @".\content\Handwritten.xml");

            GenericBrowserElementBackground = new TextureGPU(@".\content\GenericBrowserElementBackground.png");
            GenericBrowserHeaderBackground = new TextureGPU(@".\content\GenericBrowserHeaderBackground.png");

            Background = new TextureGPU(@".\content\Background.png");
        }
    }
}
