using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Synergy
{
    public class UIBackground : Control
    {
        Shader shader = null;
        TextureGPUResource image = new TextureGPUResource("./content/background.png");
        public UIBackground(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            Transformation = new Float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
            shader = ShaderCompiler.Compile(System.IO.File.ReadAllText("Default.fx"));
            OnControllerInput += new OnControllerInputHandler(UIButton_OnControllerInput);
        }

        bool UIButton_OnControllerInput(UITouchEvent _Event)
        {
            return false;
        }

        public override void Draw()
        {
            Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
            shader.SetParameter("View", GetTransformation());
            TextureGPU img = (TextureGPU)image.Get();
            if (img != null)
            {
                shader.SetParameter("DiffuseMap", img);
                shader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(1, -1),
                    new Float2(-1, 1),
                    new Float2(1, 1), 0.5f);
                shader.End();
            }
            DrawChildren();
        }
    }
}