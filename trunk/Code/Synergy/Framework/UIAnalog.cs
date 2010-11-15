using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Synergy
{
    public class UIAnalog : Control
    {
        TextureGPUResource background = new TextureGPUResource("./content/backplateButton.png");
        TextureGPUResource knob = new TextureGPUResource("./content/newbutton.png");
        public string Text;
        SpriteFont font = null;
        public float Value = 0.0f;
        bool zoomed = false;
        float OffsetValue;
        float OffsetAngle;
        float zoomstate = 0;
        Float3x3 ZoomedTransformation;
        Float3x3 NormalTransformation;
        public UIAnalog(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            Transformation = new Float3x3(0.1f, 0, 0, 0, 0.1f, 0, 0, 0, 1);
            ZoomedTransformation = new Float3x3(0.9f, 0, 0, 0, 0.9f, 0, 0, 0, 1);

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
                    if (_Event.Pressed)
                    {
                        OffsetAngle = -(float)Math.Atan2(localmouse.Y, localmouse.X);
                        OffsetValue = Value - OffsetAngle / ((float)Math.PI * 2);
                    }

                    float angle = -(float)Math.Atan2(localmouse.Y, localmouse.X);

                    float shortestangle = angle - OffsetAngle;
                    if (shortestangle > Math.PI) shortestangle = ((float)Math.PI * 2) - shortestangle;
                    if (shortestangle < -Math.PI) shortestangle = ((float)Math.PI * 2) + shortestangle;

                    Value += shortestangle / ((float)Math.PI * 2);

                    OffsetAngle = angle;

                    if (Value > 1) Value = 1f;
                    if (Value < 0) Value = 0;
                    return true;
                }
            }
            else
            {
                if (localmouse.Length() < 1 && _Event.Released)
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
            //blaat

            Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
            TextureGPU img = (TextureGPU)background.Get();

            if (img != null)
            {
                Graphics.defaultshader.SetParameter("View", GetTransformation());
                Graphics.defaultshader.SetParameter("DiffuseMap", img);
                Graphics.defaultshader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(1, -1),
                    new Float2(-1, 1),
                    new Float2(1, 1), 0.5f);
                Graphics.defaultshader.End();

            }

            img = (TextureGPU)knob.Get();

            float angle = Value * (float)Math.PI * 2;
            if (img != null)
            {
                Graphics.defaultshader.SetParameter("View", Float3x3.Rotate(-angle) * GetTransformation());
                Graphics.defaultshader.SetParameter("DiffuseMap", img);
                Graphics.defaultshader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(1, -1),
                    new Float2(-1, 1),
                    new Float2(1, 1), 0.5f);
                Graphics.defaultshader.End();
                Graphics.defaultshader.SetParameter("View", GetTransformation());
                font.Draw(new Float2(-1, 1), 2, string.Format("{0}", Value));
            }

        }
    }
}
