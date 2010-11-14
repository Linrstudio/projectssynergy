using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synergy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Synergy
{

    public class Shader
    {
        public Effect shader;

        public void SetTechnique(string _TechniqueName)
        {
            shader.CurrentTechnique = shader.Techniques[_TechniqueName];
        }

        public void SetParameter(string _FieldName, TextureGPU _Texture)
        {
            if(_Texture!=null)
                shader.Parameters[_FieldName].SetValue(_Texture.texture);
            else
                shader.Parameters[_FieldName].SetValue((Texture)null);
        }

        public void SetParameter(string _FieldName, float _Float)
        {
            shader.Parameters[_FieldName].SetValue(_Float);
        }

        public void SetParameter(string _FieldName, Float2 _Float2)
        {
            shader.Parameters[_FieldName].SetValue(new Vector2(_Float2.X, _Float2.Y));
        }

        public void SetParameter(string _FieldName, Float3 _Float3)
        {
            shader.Parameters[_FieldName].SetValue(new Vector3(_Float3.X, _Float3.Y, _Float3.Z));
        }

        public void SetParameter(string _FieldName, Float4 _Float4)
        {
            shader.Parameters[_FieldName].SetValue(new Vector4(_Float4.X, _Float4.Y, _Float4.Z, _Float4.W));
        }

        public void SetParameter(string _FieldName, Float3x3 _Value)
        {
            shader.Parameters[_FieldName].SetValue(new float[]
            {
                _Value.X.X,_Value.X.Y,_Value.X.Z,
                _Value.Y.X,_Value.Y.Y,_Value.Y.Z,
                _Value.T.X,_Value.T.Y,_Value.T.Z
            });
        }
        /*
        public void SetParameter(string _FieldName, Float4x4 _Value)
        {
            shader.Parameters[_FieldName].SetValue(
                new float[]{
                _Value.X.X,_Value.X.Y,_Value.X.Z,_Value.X.W,
                _Value.Y.X,_Value.Y.Y,_Value.Y.Z,_Value.Y.W,
                _Value.Z.X,_Value.Z.Y,_Value.Z.Z,_Value.Z.W,
                _Value.T.X,_Value.T.Y,_Value.T.Z,_Value.T.W
                });
        }
        */
        public void Begin()
        {
            shader.Begin();
            shader.CurrentTechnique.Passes[0].Begin();
        }

        public void End()
        {
            shader.CurrentTechnique.Passes[0].End();
            shader.End();
        }
    }

    public class ShaderCompiler
    {
        public static Shader Compile(string _Source)
        {
            CompilerOptions options = CompilerOptions.None;

            CompiledEffect shader = Effect.CompileEffectFromSource(_Source, null, null, options, TargetPlatform.Windows);

            if (!shader.Success)
            {
                System.Windows.Forms.MessageBox.Show(shader.ErrorsAndWarnings);
                //Log.Write("ShaderCompiler", Log.Line.Type.Error, "Failed to compile shader");
                Log.Write("ShaderCompiler", Log.Line.Type.Error, shader.ErrorsAndWarnings);
                return null;
            }
            else
            {
                Shader ret = new Shader();
                ret.shader = new Effect(Graphics.device, shader.GetEffectCode(), options, null);
                return ret;
            }
        }
    }
}
