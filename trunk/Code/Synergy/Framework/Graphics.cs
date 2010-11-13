using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Synergy;

namespace Synergy
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Float3 Position;
        public Float2 UV;
        byte R;
        byte G;
        byte B;
        byte A;
        public Float4 Color
        {
            set
            {
                R = (byte)(((uint)(value.X * 255)) & 255);
                G = (byte)(((uint)(value.Y * 255)) & 255);
                B = (byte)(((uint)(value.Z * 255)) & 255);
                A = (byte)(((uint)(value.W * 255)) & 255);
            }
            get
            {
                return new Float4(((float)R) / 255, ((float)G) / 255, ((float)B) / 255, ((float)A) / 255);
            }
        }

        public static VertexElement[] VertexElements
        {
            get
            {
                return new VertexElement[]
                {
                    new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                    new VertexElement(0, 12, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement(0, 20, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)
                };
            }
        }

        public Vertex(Float3 _Position, Float2 _UV)
        {
            Position = _Position;
            UV = _UV;
            R = G = B = A = 255;
        }

        public Vertex(Float3 _Position, Float2 _UV, Float4 _Color)
        {
            Position = _Position;
            UV = _UV;
            R = G = B = A = 0;
            Color = _Color;
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

        public static void SetScissorRect(bool _Enabled)
        {
            device.RenderState.ScissorTestEnable = _Enabled;
        }

        public static void SetScissorRect(Float2 _From, Float2 _To)
        {
            SetScissorRect(
                new Int2((int)(_From.X * device.Viewport.Width), (int)(_From.Y * device.Viewport.Height)),
                new Int2((int)(_To.X * device.Viewport.Width), (int)(_To.Y * device.Viewport.Height)));
        }

        public static void SetScissorRect(Int2 _From, Int2 _To)
        {
            device.ScissorRectangle = new Rectangle(_From.X, _From.Y, _To.X, _To.Y);
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

        public enum BlendMode
        {
            None, Alpha, Add
        }

        public static void SetBlendMode(BlendMode _Mode)
        {
            device.RenderState.AlphaBlendEnable = (_Mode != BlendMode.None);

            switch (_Mode)
            {
                case BlendMode.Alpha:
                    device.RenderState.SeparateAlphaBlendEnabled = true;
                    device.RenderState.AlphaBlendOperation = BlendFunction.Max;
                    device.RenderState.BlendFunction = BlendFunction.Add;
                    device.RenderState.SourceBlend = Blend.SourceAlpha;
                    device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                    break;
                case BlendMode.Add:
                    device.RenderState.SeparateAlphaBlendEnabled = false;
                    device.RenderState.AlphaBlendOperation = BlendFunction.Add;
                    device.RenderState.AlphaDestinationBlend = Blend.DestinationAlpha;
                    device.RenderState.BlendFunction = BlendFunction.Add;
                    device.RenderState.SourceBlend = Blend.SourceColor;
                    device.RenderState.DestinationBlend = Blend.DestinationColor;
                    break;
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
            Vertex[] vertices = new Vertex[]
            {
                new Vertex(_A, _UVA),
                new Vertex(_B, _UVB),
                new Vertex(_C, _UVC),
                new Vertex(_D, _UVD)
            };
            vbo.SetData(vertices);
            device.VertexDeclaration = new VertexDeclaration(device, Vertex.VertexElements);
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
        /// <param name="_Depth">Depth</param>
        /// <param name="_Image"></param>
        public static void DrawRectangle(Float2 _A, Float2 _B, Float2 _C, Float2 _D, float _Depth)
        {
            DrawRectangle(
                new Float3(_A.X, _A.Y, _Depth),
                new Float3(_B.X, _B.Y, _Depth),
                new Float3(_C.X, _C.Y, _Depth),
                new Float3(_D.X, _D.Y, _Depth));
        }

        public static void DrawLine(Float2 _A, Float2 _B, Float4 _ColorA, Float4 _ColorB, float _Thickness)
        {
            device.RenderState.DepthBufferEnable = false;

            VertexBuffer vbo = new VertexBuffer(Graphics.device, Marshal.SizeOf(typeof(Vertex)) * 2, BufferUsage.None);
            VertexPositionColor[] vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(_A.X,_A.Y,0),new Color(new Vector4(_ColorA.X,_ColorA.Y,_ColorA.Z,_ColorA.W))),
                new VertexPositionColor(new Vector3(_B.X,_B.Y,0),new Color(new Vector4(_ColorB.X,_ColorB.Y,_ColorB.Z,_ColorB.W)))
            };
            vbo.SetData(vertices);
            device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
            device.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, 1);
        }
    }
}
