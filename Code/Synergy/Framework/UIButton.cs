using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Synergy
{
    public class UIButton : Control
    {
        Shader shader = null;
        Shader loadingshader = null;
        TextureGPUResource buttonon = new TextureGPUResource("./content/buttonon.png");
        TextureGPUResource buttonoff = new TextureGPUResource("./content/buttonoff.png");
        public string Text;
        SpriteFont font = null;
        public bool Checked;
        public UIButton(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            Transformation = new Float3x3(0.1f, 0, 0, 0, 0.1f, 0, 0, 0, 0);
            shader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Default.fx"));
            loadingshader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Loading.fx"));

            font = new SpriteFont("./content/Arial.png", "./content/Arial.xml");

            OnControllerInput += new OnControllerInputHandler(UIButton_OnControllerInput);
        }

        bool UIButton_OnControllerInput(UITouchEvent _Event)
        {
            Float3 mouse = new Float3(_Event.Position, 1);
            Float3x3 mat = GetTransformation().Invert();
            Float2 localmouse = (mouse * mat).XY;
            if (localmouse.Length() < 1 && _Event.Released)
            {
                Checked = !Checked;
                return true;
            }
            return false;
        }

        public override void Draw()
        {
            Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
            TextureGPU img = Checked ? (TextureGPU)buttonoff.Get() : (TextureGPU)buttonon.Get();
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
                    new Float2(0, 0),
                    new Float2(0, 1),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                Graphics.DrawLine(
                    new Float2(0, 0),
                    new Float2(1, 0),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                Graphics.DrawLine(
                    new Float2(1, 1),
                    new Float2(0, 1),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                Graphics.DrawLine(
                    new Float2(1, 1),
                    new Float2(1, 0),
                    new Float4(1, 0, 0, 1),
                    new Float4(1, 0, 0, 1), 4);

                loadingshader.End();
            }
        }
    }
}