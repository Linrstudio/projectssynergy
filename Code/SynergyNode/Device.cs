using System;
using System.Collections.Generic;
using System.Text;

namespace SynergyNode
{
    public class Device
    {

        public Device(bool _IsLocal) { islocal = _IsLocal; }
        private bool islocal;//do not change
        public ushort ID;
        public byte DeviceType = 0;
        public byte[] Memory = new byte[0];
        public bool IsLocal() { return islocal; }
        public virtual void OnMemoryChanged(){}
        public void SetMemory(byte[] _Memory, bool _UpdateRemote)
        {
            Memory = _Memory;
            OnMemoryChanged();
            if (_UpdateRemote) UpdateRemoteMemory();
            Console.WriteLine("Memory changed!");
        }
        public void SetMemory(byte[] _Memory)
        {
            SetMemory(_Memory, true);
        }
        public void UpdateRemoteMemory()
        {
            ConnectionManager.SetRemoteMemory(ID, Memory);
        }
        //Data types!
        public void SetDigitalState(bool _On) { Memory = new byte[] { (byte)(_On ? 255 : 0) }; }//Type 0/1
        public void ToggleDigital() { SetDigitalState(!GetDigitalState()); }
        public bool GetDigitalState() { if (Memory.Length == 0)return false; return Memory[0] != 0; }//Type 0/1

        public void SetAnalogState(byte _Value) { Memory = new byte[] { _Value }; }//Type 2/3
        public byte GetAnalogState() { if (Memory.Length == 0) return 0; else return Memory[0]; }//Type 2/3
    }

    public class LocalDevice : Device
    {
        public LocalDevice(ushort _DeviceID,byte _Type) : base(true)
        {
            ID = _DeviceID;
            DeviceType = _Type;
        }
    }

    public class RemoteDevice : Device
    {
        public RemoteDevice(ushort _DeviceID) : base(false)
        {
            ID = _DeviceID;
        }
    }
}
