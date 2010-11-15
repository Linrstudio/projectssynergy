using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Synergy;

namespace SynergyClient
{
    public class UIFloor : Control
    {
        TextureGPUResource image = new TextureGPUResource("./content/EmptyFloor.png");
        public int Level = 0;
        public float Rotation = 0;

        Float3x3 OverviewTransform;
        Float3x3 ZoomedTransform;

        bool zoomed = false;
        float zoomstate = 0;

        public UIFloor(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            RelativeToParent = false;
            ZoomedTransform = new Float3x3(0.9f, 0, 0, 0, 0.9f, 0, 0, 0, 1);
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
                    HandleChildInputFirst = false;
                    return true;
                }
                else
                {
                    if (_Event.Released)
                    {
                        //handle stuff here

                        return true;
                    }
                }
            }
            else
            {
                if (localmouse.Length() < 1 && zoomstate < 0.1f && _Event.Released&&!zoomed)
                {
                    zoomed = true;
                    HandleChildInputFirst = true;
                    BringToFront();
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            OverviewTransform = Float3x3.Rotate(Rotation) * Float3x3.Scale(new Float2(0.5f, 0.2f)) * Float3x3.Translate(new Float2(0, ((float)Level - 2) / 4.0f));
            base.Update();
        }


        public override void Draw()
        {
            if (zoomed)
            {
                if (zoomstate <= 1)
                {
                    zoomstate += 0.04f;
                    float t = (float)Math.Sin((zoomstate - 0.5) * Math.PI) * 0.5f + 0.5f;
                    Transformation = Float3x3.Interpolate(OverviewTransform * Parent.Transformation, ZoomedTransform, t);
                }
                else if (zoomstate != 1) { zoomstate = 1; }
            }
            else
            {
                if (zoomstate >= 0)
                {
                    zoomstate -= 0.04f;
                    float t = (float)Math.Sin((zoomstate - 0.5) * Math.PI) * 0.5f + 0.5f;
                    Transformation = Float3x3.Interpolate(OverviewTransform * Parent.Transformation, ZoomedTransform, t);
                }
                else zoomstate = 0;
            }


            Graphics.SetBlendMode(Graphics.BlendMode.Alpha);
            TextureGPU img = (TextureGPU)image.Get();
            if (img != null)
            {
                Graphics.defaultshader.SetParameter("DiffuseMap", img);
                Graphics.defaultshader.SetParameter("View", GetTransformation());
                Graphics.defaultshader.Begin();
                Graphics.DrawRectangle(
                    new Float2(-1, -1),
                    new Float2(1, -1),
                    new Float2(-1, 1),
                    new Float2(1, 1), 0.5f);
                Graphics.defaultshader.End();
            }
            base.Draw();
        }
    }
}
