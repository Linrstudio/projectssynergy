using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MainStationFrontEnd
{
    public partial class ScheduleEditor : UserControl
    {
        public const int CellX = 48;
        public const int CellY = 32;
        public int CellWidth = 128;
        public const int CellHeight = 16;

        ushort selectedday;
        public ushort SelectedDay
        {
            set { selectedday = value; Invalidate(); }
            get { return selectedday; }
        }

        public class Element
        {

        }

        public ScheduleEditor()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }

        void Parent_Resize(object sender, EventArgs e)
        {
            OnResize(e);
        }

        private void SheduleEditor_Load(object sender, EventArgs e)
        {
            Height = 24 * CellHeight * 2;
            Parent.Resize += new EventHandler(Parent_Resize);
        }

        public int GetCellYForPoint(int _Y)
        {
            return (_Y - CellY) / CellHeight;
        }

        public int GetCellXForPoint(int _X)
        {
            return (_X - CellX) / CellHeight;
        }

        public int GetCellYForCell(int _Y)
        {
            return _Y * CellHeight + CellY;
        }

        public int GetCellXForCell(int _X)
        {
            return _X * CellWidth + CellX;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Font timefont = new Font("Arial", 12);
            Font entryfont = new Font("Arial", 12);

            StringFormat timefontformat = new StringFormat();

            timefontformat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

            for (int h = 0; h < 24; h++)
            {
                e.Graphics.DrawString(string.Format("{0:00}:00", h), timefont, Brushes.Black, CellX, (-timefont.Height / 2) + (h + 0.25f) * CellHeight * 2, timefontformat);
                e.Graphics.DrawLine(Pens.Black, CellX, h * CellHeight * 2, Width, h * CellHeight * 2);
                e.Graphics.DrawLine(Pens.DarkGray, CellX, h * CellHeight * 2 + CellHeight, Width, h * CellHeight * 2 + CellHeight);
                //e.Graphics.DrawLine(Pens.DarkGray, CellX, (h + 0.5f) * CellHeight * 2 + CellHeight, Width, (h + 0.5f) * CellHeight * 2 + CellHeight);
                //e.Graphics.DrawLine(Pens.Black, 0, (h + 1) * CellHeight*2, CellX, (h + 1) * CellHeight * 2);
            }

            foreach (EEPROM.ScheduleEntry entry in EEPROM.ScheduleEntries)
            {
                if (SelectedDay == entry.Days)
                {
                    e.Graphics.DrawString(entry.Name, entryfont, Brushes.Black, CellX, (-timefont.Height / 2) + (entry.Hours + 0.25f) * CellHeight * 2);
                }
            }

            int x = CellX;
            e.Graphics.DrawLine(Pens.Black, x, 0, x, Height);

            base.OnPaint(e);
        }
        protected override void OnResize(EventArgs e)
        {
            Height = 24 * CellHeight * 2;
            if (Parent != null)
                Width = Bounds.Width;

            Refresh();
            base.OnResize(e);
        }
    }
}
