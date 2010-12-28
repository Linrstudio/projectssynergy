using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;


namespace MainStationCodeBlocks
{
    public abstract class BaseBlockInstruction : MainStationCodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawShapeShadow(_Graphics,
                new PointF(-width / 2, -height / 2),
                new PointF(-width / 2, height / 2),
                new PointF(width / 2, height / 2),
                new PointF(width / 2, -height / 2));
            base.DrawShadow(_Graphics);
        }
    }

    public abstract class BaseBlockData : MainStationCodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawConstant(_Graphics);
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawConstantShadow(_Graphics);
            base.DrawShadow(_Graphics);
        }
    }

    public abstract class BaseBlockEvent : MainStationCodeBlock
    {
        public override void Draw(Graphics _Graphics)
        {
            DrawShape(_Graphics,
             new PointF(-width / 2, height / 4),
             new PointF(-width / 2, -height / 4),
             new PointF(0, -height / 2),
             new PointF(width / 2, -height / 4),
             new PointF(width / 2, height / 4),
             new PointF(0, height / 2));
            base.Draw(_Graphics);
        }

        public override void DrawShadow(Graphics _Graphics)
        {
            DrawShapeShadow(_Graphics,
             new PointF(-width / 2, height / 4),
             new PointF(-width / 2, -height / 4),
             new PointF(0, -height / 2),
             new PointF(width / 2, -height / 4),
             new PointF(width / 2, height / 4),
             new PointF(0, height / 2));
            base.DrawShadow(_Graphics);
        }
    }

    public class BlockBoolInvert : BaseBlockInstruction
    {
        public BlockBoolInvert()
        {
            width = 100;
            height = 50;
            DataInputs.Add(new DataInput(this, "", "bool"));
            DataOutputs.Add(new DataOutput(this, "", "bool"));
            UpdateConnectors();
            IsEvent = true;
            Name = "Invert";
        }
    }

    public class BlockBoolConstant : BaseBlockData
    {
        bool val;
        [Browsable(true), CategoryAttribute("Constant")]
        public bool Value { get { return val; } set { val = value; } }

        public BlockBoolConstant()
        {
            width = 50;
            height = 50;
            DataOutputs.Add(new DataOutput(this, "", "bool"));
            UpdateConnectors();
            IsEvent = true;
            Name = "Constant";
        }

        public override void Load(XElement _Data) { val = bool.Parse(_Data.Value); }
        public override void Save(XElement _Data) { _Data.Value = val.ToString(); }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString(val.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }
    }

    public class BlockEventSchedule : BaseBlockEvent
    {
        DateTime moment;

        [Browsable(true)]
        public DateTime Moment
        {
            get { return moment; }
            set { moment = value; }
        }

        public BlockEventSchedule()
        {
            width = 100;
            height = 50;
            TriggerOutputs.Add(new TriggerOutput(this, ""));
            UpdateConnectors();
            IsEvent = true;
        }

        public override void DrawText(Graphics _Graphics)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            _Graphics.DrawString("Schedule\n" + moment.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, X, Y, sf);
        }

        public override void Load(XElement _Data) { }
        public override void Save(XElement _Data) { }
    }
}
