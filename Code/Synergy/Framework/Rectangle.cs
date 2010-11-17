using System;
using System.Collections.Generic;
using System.Text;

namespace Synergy
{
    public struct Rect
    {
        public Int2 From;
        public Int2 To;

        public Rect(int _FromX, int _FromY, int _ToX, int _ToY)
        {
            From.X = _FromX;
            From.Y = _FromY;
            To.X = _ToX;
            To.Y = _ToY;
        }

        public Rect(Int2 _From, Int2 _To)
        {
            From = _From;
            To = _To;
        }

        public Int2 Size
        {
            get { return To - From; }
            set { To = From + Size; }
        }
    }
}
