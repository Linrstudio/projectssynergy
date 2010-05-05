using System;
using System.Collections.Generic;
using System.Text;

using Framework;
using SynergyTemplate;
using SynergyGraphics;

namespace SynergyClient
{
    public class GenericBrowser : Control
    {
        public SortedDictionary<string, Column> columns = new SortedDictionary<string, Column>();
        public class Column
        {
            public GenericBrowserHeader header = null;
            public SortedDictionary<string, GenericBrowserElement> elements = new SortedDictionary<string, GenericBrowserElement>();
        }

        public GenericBrowser(Control _Parent)
            : base("GenericBrowser", _Parent)
        {
            OnRender += GenericBrowser_OnRefresh;
        }

        void GenericBrowser_OnRefresh()
        {
            //SetViewRelativeToParent(Graphics.defaultshader);
            Graphics.defaultshader.SetParameter("View", GetTransformation());
            Graphics.defaultshader.SetParameter("DiffuseMap", ClientResources.Background);
            Graphics.defaultshader.Begin();
            Graphics.DrawRectangle(
                new Float2(0, 0),
                new Float2(1, 0),
                new Float2(0, 1),
                new Float2(1, 1), 0.5f);
            Graphics.defaultshader.End();
        }

        public const float VerticalElementCount = 10;
        public const float HorizontalElementCount = 3;

        void addelement(NetworkNodeRemote _Node, NetworkClassSlave _Class)
        {
            if (!columns.ContainsKey(_Node.NodeID.ToString()))
            {
                GenericBrowserHeader header = new GenericBrowserHeader(_Node.NodeID.ToString(), this);
                header.node = _Node;
                columns.Add(_Node.NodeID.ToString(), new Column());
                columns[_Node.NodeID.ToString()].header = header;
            }
            Column col = columns[_Node.NodeID.ToString()];
            if (!col.elements.ContainsKey(_Class.Name))
            {
                GenericBrowserElement element = new GenericBrowserElement(_Class.Name, this);
                element.networkclass = _Class;
                col.elements.Add(_Class.Name, element);
            }
        }

        public override void Update()
        {
            foreach (NetworkNodeRemote node in NetworkManager.RemoteNodes.Values)
            {
                foreach (NetworkClassSlave clas in node.LocalDevices.Values)
                {
                    string controlname = clas.Name;
                    if (!Children.ContainsKey(controlname))
                    {
                        addelement(node, clas);
                    }
                }
            }
            float x = 0;
            float y = 1 / VerticalElementCount;
            foreach (Column c in columns.Values)
            {
                float startx = x;
                c.header.Transformation = Float3x3.Translate(new Float2(x, 0)) * Float3x3.Scale(new Float2(1.0f / HorizontalElementCount, 1.0f / VerticalElementCount));
                foreach (GenericBrowserElement e in c.elements.Values)
                {
                    e.Transformation = new Float3x3();
                    if (y > 1) { y = 1 / VerticalElementCount; x += 1 / HorizontalElementCount; }
                    e.Transformation = Float3x3.Translate(new Float2(x, y)) * Float3x3.Scale(new Float2(1.0f / HorizontalElementCount, 1.0f / VerticalElementCount));
                    y += 1 / VerticalElementCount;
                }

                //c.header.Position.X = Size.X * ((x + startx) * 0.5f);
                //c.header.Position.Y = 0;
                //c.header.Size.X = Size.X / HorizontalElementCount;
                //c.header.Size.Y = Size.Y / VerticalElementCount;

                y = 1 / VerticalElementCount;
                x += 1 / HorizontalElementCount;
                x += 0.1f;
            }
            base.Update();
        }
    }

    public class GenericBrowserHeader : Control
    {
        public NetworkNodeRemote node = null;
        public GenericBrowserHeader(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            OnRender += OnDraw;
        }
        public void OnDraw()
        {
            Graphics.SetAlphaBlending(true);
            Graphics.defaultshader.SetParameter("View", GetTransformation());
            Graphics.defaultshader.SetParameter("DiffuseMap", ClientResources.GenericBrowserHeaderBackground);
            Graphics.defaultshader.Begin();
            Graphics.DrawRectangle(
                new Float2(0, 0),
                new Float2(1, 0),
                new Float2(0, 1),
                new Float2(1, 1),
                0.3f);
            Graphics.defaultshader.End();

            Float2 Fontoffset = new Float2(0.05f, 0.05f);
            ClientResources.handwrittenfont.Draw(new Float2(0, 0) + Fontoffset, new Float2(1, 1) - Fontoffset * 2, node.NodeID.ToString());
        }
    }

    public class GenericBrowserElement : Control
    {
        public NetworkClassSlave networkclass = null;
        public GenericBrowserElement(string _Name, Control _Parent)
            : base(_Name, _Parent)
        {
            OnRender += OnDraw;
        }
        public void OnDraw()
        {
            Graphics.SetAlphaBlending(true);
            Graphics.defaultshader.SetParameter("View", GetTransformation());
            Graphics.defaultshader.SetParameter("DiffuseMap", ClientResources.GenericBrowserElementBackground);
            Graphics.defaultshader.Begin();
            Graphics.DrawRectangle(
                new Float2(0, 0),
                new Float2(1, 0),
                new Float2(0, 1),
                new Float2(1, 1),
                0.5f);
            Graphics.defaultshader.End();

            Float2 Fontoffset = new Float2(0.05f, 0.05f);
            /*
            Graphics.defaultshader.Begin();
            Graphics.DrawRectangle(
                new Float2(-1, -1),
                new Float2(1, -1),
                new Float2(-1, 1),
                new Float2(1, 1),
                0.5f);
            Graphics.defaultshader.End();
            */
            //ClientResources.handwrittenfont.DrawMakeFit(new Float2(0, 0) + Fontoffset, new Float2(1, 1) - Fontoffset * 2, Name, true);
            ClientResources.handwrittenfont.Draw(new Float2(0, 0) + Fontoffset, new Float2(1, 1) - Fontoffset * 2, Name);
            ClientResources.handwrittenfont.Draw(new Float2(90, 0) + Fontoffset, new Float2(1, 1) - Fontoffset * 2, "Fields:"+networkclass.Fields.Count);
        }
    }
}
