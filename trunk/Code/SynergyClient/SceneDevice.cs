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
        public SceneDevice(XElement _Element)
        {
            Name = (string)_Element.Element("Name").Value;
            Type = byte.Parse((string)_Element.Element("Type"));
            DeviceID = uint.Parse((string)_Element.Element("DeviceID").Value);
            Size = float.Parse((string)_Element.Element("Size").Value);
            TextSize = float.Parse((string)_Element.Element("TextSize").Value);
            X = float.Parse((string)_Element.Element("X").Value);
            Y = float.Parse((string)_Element.Element("Y").Value);
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
        public virtual void OnClick(){ }
        public virtual void OnDraw(Graphics _Graphics, float _GraphicsSize) { }
        public string Name;
        public uint DeviceID;
        public float Size;
        public float TextSize;
        public float X, Y;
        public byte Type;
    }
    public class DigitalOutSceneDevice:SceneDevice
    {
        public DigitalOutSceneDevice(XElement _Xelement):base(_Xelement)
        {
        }
        public override void OnClick()
        {
            if(ConnectionManager.Devices.ContainsKey(DeviceID))
            {
                ConnectionManager.Devices[DeviceID].ToggleDigital();
                ConnectionManager.Devices[DeviceID].UpdateRemoteMemory();
            }
        }
        public override void OnDraw(Graphics _Graphics, float _GraphicsSize)
        {
            float size = Size * _GraphicsSize;
            Font font = new Font("Arial", size * TextSize, FontStyle.Bold);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            if (ConnectionManager.Devices.ContainsKey(DeviceID))
            {
                _Graphics.FillEllipse(ConnectionManager.Devices[DeviceID].GetDigitalState() ? Brushes.Green : Brushes.Red, new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size));
            }
            else
            {
                _Graphics.FillEllipse(Brushes.Gray, new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size));
            }
            _Graphics.DrawString(Name, font, Brushes.Black, X * _GraphicsSize, Y * _GraphicsSize, format);
        }
    }
    public class AnalogOutSceneDevice : SceneDevice
    {
        public AnalogOutSceneDevice(XElement _Xelement)
            : base(_Xelement)
        {
        }
        public override void OnClick()
        {
            if (ConnectionManager.Devices.ContainsKey(DeviceID))
            {
                ConnectionManager.Devices[DeviceID].ToggleDigital();
                ConnectionManager.Devices[DeviceID].UpdateRemoteMemory();
            }
        }
        public override void OnDraw(Graphics _Graphics, float _GraphicsSize)
        {
            float size = Size * _GraphicsSize;
            Font font = new Font("Arial", size * TextSize, FontStyle.Bold);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            Rectangle rect = new Rectangle((int)((X * _GraphicsSize) - size * 0.5f), (int)((Y * _GraphicsSize) - size * 0.5f), (int)size, (int)size);
            if (ConnectionManager.Devices.ContainsKey(DeviceID))
            {
                _Graphics.FillPie(Brushes.Green,rect, 0, 90);

                _Graphics.FillEllipse(ConnectionManager.Devices[DeviceID].GetDigitalState() ? Brushes.Green : Brushes.Red, rect);
            }
            else
            {
                _Graphics.FillEllipse(Brushes.Gray, new RectangleF((X * _GraphicsSize) - size * 0.5f, (float)(Y * _GraphicsSize) - size * 0.5f, size, size));
            }
            _Graphics.DrawString(Name, font, Brushes.Black, X * _GraphicsSize, Y * _GraphicsSize, format);
        }
    }
}
