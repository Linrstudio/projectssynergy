using System;
using System.Collections.Generic;
using System.Text;

namespace Synergy
{
    public struct Int2
    {
        //Fields
        public int X;
        public int Y;

        public Int2(int _X, int _Y) { X = _X; Y = _Y; }
        //Comparations
        public static bool operator <(Int2 _A, Int2 _B) { return _A.X < _B.X && _A.Y < _B.Y; }
        public static bool operator >(Int2 _A, Int2 _B) { return _A.X > _B.X && _A.Y > _B.Y; }
        public static bool operator ==(Int2 _A, Int2 _B) { return _A.X == _B.X && _A.Y == _B.Y; }
        public static bool operator !=(Int2 _A, Int2 _B) { return !(_A == _B); }
        //Operators
        public static Int2 operator +(Int2 _A, Int2 _B) { return new Int2(_A.X + _B.X, _A.Y + _B.Y); }
        public static Int2 operator -(Int2 _A, Int2 _B) { return new Int2(_A.X - _B.X, _A.Y - _B.Y); }
        public static Int2 operator *(Int2 _A, Int2 _B) { return new Int2(_A.X * _B.X, _A.Y * _B.Y); }
        public static Int2 operator *(Int2 _A, int _B) { return new Int2(_A.X * _B, _A.Y * _B); }
        public static Int2 operator /(Int2 _A, Int2 _B) { return new Int2(_A.X / _B.X, _A.Y / _B.Y); }
        public static Int2 operator /(Int2 _A, int _B) { return new Int2(_A.X / _B, _A.Y / _B); }
        public static Int2 operator -(Int2 _A) { return new Int2(-_A.X, -_A.Y); }

        public float Length() { return (float)Math.Sqrt((float)(X * X + Y * Y)); }
        public int LengthSquared() { return X * X + Y * Y; }

        //casts
        public static implicit operator Int2(Float2 _V) { return new Int2((int)_V.X, (int)_V.Y); }

        //Methods

        public override string ToString()
        {
            return string.Format("[{0} {1}]", X, Y);
        }
    }

    public struct Float2
    {
        //Fields
        public float X;
        public float Y;

        public Float2(float _X, float _Y) { X = _X; Y = _Y; }
        //Comparations
        public static bool operator <(Float2 _A, Float2 _B) { return _A.X < _B.X && _A.Y < _B.Y; }
        public static bool operator >(Float2 _A, Float2 _B) { return _A.X > _B.X && _A.Y > _B.Y; }
        public static bool operator ==(Float2 _A, Float2 _B) { return _A.X == _B.X && _A.Y == _B.Y; }
        public static bool operator !=(Float2 _A, Float2 _B) { return !(_A == _B); }
        //Operators
        public static Float2 operator +(Float2 _A, Float2 _B) { return new Float2(_A.X + _B.X, _A.Y + _B.Y); }
        public static Float2 operator -(Float2 _A, Float2 _B) { return new Float2(_A.X - _B.X, _A.Y - _B.Y); }
        public static Float2 operator *(Float2 _A, Float2 _B) { return new Float2(_A.X * _B.X, _A.Y * _B.Y); }
        public static Float2 operator *(Float2 _A, float _B) { return new Float2(_A.X * _B, _A.Y * _B); }
        public static Float2 operator /(Float2 _A, Float2 _B) { return new Float2(_A.X / _B.X, _A.Y / _B.Y); }
        public static Float2 operator /(Float2 _A, float _B) { return new Float2(_A.X / _B, _A.Y / _B); }
        public static Float2 operator -(Float2 _A) { return new Float2(-_A.X, -_A.Y); }

        public float Length() { return (float)Math.Sqrt(X * X + Y * Y); }
        public float LengthSquared() { return X * X + Y * Y; }

        //casts
        public static implicit operator Float2(Int2 _V) { return new Float2((float)_V.X, (float)_V.Y); }

        //Methods
        public static float Dot(Float2 _A, Float2 _B) { return _A.X * _B.X + _A.Y * _B.Y; }
        public static Float2 Normalize(Float2 _A) { return _A / _A.Length(); }
        public static float Length(Float2 _A) { return _A.Length(); }

        public override string ToString()
        {
            return string.Format("[{0} {1}]", X, Y);
        }
    }

    public struct Float3
    {
        //Fields
        public float X;
        public float Y;
        public float Z;

        public Float2 XY { get { return new Float2(X, Y); } set { X = value.X; Y = value.Y; } }
        public Float2 YZ { get { return new Float2(Y, Z); } set { Y = value.X; Z = value.Y; } }
        public Float2 ZX { get { return new Float2(Z, X); } set { Z = value.X; X = value.Y; } }

        public Float2 YX { get { return new Float2(Y, X); } set { Y = value.X; X = value.Y; } }
        public Float2 ZY { get { return new Float2(Z, Y); } set { Z = value.X; Y = value.Y; } }
        public Float2 XZ { get { return new Float2(X, Z); } set { X = value.X; Z = value.Y; } }

