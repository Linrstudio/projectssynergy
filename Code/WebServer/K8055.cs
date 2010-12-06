using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebServer
{
    public static class K8055
    {
        [DllImport("k8055.dll")]
        private static extern int OpenDevice(int CardAddress);
        [DllImport("k8055.dll")]
        private static extern void SetDigitalChannel(int Channel);
        [DllImport("k8055.dll")]
        private static extern void ClearDigitalChannel(int Channel);
        [DllImport("k8055.dll")]
        private static extern int ReadDigitalChannel(int Channel);

        public static void Initialize(int _CardAddress)
        {
            OpenDevice(_CardAddress);
            for (int i = 0; i < 8; i++) ClearDigitalChannel(i + 1);
        }

        public static bool[] State = new bool[8];
        public static void SetDigital(int _Channel, bool _State)
        {
            if (_State != State[_Channel])
            {
                if (_State)
                {
                    SetDigitalChannel(_Channel + 1);
                    State[_Channel] = true;
                }
                else
                {
                    ClearDigitalChannel(_Channel + 1);
                    State[_Channel] = false;
                }
            }
        }

        public static bool GetDigital(int _Channel)
        {
            return ReadDigitalChannel(_Channel + 1) != 0;
        }
    }
}
