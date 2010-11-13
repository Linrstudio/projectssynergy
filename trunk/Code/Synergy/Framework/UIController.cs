using System;
using System.Collections.Generic;
using System.Text;
using Synergy;

namespace Synergy
{
    public class UITouchEvent
    {
        enum State
        {
            Pressed, Released, Up, Down
        }
        public UITouchEvent(Float2 _Position, bool _Pressed, bool _Released)
        {
            position = _Position;
            pressed = _Pressed;
            released = _Released;
        }

        /// <summary>
        /// Indicates that the screen is released
        /// </summary>
        public bool Pressed { get { return pressed; } }
        bool pressed;

        /// <summary>
        /// Indicates that the screen is pressed
        /// </summary>
        public bool Released { get { return released; } }
        bool released;

        /// <summary>
        /// The position on the screen where the event occured
        /// </summary>
        public Float2 Position { get { return position; } }
        Float2 position;
    }
}
