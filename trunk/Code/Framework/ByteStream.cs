using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class ByteStream
    {
        private Queue<byte> stream = new Queue<byte>();

        public int Length { get { return stream.Count; } }

        public ByteStream() { }
        public ByteStream(byte[] _Data)
        {
            Write(_Data);
        }
        public void Write(byte _Byte)
        {
            lock (stream) { stream.Enqueue(_Byte); }
        }
        public void Write(byte[] _Bytes)
        {
            foreach (byte b in _Bytes) Write(b);
        }
        public void Write(ByteStream _Stream)
        {
            Write(_Stream.ReadAll());
        }
        public byte Read()
        {
            byte b;
            lock (stream) { b = stream.Dequeue(); }
            return b;
        }
        public byte[] Read(int _Count)
        {
            int count = Math.Min(_Count, stream.Count);
            byte[] ret = new byte[count];
            for (int i = 0; i < count; i++) ret[i] = Read();
            return ret;
        }
        public byte[] ReadAll()
        {
            byte[] ret = new byte[stream.Count];
            for (int i = 0; i < ret.Length; i++) ret[i] = Read();
            return ret;
        }
        public byte[] CopyAll()
        {
            byte[] b = new byte[stream.Count];
            lock (stream) { stream.CopyTo(b,0); }
            return b;
        }
        public void Clear()
        {
            stream.Clear();
        }
    }
}