        public Float3(float _X, float _Y, float _Z) { X = _X; Y = _Y; Z = _Z; }
        public Float3(Float2 _XY, float _Z) { X = _XY.X; Y = _XY.Y; Z = _Z; }
        //Comparations
        public static bool operator <(Float3 _A, Float3 _B) { return _A.X < _B.X && _A.Y < _B.Y && _A.Z < _B.Z; }
        public static bool operator >(Float3 _A, Float3 _B) { return _A.X > _B.X && _A.Y > _B.Y && _A.Z > _B.Z; }
        public static bool operator ==(Float3 _A, Float3 _B) { return _A.X == _B.X && _A.Y == _B.Y && _A.Z == _B.Z; }
        public static bool operator !=(Float3 _A, Float3 _B) { return !(_A == _B); }
        //Operators
        public static Float3 operator +(Float3 _A, Float3 _B) { return new Float3(_A.X + _B.X, _A.Y + _B.Y, _A.Z + _B.Z); }
        public static Float3 operator -(Float3 _A, Float3 _B) { return new Float3(_A.X - _B.X, _A.Y - _B.Y, _A.Z - _B.Z); }
        public static Float3 operator *(Float3 _A, Float3 _B) { return new Float3(_A.X * _B.X, _A.Y * _B.Y, _A.Z * _B.Z); }
        public static Float3 operator *(Float3 _A, float _B) { return new Float3(_A.X * _B, _A.Y * _B, _A.Z * _B); }
        public static Float3 operator /(Float3 _A, Float3 _B) { return new Float3(_A.X / _B.X, _A.Y / _B.Y, _A.Z / _B.Z); }
        public static Float3 operator /(Float3 _A, float _B) { return new Float3(_A.X / _B, _A.Y / _B, _A.Z / _B); }
        public static Float3 operator -(Float3 _A) { return new Float3(-_A.X, -_A.Y, -_A.Z); }

        public float Length() { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
        public float LengthSquared() { return X * X + Y * Y + Z * Z; }

        //Methods
        public static float Dot(Float3 _A, Float3 _B) { return _A.X * _B.X + _A.Y * _B.Y + _A.Z * _B.Z; }
        public static Float3 Normalize(Float3 _A) { return _A / _A.Length(); }
        public static float Length(Float3 _A) { return _A.Length(); }

        public override string ToString()
        {
            return string.Format("[{0} {1} {2}]", X, Y, Z);
        }
    }


    public struct Float4
    {
        //Fields
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Float3 XYZ
        {
            get { return new Float3(X, Y, Z); }
            set { X = value.X; Y = value.Y; Z = value.Z; }
        }

        //Comparations
        public Float4(float _X, float _Y, float _Z, float _W) { X = _X; Y = _Y; Z = _Z; W = _W; }
        public static bool operator <(Float4 _A, Float4 _B) { return _A.X < _B.X && _A.Y < _B.Y && _A.Z < _B.Z && _A.W < _B.W; }
        public static bool operator >(Float4 _A, Float4 _B) { return _A.X > _B.X && _A.Y > _B.Y && _A.Z > _B.Z && _A.W > _B.W; }
        public static bool operator ==(Float4 _A, Float4 _B) { return _A.X == _B.X && _A.Y == _B.Y && _A.Z == _B.Z && _A.W == _B.W; }
        public static bool operator !=(Float4 _A, Float4 _B) { return !(_A == _B); }

        //Operators
        public static Float4 operator +(Float4 _A, Float4 _B) { return new Float4(_A.X + _B.X, _A.Y + _B.Y, _A.Z + _B.Z, _A.W + _B.W); }
        public static Float4 operator -(Float4 _A, Float4 _B) { return new Float4(_A.X - _B.X, _A.Y - _B.Y, _A.Z - _B.Z, _A.W - _B.W); }
        public static Float4 operator *(Float4 _A, Float4 _B) { return new Float4(_A.X * _B.X, _A.Y * _B.Y, _A.Z * _B.Z, _A.W * _B.W); }
        public static Float4 operator *(Float4 _A, float _B) { return new Float4(_A.X * _B, _A.Y * _B, _A.Z * _B, _A.W * _B); }
        public static Float4 operator /(Float4 _A, Float4 _B) { return new Float4(_A.X / _B.X, _A.Y / _B.Y, _A.Z / _B.Z, _A.W / _B.W); }
        public static Float4 operator /(Float4 _A, float _B) { return new Float4(_A.X / _B, _A.Y / _B, _A.Z / _B, _A.W / _B); }
        public static Float4 operator -(Float4 _A) { return new Float4(-_A.X, -_A.Y, -_A.Z, -_A.W); }

        public float Length() { return (float)Math.Sqrt(Float4.Dot(this, this)); }
        public float LengthSquared() { return X * X + Y * Y + Z * Z + W * W; }

        //Methods
        public static float Dot(Float4 _A, Float4 _B) { return _A.X * _B.X + _A.Y * _B.Y + _A.Z * _B.Z + _A.W * _B.W; }
        public static Float4 Normalize(Float4 _A) { return _A / _A.Length(); }
        public static float Length(Float2 _A) { return _A.Length(); }

        public override string ToString()
        {
            return string.Format("[{0} {1} {2} {3}]", X, Y, Z, W);
        }
    }
}