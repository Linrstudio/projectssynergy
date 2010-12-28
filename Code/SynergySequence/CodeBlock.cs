using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;

namespace SynergySequence
{
    public abstract class CodeBlock
    {
        public CodeBlock()
        {
        }
        [Obsolete]
        public CodeBlock(Sequence _Sequence)
        {
            Sequence = _Sequence;
        }

        public Sequence Sequence;

        public List<DataInput> DataInputs = new List<DataInput>();
        public List<DataOutput> DataOutputs = new List<DataOutput>();
        public List<TriggerInput> TriggerInputs = new List<TriggerInput>();
        public List<TriggerOutput> TriggerOutputs = new List<TriggerOutput>();

        [Browsable(false)]
        public Input[] Inputs { get { List<Input> l = new List<Input>(DataInputs.ToArray()); l.AddRange(TriggerInputs.ToArray()); return l.ToArray(); } }
        [Browsable(false)]
        public Output[] Outputs { get { List<Output> l = new List<Output>(DataOutputs.ToArray()); l.AddRange(TriggerOutputs.ToArray()); return l.ToArray(); } }
        public bool Selected = false;

        List<int> InputRegisters = new List<int>();

        //editor stuff
        public float X;
        public float Y;
        public float width = 200;
        public float height = 100;
        [Browsable(false)]
        public float Width { get { return width; } }
        [Browsable(false)]
        public float Height { get { return height; } }

        public bool IsEvent = false;

        public string Name;

        //for save and load purposes
        public virtual void Save(XElement _Data){}
        public virtual void Load(XElement _Data){}

        public virtual bool Intersect(Point _Point)
        {
            return _Point.X > X - Width / 2 && _Point.Y > Y - Height / 2 && _Point.X < X + Width / 2 && _Point.Y < Y + Height / 2;
        }

        public void Update()
        {

        }

        public PointF GetShadowOffset()
        {
            //float h = (float)Math.Sqrt(ScopeDepth + 1);
            float h = 1;
            return new PointF(h * 2.5f, h * 5);
        }

        public void DrawShape(Graphics _Graphics, params PointF[] _Points)
        {
            for (int i = 0; i < _Points.Length; i++)
            {
                _Points[i].X += X;
                _Points[i].Y += Y;
            }
            _Graphics.FillPolygon(new SolidBrush(Color.FromArgb(150, 150, 255)), _Points);
            _Graphics.DrawPolygon(new Pen(Brushes.Black, 2), _Points);
        }

        public virtual void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(Name, new Font("Arial", 15, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }

        public void DrawShapeShadow(Graphics _Graphics, params PointF[] _Points)
        {
            for (int i = 0; i < _Points.Length; i++)
            {
                _Points[i].X += X + GetShadowOffset().X;
                _Points[i].Y += Y + GetShadowOffset().Y;
            }
            _Graphics.FillPolygon(new SolidBrush(Sequence.ShadowColor), _Points);
        }

        public void DrawTriangle(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new Point(-50, -75 / 2),
                new Point(50, 0),
                new Point(-50, 75 / 2),
                new Point(-50, -75 / 2));
        }

