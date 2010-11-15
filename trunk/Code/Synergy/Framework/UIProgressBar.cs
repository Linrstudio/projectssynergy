using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synergy
{
    public class UIProgressBar : Control
    {
        TextureGPUResource image = new TextureGPUResource("./content/ProgressBar.png");
        TextureGPUResource background = new TextureGPUResource("./content/ProgressBarBackground.png");

        int val = 0;
        public int Value
        {
            get { return val; }
            set { val = Math.Max(ValueMin, Math.Min(ValueMax, value)); }
        }
        public int ValueMin = 0;
        public int ValueMax = 100;

        public UIProgressBar(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {

            OnControllerInput += new OnControllerInputHandler(UIButton_OnControllerInput);
        }

        bool UIButton_OnControllerInput(UITouchEvent _Event)
        {
            Float3 mouse = new Float3(_Event.Position, 1);
            Float3x3 mat = GetTransformation();
            Float3x3 matinv = mat.Invert();
            Float2 localmouse = (mouse * matinv).XY;
            Value = (int)((localmouse.X * 0.5f + 0.5f) * 100);
            return false;
        }

        public override void Draw()
        {
            float p = (float)(Value - ValueMin) / (float)(ValueMax - ValueMin);
            float s = 0.1f;
            float bgs = 0.1f;
            if (p < s) s = p / 2;

            TextureGPU img = (TextureGPU)background.Get();
            if (img != null)
            {
                Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
                //background
                Graphics.defaultshader.SetParameter("View", GetTransformation());
                Graphics.defaultshader.SetParameter("DiffuseMap", img);
                Graphics.defaultshader.Begin();
                //left piece
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(-1 + bgs * 2, -1),
                    new Float2(-1, 1),
                    new Float2(-1 + bgs * 2, 1),

                    new Float2(0, 0),
                    new Float2(0.5f, 0),
                    new Float2(0, 1),
                    new Float2(0.5f, 1),
                    0.5f);

                //middle piece
                Graphics.DrawRectangle(
                    new Float2(-1 + bgs * 2, -1),
                    new Float2(1 - bgs * 2, -1),
                    new Float2(-1 + bgs * 2, 1),
                    new Float2(1 - bgs * 2, 1),

                    new Float2(0.5f, 0),
                    new Float2(0.5f, 0),
                    new Float2(0.5f, 1),
                    new Float2(0.5f, 1),
                    0.5f);

                //right piece
                Graphics.DrawRectangle(
                    new Float2(1 - bgs * 2, -1),
                    new Float2(1, -1),
                    new Float2(1 - bgs * 2, 1),
                    new Float2(1, 1),

                    new Float2(0.5f, 0),
                    new Float2(1, 0),
                    new Float2(0.5f, 1),
                    new Float2(1, 1),
                    0.5f);
                Graphics.defaultshader.End();
            }
            img = (TextureGPU)image.Get();
            if (img != null)
            {
                Graphics.SetBlendMode(Graphics.BlendMode.None);
                Graphics.defaultshader.SetParameter("View", GetTransformation());
                Graphics.defaultshader.SetParameter("DiffuseMap", img);
                Graphics.defaultshader.Begin();
                //left piece
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(-1 + s * 2, -1),
                    new Float2(-1, 1),
                    new Float2(-1 + s * 2, 1),

                    new Float2(0, 0),
                    new Float2(0.5f, 0),
                    new Float2(0, 1),
                    new Float2(0.5f, 1),
                    0.5f);

                //middle piece
                Graphics.DrawRectangle(
                    new Float2(-1 + s * 2, -1),
                    new Float2(-1 + p * 2 - s * 2, -1),
                    new Float2(-1 + s * 2, 1),
                    new Float2(-1 + p * 2 - s * 2, 1),

                    new Float2(0.5f, 0),
                    new Float2(0.5f, 0),
                    new Float2(0.5f, 1),
                    new Float2(0.5f, 1),
                    0.5f);

                //right piece
                Graphics.DrawRectangle(
                    new Float2(-1 + p * 2 - s * 2, -1),
                    new Float2(-1 + p * 2, -1),
                    new Float2(-1 + p * 2 - s * 2, 1),
                    new Float2(-1 + p * 2, 1),

                    new Float2(0.5f, 0),
                    new Float2(1, 0),
                    new Float2(0.5f, 1),
                    new Float2(1, 1),
                    0.5f);
                Graphics.defaultshader.End();
            }
            //Graphics.defaultshader.SetParameter("View", GetTransformation());
            //font.Draw(new Float2(-1, 1), 2, string.Format("{0}", Value));
        }
    }
}
