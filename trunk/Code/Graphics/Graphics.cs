using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Synergy;

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
        public static int ImageMipLevelDepth = 3;
        public static GraphicsDevice device = null;

        public static Shader defaultshader = null;

        public static Synergy.Rect GetTotalDesktopSize()
        {
            Synergy.Rect rect = new Synergy.Rect(new Int2(100000, 100000), new Int2(-100000, -100000));
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Left < rect.From.X) rect.From.X = screen.Bounds.Left;
                if (screen.Bounds.Top < rect.From.Y) rect.From.Y = screen.Bounds.Top;

                if (screen.Bounds.Right > rect.To.X) rect.To.X = screen.Bounds.Right;
                if (screen.Bounds.Bottom > rect.To.Y) rect.To.Y = screen.Bounds.Bottom;
            }

            return rect;
        }

        public static void Initialize(IntPtr _WindowHandle, Int2 _Resolution)
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




            //load default stuff
            defaultshader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Default.fx"));
        }

        public static void Present()
        {
            device.Present();
        }

        public static void SetRenderTarget(RenderTarget _RenderTarget)
        {
            if (_RenderTarget != null)
                device.SetRenderTarget(0, _RenderTarget.rendertarget);
            else
                device.SetRenderTarget(0, null);
        }

        public static void Present(IntPtr _WindowHandle)
        {
            device.Present(_WindowHandle);
        }

        public static void Present(IntPtr _WindowHandle, Float2 _SourceRectFrom, Float2 _SourceRectTo, Int2 _TargetRectFrom, Int2 _TargetRectTo)
        {
            float w = device.PresentationParameters.BackBufferWidth;
            float h = device.PresentationParameters.BackBufferHeight;
            _SourceRectTo.X *= w;
            _SourceRectTo.Y *= h;
            _SourceRectFrom.X *= w;
            _SourceRectFrom.Y *= h;

            _SourceRectTo -= _SourceRectFrom;
            _TargetRectTo -= _TargetRectFrom;
            device.Present(
                new Rectangle((int)_SourceRectFrom.X, (int)_SourceRectFrom.Y, (int)_SourceRectTo.X, (int)_SourceRectTo.Y),
                new Rectangle(_TargetRectFrom.X, _TargetRectFrom.Y, _TargetRectTo.X, _TargetRectTo.Y),

                _WindowHandle);
        }

        public static void Update()
        {

        }

        public static void SetAlphaBlending(bool _Enabled)
        {
            device.RenderState.AlphaBlendEnable = _Enabled;
            if (_Enabled)
            {
                device.RenderState.SeparateAlphaBlendEnabled = true;
                device.RenderState.AlphaBlendOperation = BlendFunction.Max;
                device.RenderState.BlendFunction = BlendFunction.Add;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            }
        }

        public static void SetDepthCheck(bool _Enabled)
        {
            device.RenderState.DepthBufferEnable = _Enabled;
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
        public static void DrawRectangle(Float3 _A, Float3 _B, Float3 _C, Float3 _D)
        {
            DrawRectangle(_A, _B, _C, _D, new Float2(0, 0), new Float2(1, 0), new Float2(0, 1), new Float2(1, 1));
        }

        /// <summary>
        /// draws a rectangle using the provided image to the active render target
        /// </summary>
        /// <param name="_A">Left upper Coordinate</param>
        /// <param name="_B">Right upper Coordinate</param>
        /// <param name="_C">Left lower Coordinate</param>
        /// <param name="_D">Right lower Coordinate</param>
        /// <param name="_Image"></param>
        public static void DrawRectangle(Float3 _A, Float3 _B, Float3 _C, Float3 _D, Float2 _UVA, Float2 _UVB, Float2 _UVC, Float2 _UVD)
        {
            device.RenderState.DepthBufferEnable = false;
            device.RenderState.CullMode = CullMode.None;

            VertexBuffer vbo = new VertexBuffer(Graphics.device, Marshal.SizeOf(typeof(Vertex)) * 4, BufferUsage.None);
            VertexPositionTexture[] vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(_A.X,_A.Y,_A.Z), new Vector2(_UVA.X,_UVA.Y)),
                new VertexPositionTexture(new Vector3(_B.X,_B.Y,_B.Z), new Vector2(_UVB.X,_UVB.Y)),
                new VertexPositionTexture(new Vector3(_C.X,_C.Y,_C.Z), new Vector2(_UVC.X,_UVC.Y)),
                new VertexPositionTexture(new Vector3(_D.X,_D.Y,_D.Z), new Vector2(_UVD.X,_UVD.Y))
            };
            vbo.SetData(vertices);
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }

        /// <summary>
        /// draws a rectangle using the provided image to the active render target
        /// </summary>
        /// <param name="_A">Left upper Coordinate</param>
        /// <param name="_B">Right upper Coordinate</param>
        /// <param name="_C">Left lower Coordinate</param>
        /// <param name="_D">Right lower Coordinate</param>
        /// <param name="_Image"></param>
        public static void DrawRectangle(Float2 _A, Float2 _B, Float2 _C, Float2 _D, Float2 _UVA, Float2 _UVB, Float2 _UVC, Float2 _UVD, float _Depth)
        {
            DrawRectangle(
                new Float3(_A.X, _A.Y, _Depth),
                new Float3(_B.X, _B.Y, _Depth),
                new Float3(_C.X, _C.Y, _Depth),
                new Float3(_D.X, _D.Y, _Depth),
                _UVA,
                _UVB,
                _UVC,
                _UVD);
        }

        /// <summary>
        /// draws a rectangle using the provided image to the active render target
        /// </summary>
        /// <param name="_A">Left upper Coordinate</param>
        /// <param name="_B">Right upper Coordinate</param>
        /// <param name="_C">Left lower Coordinate</param>
        /// <param name="_D">Right lower Coordinate</param>
        /// <param name="_Depth">Right lower Coordinate</param>
        /// <param name="_Image"></param>
        public static void DrawRectangle(Float2 _A, Float2 _B, Float2 _C, Float2 _D, float _Depth)
        {
            DrawRectangle(
                new Float3(_A.X, _A.Y, _Depth),
                new Float3(_B.X, _B.Y, _Depth),
                new Float3(_C.X, _C.Y, _Depth),
                new Float3(_D.X, _D.Y, _Depth));
        }
    }
}