        public void DrawBlock(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
        }
        public void DrawBlockShadow(Graphics _Graphics)
        {
            DrawShapeShadow(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
        }

        public void DrawTriangleShadow(Graphics _Graphics)
        {
            DrawShapeShadow(_Graphics,
                new Point(-50, -75 / 2),
                new Point(50, 0),
                new Point(-50, 75 / 2),
                new Point(-50, -75 / 2));
        }

        public void DrawConstant(Graphics _Graphics)
        {
            _Graphics.FillEllipse(new SolidBrush(Color.FromArgb(150, 150, 255)), new RectangleF(X - Width / 2, Y - Height / 2, Width, Height));
            _Graphics.DrawEllipse(new Pen(Brushes.Black, 2), new RectangleF(X - Width / 2, Y - Height / 2, Width, Height));
        }

        public void DrawConstantShadow(Graphics _Graphics)
        {
            _Graphics.FillEllipse(new SolidBrush(Sequence.ShadowColor), new RectangleF(GetShadowOffset().X + X - Width / 2, GetShadowOffset().Y + Y - Height / 2, Width, Height));
        }

        public void UpdateConnectors()
        {
            foreach (DataInput i in DataInputs) i.UpdatePosition();
            foreach (DataOutput o in DataOutputs) o.UpdatePosition();
            foreach (TriggerInput i in TriggerInputs) i.UpdatePosition();
            foreach (TriggerOutput o in TriggerOutputs) o.UpdatePosition();
        }

        public void DisconnectAllInputs()
        {
            foreach (DataInput i in DataInputs)
            {
                if (i.Connected != null)
                    i.Connected.Connected.Remove(i);
            }
            foreach (TriggerInput i in TriggerInputs)
            {
                foreach (TriggerOutput o in i.Connected) o.Connected.Remove(i);
                i.Connected.Clear();
            }
        }
        public void DisconnectAllOutputs()
        {
            foreach (DataOutput o in DataOutputs)
            {
                foreach (DataInput i in o.Connected) i.Connected = null;
                o.Connected.Clear();
            }
            foreach (TriggerOutput o in TriggerOutputs)
            {
                foreach (TriggerInput i in o.Connected) i.Connected.Remove(o);
                o.Connected.Clear();
            }
        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////
        /// </summary>
        public abstract class Connector
        {
            public CodeBlock Owner;
            public float X;
            public float Y;
            public Connector(CodeBlock _Owner) { Owner = _Owner; }
            public System.Drawing.PointF GetPosition()
            {
                return new System.Drawing.PointF(Owner.X + X, Owner.Y + Y);
            }
            public abstract bool AnyConnected { get; }
        };

        public abstract class Input : Connector
        {
            public Input(CodeBlock _CodeBlock) : base(_CodeBlock) { }

        }

        public abstract class Output : Connector
        {
            public Output(CodeBlock _CodeBlock) : base(_CodeBlock) { }
        }

        public class DataInput : Input
        {
            public DataInput(CodeBlock _Owner, string _Text, string _DataType)
                : base(_Owner)
            {
                if (_DataType == null) throw new Exception("null DataType");
                Owner = _Owner;
                Text = _Text;
                datatype = _DataType;
                UpdatePosition();
            }
            public string Text;



            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public DataOutput Connected = null;
            public CodeBlock Owner;
            public string datatype;

            public override bool AnyConnected { get { return Connected != null; } }

            public void UpdatePosition()
            {
                float cnt = Owner.DataInputs.Count;
                float idx = (float)Owner.DataInputs.IndexOf(this) - ((cnt - 1) / 2);
                Y = -Owner.Height / 2;
                X = (int)((idx / cnt) * Owner.height);
            }
        }

        public class DataOutput : Output
        {
            public DataOutput(CodeBlock _Owner, string _Text, string _DataType)
                : base(_Owner)
            {
                if (_DataType == null) throw new Exception("null DataType");
                Owner = _Owner;
                Text = _Text;
                datatype = _DataType;
                UpdatePosition();
            }

            public string Text;
            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public List<DataInput> Connected = new List<DataInput>();
            public CodeBlock Owner;
            public string datatype;

            public override bool AnyConnected { get { return Connected.Count != 0; } }

            public void UpdatePosition()
            {
                float cnt = Owner.DataOutputs.Count;
                float idx = (float)Owner.DataOutputs.IndexOf(this) - ((cnt - 1) / 2);
                Y = Owner.Height / 2;
                X = (int)((idx / cnt) * Owner.height);
            }

            public void DisconnectAll()
            {
                foreach (DataInput i in Connected)
                {
                    i.Connected = null;
                }
                Connected.Clear();
            }
        }

        public class TriggerInput : Input
        {
            public TriggerInput(CodeBlock _Owner, string _Text)
                : base(_Owner)
            {
                Owner = _Owner;
                Text = _Text;
                UpdatePosition();
            }

            public string Text;
            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public List<TriggerOutput> Connected = new List<TriggerOutput>();
            public CodeBlock Owner;

            public override bool AnyConnected { get { return Connected.Count != 0; } }

            public void UpdatePosition()
            {
                float cnt = Owner.TriggerInputs.Count;
                float idx = (float)Owner.TriggerInputs.IndexOf(this) - ((cnt - 1) / 2);
                X = -Owner.Width / 2;
                Y = (int)((idx / cnt) * Owner.height);
            }

            public void DisconnectAll()
            {
                foreach (TriggerOutput i in Connected)
                {
                    i.Connected.Remove(this);
                }
                Connected.Clear();
            }
        }

        public class TriggerOutput : Output
        {
            public TriggerOutput(CodeBlock _Owner, string _Text)
                : base(_Owner)
            {
                Owner = _Owner;
                Text = _Text;
                UpdatePosition();
            }

            public string Text;
            public System.Windows.Forms.ToolTip tooptip = new System.Windows.Forms.ToolTip();

            //position in codeblock
            public List<TriggerInput> Connected = new List<TriggerInput>();
            public CodeBlock Owner;

            public override bool AnyConnected { get { return Connected.Count != 0; } }

            public void UpdatePosition()
            {
                float cnt = Owner.TriggerOutputs.Count;
                float idx = (float)Owner.TriggerOutputs.IndexOf(this) - ((cnt - 1) / 2);
                X = Owner.Width / 2;
                Y = (int)((idx / cnt) * Owner.height);
            }

            public void DisconnectAll()
            {
                foreach (TriggerInput i in Connected)
                {
                    i.Connected.Remove(this);
                }
                Connected.Clear();
            }
        }

        /// <summary>
        /// returns a list of codeblocks this code block is dependent on, also this codeblock can never connect anything to one of these codeblocks
        /// </summary>
        /// <returns></returns>
        public CodeBlock[] GetDependencies()
        {
            List<CodeBlock> list = new List<CodeBlock>();
            list.Add(this);

            return list.ToArray();
        }

        public virtual void Draw(System.Drawing.Graphics _Graphics)
        {
#if false
            _Graphics.DrawString("idx:" + index.ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, X, Y - 25);
            _Graphics.DrawString("depth:" + GetDepth().ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, X, Y - 35);
            if (Scope != null) _Graphics.DrawString("scope:" + Scope.ToString(), new System.Drawing.Font("Arial", 8), System.Drawing.Brushes.Black, X, Y - 45);
#endif
        }

        public virtual void DrawShadow(System.Drawing.Graphics _Graphics)
        {
            //            _Graphics.FillEllipse(Brushes.Black, new RectangleF(X + ScopeDepth * 12, Y + ScopeDepth * 50, width, height));
        }

        public class Prototype
        {
            public string BlockName;
            public string GroupName;
            public bool UserCanAdd;
            public Type Type;

            public Prototype(string _BlockName, string _GroupName, Type _Type, bool _UserCanAdd)
            {
                BlockName = _BlockName;
                GroupName = _GroupName;
                Type = _Type;
                UserCanAdd = _UserCanAdd;
            }
            public override string ToString()
            {
                return BlockName;
            }
        }

        public class DataType
        {
            public DataType(string _Name, Type _Type, object _DefaultValue, int _ID, Color _Color, int _RegistersNeeded)
            {
                Name = _Name;
                ID = _ID;
                Color = _Color;
                Type = _Type;
                DefaultValue = _DefaultValue;
                RegistersNeeded = _RegistersNeeded;
            }
            public int ID;
            public string Name;
            public Color Color;
            public Type Type;
            public object DefaultValue;
            public int RegistersNeeded;

            public object GetDefaultValue()
            {
                return DefaultValue;
            }
        }

        public static List<DataType> DataTypes = null;

        public static void InitDataTypes()
        {

        }

        public static DataType GetDataType(string _Name)
        {
            if (DataTypes == null) return null;
            foreach (DataType t in DataTypes) if (t.Name.ToLower() == _Name.ToLower()) return t;
            return null;
        }

        private static void AddDataType(string _TypeName, Type _Type, object _DefaultInstance, int _ID, Color _Color, int _RegistersNeeded)
        {
            DataTypes.Add(new DataType(_TypeName, _Type, _DefaultInstance, _ID, _Color, _RegistersNeeded));
        }
    }
}
