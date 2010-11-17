using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Synergy
{
    public class Float3x3
    {
        public Float3 X, Y, T;
        public Float3[] Column
        {
            get
            {
                return new Float3[]
                {
                    new Float3(X.X,Y.X,T.X),
                    new Float3(X.Y,Y.Y,T.Y),
                    new Float3(X.Z,Y.Z,T.Z)
                };
            }
        }

        public Float3[] Row
        {
            get
            {
                return new Float3[]
                {
                    new Float3(X.X,X.Y,X.Z),
                    new Float3(Y.X,Y.Y,Y.Z),
                    new Float3(T.X,T.Y,T.Z)
                };
            }
        }

        public Float3x3() { X = new Float3(1, 0, 0); Y = new Float3(0, 1, 0); T = new Float3(0, 0, 1); }

        public Float3x3(float _XX, float _XY, float _XZ, float _YX, float _YY, float _YZ, float _TX, float _TY, float _TZ)
        {
            X = new Float3(_XX, _XY, _XZ);
            Y = new Float3(_YX, _YY, _YZ);
            T = new Float3(_TX, _TY, _TZ);
        }

        public Float3x3(Float3 _X, Float3 _Y, Float3 _T)
        {
            X = _X;
            Y = _Y;
            T = _T;
        }
        
        public static Float3x3 operator *(Float3x3 _A, Float3x3 _B)
        {
            // THIS ONE WORKS
            Float3x3 ret = new Float3x3(
                Float3.Dot(_A.Row[0], _B.Column[0]), Float3.Dot(_A.Row[0], _B.Column[1]), Float3.Dot(_A.Row[0], _B.Column[2]),
                Float3.Dot(_A.Row[1], _B.Column[0]), Float3.Dot(_A.Row[1], _B.Column[1]), Float3.Dot(_A.Row[1], _B.Column[2]),
                Float3.Dot(_A.Row[2], _B.Column[0]), Float3.Dot(_A.Row[2], _B.Column[1]), Float3.Dot(_A.Row[2], _B.Column[2]));
            return ret;
        }

        public static Float3 operator *(Float3 _V, Float3x3 _M)
        {
            Float3 ret = new Float3(
                Float3.Dot(_V, _M.Column[0]),
                Float3.Dot(_V, _M.Column[1]),
                Float3.Dot(_V, _M.Column[2]));
            return ret;
        }

        public Float3x3 Invert()
        {
            Matrix m = new Matrix(
                X.X, X.Y, 0, X.Z,
                Y.X, Y.Y, 0, Y.Z,
                0, 0, 1, 0,
                T.X, T.Y, 0, T.Z);

            //m = Matrix.Transpose(Matrix.Invert(Matrix.Transpose(m)));
            m = Matrix.Invert(m);

            Float3x3 r = new Float3x3();
            r.X.X = m.M11;
            r.X.Y = m.M12;
            r.X.Z = m.M14;

            r.Y.X = m.M21;
            r.Y.Y = m.M22;
            r.Y.Z = m.M24;

            r.T.X = m.M41;
            r.T.Y = m.M42;
            r.T.Z = m.M44;

            return r;
        }

        public static Float3x3 Rotate(float _Radians) { return new Float3x3((float)Math.Cos(_Radians), (float)Math.Sin(_Radians), 0, (float)-Math.Sin(_Radians), (float)Math.Cos(_Radians), 0, 0, 0, 1); }
        public static Float3x3 Translate(Float2 _Translation) { return new Float3x3(1, 0, 0, 0, 1, 0, _Translation.X, _Translation.Y, 1); }
        public static Float3x3 Scale(float _Scale) { return new Float3x3(_Scale, 0, 0, 0, _Scale, 0, 0, 0, 1); }
        public static Float3x3 Scale(Float2 _Scale) { return new Float3x3(_Scale.X, 0, 0, 0, _Scale.Y, 0, 0, 0, 1); }

        public static Float3x3 Interpolate(Float3x3 _A, Float3x3 _B, float _T)
        {
            float t = _T;
            float t1 = 1 - t;
            Float3x3 result = new Float3x3(
             _A.X * t1 + _B.X * t,
             _A.Y * t1 + _B.Y * t,
             _A.T * t1 + _B.T * t);
            return result;
        }

        public static Float3x3 Identity { get { return new Float3x3(1, 0, 0, 0, 1, 0, 0, 0, 1); } }
    }
#if false
    public class Float4x4
    {
        public Float4 X, Y, Z, T;

        public Float4[] Column
        {
            get
            {
                return new Float4[]
                {
                    new Float4(X.X,Y.X,Z.X,T.X),
                    new Float4(X.Y,Y.Y,Z.Y,T.Y),
                    new Float4(X.Z,Y.Z,Z.Z,T.Z),
                    new Float4(X.W,Y.W,Z.W,T.W)
                };
            }
        }

        public Float4[] Row
        {
            get
            {
                return new Float4[]
                {
                    new Float4(X.X,X.Y,X.Z,X.W),
                    new Float4(Y.X,Y.Y,Y.Z,Y.W),
                    new Float4(Z.X,Z.Y,Z.Z,Z.W),
                    new Float4(T.X,T.Y,T.Z,T.W)
                };
            }
        }

        public Float4x4()
        { X = new Float4(1, 0, 0, 0); Y = new Float4(0, 1, 0, 0); Z = new Float4(0, 0, 1, 0); T = new Float4(0, 0, 0, 1); }

        public Float4x4(Float4 _X, Float4 _Y, Float4 _Z, Float4 _T)
        { X = _X; Y = _Y; Z = _Z; T = _T; }

        public Float4x4(float _XX, float _XY, float _XZ, float _XW, float _YX, float _YY, float _YZ, float _YW, float _ZX, float _ZY, float _ZZ, float _ZW, float _TX, float _TY, float _TZ, float _TW)
        {
            X.X = _XX; X.Y = _XY; X.Z = _XZ; X.W = _XW;
            Y.X = _YX; Y.Y = _YY; Y.Z = _YZ; Y.W = _YW;
            Z.X = _ZX; Z.Y = _ZY; Z.Z = _ZZ; Z.W = _ZW;
            T.X = _TX; T.Y = _TY; T.Z = _TZ; T.W = _TW;
        }

        public static Float4x4 operator *(Float4x4 _A, Float4x4 _B)
        {
            // THIS ONE WORKS
            Float4x4 ret = new Float4x4(
                Float4.Dot(_B.Row[0], _A.Column[0]), Float4.Dot(_B.Row[0], _A.Column[1]), Float4.Dot(_B.Row[0], _A.Column[2]), Float4.Dot(_B.Row[0], _A.Column[3]),
                Float4.Dot(_B.Row[1], _A.Column[0]), Float4.Dot(_B.Row[1], _A.Column[1]), Float4.Dot(_B.Row[1], _A.Column[2]), Float4.Dot(_B.Row[1], _A.Column[3]),
                Float4.Dot(_B.Row[2], _A.Column[0]), Float4.Dot(_B.Row[2], _A.Column[1]), Float4.Dot(_B.Row[2], _A.Column[2]), Float4.Dot(_B.Row[2], _A.Column[3]),
                Float4.Dot(_B.Row[3], _A.Column[0]), Float4.Dot(_B.Row[3], _A.Column[1]), Float4.Dot(_B.Row[3], _A.Column[2]), Float4.Dot(_B.Row[3], _A.Column[3]));
            return ret;
        }

        public static Float4 operator *(Float4 _V, Float4x4 _M)
        {
            Float4 ret = new Float4(
                Float4.Dot(_V, _M.Row[0]),
                Float4.Dot(_V, _M.Row[1]),
                Float4.Dot(_V, _M.Row[2]),
                Float4.Dot(_V, _M.Row[3]));
            return ret;
        }

        public static Float4x4 RotateX(float _Radians)
        {
            float cos = (float)Math.Cos(_Radians);
            float sin = (float)Math.Sin(_Radians);
            return new Float4x4(1, 0, 0, 0, 0, cos, -sin, 0, 0, sin, cos, 0, 0, 0, 0, 1);
        }
        public static Float4x4 RotateY(float _Radians)
        {
            float cos = (float)Math.Cos(_Radians);
            float sin = (float)Math.Sin(_Radians);
            return new Float4x4(cos, 0, sin, 0, 0, 1, 0, 0, -sin, 0, cos, 0, 0, 0, 0, 1);
        }
        public static Float4x4 RotateZ(float _Radians)
        {
            float cos = (float)Math.Cos(_Radians);
            float sin = (float)Math.Sin(_Radians);
            return new Float4x4(cos, -sin, 0, 0, sin, cos, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
        }
        public static Float4x4 Translate(Float3 _Translation) { return new Float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, _Translation.X, _Translation.Y, _Translation.Z, 1); }
        public static Float4x4 Scale(float _Scale) { return new Float4x4(_Scale, 0, 0, 0, 0, _Scale, 0, 0, 0, 0, _Scale, 0, 0, 0, 0, 1); }
        public static Float4x4 Scale(Float3 _Scale) { return new Float4x4(_Scale.X, 0, 0, 0, 0, _Scale.Y, 0, 0, 0, 0, _Scale.Z, 0, 0, 0, 0, 1); }
    }
#endif
}
