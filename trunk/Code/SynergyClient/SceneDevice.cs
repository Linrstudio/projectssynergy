using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using SynergyNode;

namespace SynergyClient
{
    public class SceneDevice
    {
        public void FromElement(XElement _Element)
        {
            Name = (string)_Element.Element("Name").Value;
            Type = byte.Parse((string)_Element.Element("Type"));
            DeviceID = ushort.Parse((string)_Element.Element("DeviceID").Value);
            Size = float.Parse((string)_Element.Element("Size").Value);
            TextSize = float.Parse((string)_Element.Element("TextSize").Value);
            X = float.Parse((string)_Element.Element("X").Value);
            Y = float.Parse((string)_Element.Element("Y").Value);
        }
        public SceneDevice()
        {
            
        }
        public XElement Save()
        {
            XElement element = new XElement("Device");
            element.Add(new XElement("Name", Name));
            element.Add(new XElement("Type", Type));
            element.Add(new XElement("DeviceID", DeviceID.ToString()));
            element.Add(new XElement("Size", Size.ToString()));
            element.Add(new XElement("TextSize", TextSize.ToString()));
            element.Add(new XElement("X", X.ToString()));
            element.Add(new XElement("Y", Y.ToString()));
            return element;
        }
        public virtual void OnClick(float _X, float _Y){ }
        public virtual void OnDraw(Graphics _Graphics, float _GraphicsSize) { }
        public string Name;
        public ushort DeviceID;
        public float Size;
        public float TextSize;
        public float X, Y;
        public byte Type;

        public static SceneDevice GetDevice(byte _Type)
        {
            switch (_Type)
            {
                case 10: return new DigitalOutSceneDevice(); break;
                case 12: return new AnalogOutSceneDevice(); break;
            }
            return null;
        }
        public static SceneDevice GetDevice(Device _Device)
        {
            SceneDevice d = GetDevice(_Device.DeviceType);
            if (d == null) return null;
            d.DeviceID = _Device.ID;
            d.X = 0.5f;
            d.Y = 0.5f;
            d.Size = 0.2f;
            d.TextSize = 0.1f;
            d.Name = "Device " + d.DeviceID.ToString();

            return d;
        }
        public static SceneDevice GetDevice(XElement _Element)
        {
            byte type = byte.Parse((string)_Element.Element("Type"));
            SceneDevice d = GetDevice(type);
            if (d == null) return null;
            d.FromElement(_Element);
            return d;
        }
    }
    public class DigitalOutSceneDevice:SceneDevice
    {
        //public DigitalOutSceneDevice(XElement _Xelement):base(_Xelement){}
        public DigitalOutSceneDevice() : base() { }
        public override void OnClick(float _X, float _Y)
        {
            if(NetworkNode.RemoteDevices.ContainsKey(DeviceID))
            {
                ((DigitalMemoryBin)NetworkNode.RemoteDevices[DeviceID].Memory).Toggle();
                NetworkNode.RemoteDevices[DeviceID].UpdateRemoteMemory();
            }
        }
        public override void OnDraw(Graphics _Graphics, float _GraphicsSize)
        {
            float size = Size * _GraphicsSize;
            if (size * TextSize <= 0) return;

            Font font = new Font("Courier new", size * TextSize, FontStyle.Bold);


            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            
            RectangleF rect=new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size);

            if (NetworkNode.RemoteDevices.ContainsKey(DeviceID))
            {
                bool on = ((DigitalMemoryBin)NetworkNode.RemoteDevices[DeviceID].Memory).On;
                //_Graphics.FillEllipse(on ? Brushes.Green : Brushes.Red, new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size));
                _Graphics.DrawImage(Resources.images["Options"],rect);
                _Graphics.DrawImage(Resources.images[on ? "DigitalOn" : "DigitalOff"], rect);
            }
            else
            {
                _Graphics.DrawImage(Resources.images["DigitalNotFound"], new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size));
            }
            _Graphics.DrawString(Name, font, Brushes.Black, (X + (Size * 0.02f)) * _GraphicsSize, (Y + (Size * 0.02f)) * _GraphicsSize, format);
            _Graphics.DrawString(Name, font, Brushes.DarkGray, X * _GraphicsSize, Y * _GraphicsSize, format);

        }
    }
    public class AnalogOutSceneDevice : SceneDevice
    {
        //public AnalogOutSceneDevice(XElement _Xelement) : base(_Xelement){}
        public AnalogOutSceneDevice() : base() { }
        public override void OnClick(float _X,float _Y)
        {
            if (NetworkNode.RemoteDevices.ContainsKey(DeviceID))
            {
                float dx=_X-X;
                float dy=Y-_Y;
                byte value = (byte)(Math.Atan2(dx, dy) * 40.86f);
                ((AnalogMemoryBin)NetworkNode.RemoteDevices[DeviceID].Memory).TargetValue = value;
                ((AnalogMemoryBin)NetworkNode.RemoteDevices[DeviceID].Memory).Speed = 3;
                NetworkNode.RemoteDevices[DeviceID].UpdateRemoteMemory();
            }
        }
        public override void OnDraw(Graphics _Graphics, float _GraphicsSize)
        {
            float size = Size * _GraphicsSize;
            Font font = new Font("Courier new", size * TextSize, FontStyle.Bold);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            Rectangle rect = new Rectangle((int)((X * _GraphicsSize) - size * 0.5f), (int)((Y * _GraphicsSize) - size * 0.5f), (int)size, (int)size);
            if (NetworkNode.RemoteDevices.ContainsKey(DeviceID))
            {
                byte value = ((AnalogMemoryBin)NetworkNode.RemoteDevices[DeviceID].Memory).TargetValue;
                _Graphics.FillPie(Brushes.Green, rect, 270, value * 1.411f);
                _Graphics.DrawEllipse(Pens.Green, rect);
                //_Graphics.FillEllipse(NetworkNode.Devices[DeviceID].GetDigitalState() ? Brushes.Green : Brushes.Red, rect);
            }
            else
            {
                _Graphics.FillEllipse(Brushes.Gray, new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size));
            }
            _Graphics.DrawString(Name, font, Brushes.Black, X * _GraphicsSize, Y * _GraphicsSize, format);
        }
    }
}
