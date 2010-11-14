using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Synergy;

namespace SynergyClient
{
    public class UIFloor : Control
    {
        public int level = 0;
        public UIFloor(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {

        }

        public override void Update()
        {
            Transformation = Float3x3.Rotate((float)Math.PI/4) * Float3x3.Scale(new Float2(0.5f, 0.2f)) * Float3x3.Translate(new Float2(0, ((float)level-2) / 4.0f));
            base.Update();
        }


        public override void Draw()
        {
            Graphics.SetBlendMode(Graphics.BlendMode.None);

            Graphics.defaultshader.SetParameter("View", GetTransformation());
            Graphics.defaultshader.Begin();
            Graphics.DrawRectangle(
                new Float2(-1, -1),
                new Float2(1, -1),
                new Float2(-1, 1),
                new Float2(1, 1), 0.5f);
            Graphics.defaultshader.End();

            base.Draw();
        }
    }
}
