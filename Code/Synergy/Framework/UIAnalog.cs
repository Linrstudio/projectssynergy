using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Synergy
{
    public class UIAnalog : Control
    {
        Shader shader = null;
        Shader loadingshader = null;
        TextureGPUResource buttonon = new TextureGPUResource("./content/buttonon.png");
        TextureGPUResource buttonoff = new TextureGPUResource("./content/buttonoff.png");
        public string Text;
        SpriteFont font = null;
        bool red;
        bool zoomed = false;
        float zoomstate = 0;
        Float3x3 ZoomedTransformation;
        Float3x3 NormalTransformation;
        public UIAnalog(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            Transformation = new Float3x3(0.1f, 0, 0, 0, 0.1f, 0, 0, 0, 1);
            ZoomedTransformation = new Float3x3(0.9f, 0, 0, 0, 0.9f, 0, 0, 0, 1);
            shader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Default.fx"));
            loadingshader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Loading.fx"));

            font = new SpriteFont("./content/Arial.png", "./content/Arial.xml");
            NormalTransformation = Transformation;
            OnControllerInput += new OnControllerInputHandler(UIButton_OnControllerInput);
        }

        bool UIButton_OnControllerInput(UITouchEvent _Event)
        {
            Float3 mouse = new Float3(_Event.Position, 1);
            Float3x3 mat = GetTransformation();
            Float3x3 matinv = mat.Invert();
            Float2 localmouse = (mouse * matinv).XY;

            if (zoomed)
            {
                if (localmouse.Length() > 1 && _Event.Released)
                {
                    zoomed = false;
                    return true;
                }
                else
                {
                    if (_Event.Released)
                    {
                        red = !red;
                        return true;
                    }
                }
            }
            else
            {
                if (localmouse.Length() < 1 && zoomstate < 0.1f && _Event.Released)
                {
                    NormalTransformation = Transformation;
                    RelativeToParent = false;
                    zoomed = true;
                    BringToFront();
                    return true;
                }
            }
            return false;
        }

        public override void Draw()
        {
            if (zoomed)
            {
                if (zoomstate < 1)
                {
                    zoomstate += 0.04f;
                    float t = (float)Math.Sin((zoomstate - 0.5) * Math.PI) * 0.5f + 0.5f;
                    Transformation = Float3x3.Interpolate(NormalTransformation * Parent.Transformation, ZoomedTransformation, t);
                }
                else if (zoomstate != 1) { zoomstate = 1; }
            }
            else
            {
                if (zoomstate > 0)
                {
                    zoomstate -= 0.04f;
                    float t = (float)Math.Sin((zoomstate - 0.5) * Math.PI) * 0.5f + 0.5f;
                    Transformation = Float3x3.Interpolate(NormalTransformation * Parent.Transformation, ZoomedTransformation, t);
                }
                else if (zoomstate != 0) { zoomstate = 0; Transformation = NormalTransformation; RelativeToParent = true; }
            }

            Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
            TextureGPU img = red ? (TextureGPU)buttonoff.Get() : (TextureGPU)buttonon.Get();
            if (img != null)
            {
                shader.SetParameter("View", GetTransformation());
                shader.SetParameter("DiffuseMap", img);
                shader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(1, -1),
                    new Float2(-1, 1),
                    new Float2(1, 1), 0.5f);
                shader.End();
                Graphics.defaultshader.SetParameter("View", GetTransformation());
                font.Draw(new Float2(-1, 1), 2, Text);
            }
            else
            {
                loadingshader.SetParameter("View", GetTransformation());
                loadingshader.Begin();
                Graphics.DrawLine(
                    new Float2(-1, -1),
                    new Float2(-1, 1),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                Graphics.DrawLine(
                    new Float2(-1, -1),
                    new Float2(1, -1),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                Graphics.DrawLine(
                    new Float2(1, 1),
                    new Float2(-1, 1),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                Graphics.DrawLine(
                    new Float2(1, 1),
                    new Float2(1, -1),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                loadingshader.End();
            }
        }
    }
}