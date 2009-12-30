using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynergyTemplate
{
    public struct Int2
    {
        public int X;
        public int Y;

        public Int2(int _X, int _Y)
        {
            X = _X; Y = _Y;
        }

        public static bool operator <(Int2 _A, Int2 _B)
        {
            return _A.X < _B.X && _A.Y < _B.Y;
        }
        public static bool operator >(Int2 _A, Int2 _B)
        {
            return _A.X > _B.X && _A.Y > _B.Y;
        }

        public static Int2 operator +(Int2 _A, Int2 _B)
        {
            return new Int2(_A.X + _B.X, _A.Y + _B.Y);
        }

        public static Int2 operator -(Int2 _A, Int2 _B)
        {
            return new Int2(_A.X - _B.X, _A.Y - _B.Y);
        }

        public static Int2 operator *(Int2 _A, Int2 _B)
        {
            return new Int2(_A.X * _B.X, _A.Y * _B.Y);
        }

        public static Int2 operator *(Int2 _A, int _B)
        {
            return new Int2(_A.X * _B, _A.Y * _B);
        }

        public static Int2 operator /(Int2 _A, Int2 _B)
        {
            return new Int2(_A.X / _B.X, _A.Y / _B.Y);
        }

        public static Int2 operator /(Int2 _A, int _B)
        {
            return new Int2(_A.X / _B, _A.Y / _B);
        }

        public static Int2 operator -(Int2 _A)
        {
            return new Int2(-_A.X, -_A.Y);
        }
    }

    public struct Float2
    {
        public float X;
        public float Y;

        public Float2(float _X, float _Y)
        {
            X = _X; Y = _Y;
        }

        public static bool operator <(Float2 _A, Float2 _B)
        {
            return _A.X < _B.X && _A.Y < _B.Y;
        }
        public static bool operator >(Float2 _A, Float2 _B)
        {
            return _A.X > _B.X && _A.Y > _B.Y;
        }

        public static Float2 operator +(Float2 _A, Float2 _B)
        {
            return new Float2(_A.X + _B.X, _A.Y + _B.Y);
        }

        public static Float2 operator -(Float2 _A, Float2 _B)
        {
            return new Float2(_A.X - _B.X, _A.Y - _B.Y);
        }

        public static Float2 operator *(Float2 _A, Float2 _B)
        {
            return new Float2(_A.X * _B.X, _A.Y * _B.Y);
        }

        public static Float2 operator *(Float2 _A, float _B)
        {
            return new Float2(_A.X * _B, _A.Y * _B);
        }

        public static Float2 operator /(Float2 _A, Float2 _B)
        {
            return new Float2(_A.X / _B.X, _A.Y / _B.Y);
        }

        public static Float2 operator /(Float2 _A, float _B)
        {
            return new Float2(_A.X / _B, _A.Y / _B);
        }

        public static Float2 operator -(Float2 _A)
        {
            return new Float2(-_A.X, -_A.Y);
        }
    }

    public struct Float3
    {
        public float X;
        public float Y;
        public float Z;
        public Float3(float _X, float _Y, float _Z) { X = _X; Y = _Y; Z = _Z; }
        public static bool operator <(Float3 _A, Float3 _B) { return _A.X < _B.X && _A.Y < _B.Y && _A.Z < _B.Z; }
        public static bool operator >(Float3 _A, Float3 _B) { return _A.X > _B.X && _A.Y > _B.Y && _A.Z > _B.Z; }
        public static Float3 operator +(Float3 _A, Float3 _B) { return new Float3(_A.X + _B.X, _A.Y + _B.Y, _A.Z + _B.Z); }
        public static Float3 operator -(Float3 _A, Float3 _B) { return new Float3(_A.X - _B.X, _A.Y - _B.Y, _A.Z - _B.Z); }
        public static Float3 operator *(Float3 _A, Float3 _B) { return new Float3(_A.X * _B.X, _A.Y * _B.Y, _A.Z - _B.Z); }
        public static Float3 operator *(Float3 _A, float _B) { return new Float3(_A.X * _B, _A.Y * _B, _A.Z * _B); }
        public static Float3 operator /(Float3 _A, Float3 _B) { return new Float3(_A.X / _B.X, _A.Y / _B.Y, _A.Z / _B.Z); }
        public static Float3 operator /(Float3 _A, float _B) { return new Float3(_A.X / _B, _A.Y / _B, _A.Z / _B); }
        public static Float3 operator -(Float3 _A) { return new Float3(-_A.X, -_A.Y, -_A.Z); }
    }


    public struct Float4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Float4(float _X, float _Y, float _Z, float _W)
        {
            X = _X; Y = _Y; Z = _Z; W = _W;
        }

        public static bool operator <(Float4 _A, Float4 _B)
        {
            return _A.X < _B.X && _A.Y < _B.Y && _A.Z < _B.Z && _A.W < _B.W;
        }
        public static bool operator >(Float4 _A, Float4 _B)
        {
            return _A.X > _B.X && _A.Y > _B.Y && _A.Z > _B.Z && _A.W > _B.W;
        }

        public static Float4 operator +(Float4 _A, Float4 _B)
        {
            return new Float4(_A.X + _B.X, _A.Y + _B.Y, _A.Z + _B.Z, _A.W + _B.W);
        }

        public static Float4 operator -(Float4 _A, Float4 _B)
        {
            return new Float4(_A.X - _B.X, _A.Y - _B.Y, _A.Z - _B.Z, _A.W - _B.W);
        }

        public static Float4 operator *(Float4 _A, Float4 _B)
        {
            return new Float4(_A.X * _B.X, _A.Y * _B.Y, _A.Z - _B.Z, _A.W - _B.W);
        }

        public static Float4 operator *(Float4 _A, float _B)
        {
            return new Float4(_A.X * _B, _A.Y * _B, _A.Z * _B, _A.W * _B);
        }

        public static Float4 operator /(Float4 _A, Float4 _B)
        {
            return new Float4(_A.X / _B.X, _A.Y / _B.Y, _A.Z / _B.Z, _A.W / _B.W);
        }

        public static Float4 operator /(Float4 _A, float _B)
        {
            return new Float4(_A.X / _B, _A.Y / _B, _A.Z / _B, _A.W / _B);
        }

        public static Float4 operator -(Float4 _A)
        {
            return new Float4(-_A.X, -_A.Y, -_A.Z, -_A.W);
        }
    }
}
