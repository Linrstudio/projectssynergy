using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyTemplate
{
    public class Float2x3
    {
        public Float2 X;
        public Float2 Y;
        public Float2 T;

        public Float2x3() { X = new Float2(1, 0); Y = new Float2(0, 1); T = new Float2(0, 0); }

        public Float2x3(float _XX, float _XY, float _YX, float _YY, float _TX, float _TY)
        {
            X = new Float2(_XX, _XY);
            Y = new Float2(_YX, _YY);
            T = new Float2(_TX, _TY);
        }

        public Float2x3(Float2 _X, Float2 _Y, Float2 _T)
        {
            X = _X;
            Y = _Y;
            T = _T;
        }

        public static Float2x3 operator *(Float2x3 _A, Float2x3 _B)
        {
            Float2x3 ret = new Float2x3(
                _A.TransformNormal(_B.X),
                _A.TransformNormal(_B.Y),
                _A.Transform(_B.T));
            return ret;
        }

        public static Float2 operator*(Float2x3 _M,Float2 _V)
        {
            return _M.Transform(_V);
        }

        public Float2 TransformNormal(Float2 _V)
        {
            return new Float2(Float2.Dot(_V, X), Float2.Dot(_V, Y));
        }

        public Float2 Transform(Float2 _V)
        {
            return new Float2(Float2.Dot(_V, X), Float2.Dot(_V, Y)) + T;
        }
    }
}
