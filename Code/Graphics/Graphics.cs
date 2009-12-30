using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SynergyTemplate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynergyGraphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Float3 Position;
        public Float2 UV;

        public Vertex(Float3 _Position, Float2 _UV)
        {
            Position = _Position;
            UV = _UV;
        }
    }

    public class Graphics
    {
        public static int ImageMipLevelDepth = 1;
        public static GraphicsDevice device = null;

        public struct DeviceTarget
        {
            public DeviceTarget(IntPtr _WindowHandle, Int2 _Resolution)
            {
                WindowHandle = _WindowHandle;
                Resolution = _Resolution;
            }
            public IntPtr WindowHandle;
            public Int2 Resolution;
        }

        public static void Initialize(IntPtr _WindowHandle,Int2 _Resolution)
        {
            foreach (GraphicsAdapter adapter in GraphicsAdapter.Adapters)
            {
                Log.Write("Graphics", "Adapter found :", adapter.Description);
            }

            var parameter = new PresentationParameters();
            parameter.BackBufferWidth = _Resolution.X;
            parameter.BackBufferHeight = _Resolution.Y;
            parameter.DeviceWindowHandle = _WindowHandle;

            device = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, _WindowHandle, parameter);
        }

        public static void Flush()
        {
            device.Present();
        }

        public static void Update()
        {

        }

        public static void SetAlphaBlending(bool _Enabled)
        {
            device.RenderState.AlphaBlendEnable = _Enabled;
            if (_Enabled)
            {
                device.RenderState.AlphaBlendOperation = BlendFunction.Add;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                device.RenderState.SeparateAlphaBlendEnabled = false;
            }
        }

        public static void Clear(Float4 _Color)
        {
            device.Clear(ClearOptions.Target, new Color(_Color.X, _Color.Y, _Color.Z, _Color.W), 0, 0);
        }

        public static void ClearZBuffer(float _Depth)
        {
            device.Clear(ClearOptions.DepthBuffer, Color.Black, _Depth, 0);
        }

        /// <summary>
        /// draws a rectangle using the provided image to the active render target
        /// </summary>
        /// <param name="_A">Left upper Coordinate</param>
        /// <param name="_B">Right upper Coordinate</param>
        /// <param name="_C">Left lower Coordinate</param>
        /// <param name="_D">Right lower Coordinate</param>
        /// <param name="_Image"></param>
        public static void DrawRectangle(Float3 _A, Float3 _B, Float3 _C, Float3 _D, TextureGPU _Image)
        {
            device.RenderState.DepthBufferEnable = false;
            device.RenderState.CullMode = CullMode.None;

            VertexBuffer vbo = new VertexBuffer(Graphics.device, Marshal.SizeOf(typeof(Vertex)) * 4, BufferUsage.None);
            VertexPositionTexture[] vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(_A.X,_A.Y,_A.Z), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(_B.X,_B.Y,_B.Z), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(_C.X,_C.Y,_C.Z), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(_D.X,_D.Y,_D.Z), new Vector2(1, 1))
            };
            vbo.SetData(vertices);
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
    }
}
