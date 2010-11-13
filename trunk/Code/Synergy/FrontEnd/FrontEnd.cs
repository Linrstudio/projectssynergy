using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Framework;
using SynergyTemplate;

namespace FrontEnd
{
    public partial class FrontEnd : Form
    {
        public bool update = true;
        public Dictionary<string, CheckBox> enabledlogs = new Dictionary<string, CheckBox>();
        public FrontEnd()
        {
            NetworkClassSlave.AllowedTypes.Add(typeof(K8055.testclass));
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.EnableNotifyMessage | ControlStyles.UserPaint, true);
            InitializeComponent();
            Log.Write("user input", "user input log added");
        }

        public void RebuildEnabledLogsList()
        {
            foreach (Log.Line s in Log.AllLines)
            {
                AddLog(s.LogName, true);
            }
            foreach (Log.Variable s in Log.AllVariables)
            {
                AddLog(s.LogName, true);
            }
        }

        public void OnLogChecked(object _Sender, EventArgs _Args)
        {
            RebuildLogText();
            RebuildVariables();
        }

        public void AddLog(string _name, bool _Enabled)
        {
            if (!enabledlogs.ContainsKey(_name))
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Checked = _Enabled;
                checkbox.Text = _name;
                checkbox.AutoSize = true;
                checkbox.Dock = DockStyle.Left;
                checkbox.Name = "logenabled " + _name;
                checkbox.CheckedChanged += OnLogChecked;
                p_EnabledLogs.Controls.Add(checkbox);
                enabledlogs.Add(_name, checkbox);
            }
        }

        public void UpdateAll()
        {
            RebuildEnabledLogsList();
            RebuildLogText();
            RebuildVariables();
        }

        public void OnLogAdded(Log _AddedLog)
        {
            update = true;
        }

        public void OnWriteLine(Log.Line _LogLine)
        {
            update = true;
        }

        public void OnWriteVariable(Log.Variable _LogVariable)
        {
            update = true;
        }

        public void RebuildLogText()
        {
            DataGridView log = d_Log;
            int selectedindex = log.SelectedRows.Count>0?log.SelectedRows[0].Index:log.Rows.Count;
            log.Rows.Clear();
            log.Text = "";
            foreach (Log.Line line in Log.AllLines)
            {
                if (line.LogName != null && enabledlogs.ContainsKey(line.LogName) && enabledlogs[line.LogName].Checked)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Height = 16;
                    switch (line.type)
                    {
                        case Log.Line.Type.Message:
                            row.DefaultCellStyle.ForeColor = Color.White;
                            break;
                        case Log.Line.Type.Error:
                            row.DefaultCellStyle.ForeColor = Color.Red;
                            break;
                        case Log.Line.Type.Warning:
                            row.DefaultCellStyle.ForeColor = Color.Yellow;
                            break;
                    }
                    row.CreateCells(log, line.Time.ToShortTimeString(), line.LogName, line.Message);
                    row.Cells[0].ToolTipText = line.Time.ToString();
                    log.Rows.Add(row);
                    row.Selected = false;
                }
            }
        }

        public void RebuildVariables()
        {
            DataGridView log = d_Variables;
            log.Rows.Clear();
            log.Text = "";
            foreach (Log.Variable variable in Log.AllVariables)
            {
                if (variable.LogName != null && enabledlogs.ContainsKey(variable.LogName) && enabledlogs[variable.LogName].Checked)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Height = 16;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    row.CreateCells(log, variable.Time.ToShortTimeString(), variable.LogName, variable.VariableName, variable.Value.ToString());
                    row.Cells[0].ToolTipText = variable.Time.ToString();
                    log.Rows.Add(row);
                }
            }
        }

        private void FrontEnd_Load(object sender, EventArgs e)
        {
            RebuildEnabledLogsList();
            RebuildLogText();
            Log.OnLogAdded += OnLogAdded;
            Log.OnWriteLine += OnWriteLine;
            Log.OnWriteVariable += OnWriteVariable;

            Application.Idle += t_Tick_Tick;
            c_Input.Select();
            //ExecuteCommand(@"load .\plugins\K8055.cs");
            ExecuteCommand("connect 192.168.1.93 1000");
        }

        private void c_Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 || e.KeyChar == 10)
            {
                ExecuteCommand(c_Input.Text);
                c_Input.Text = "";
            }
        }

        public void ExecuteCommand(string _Command)
        {
            c_Input.Items.Add(_Command);
            string[] split = _Command.Split(' ');
            //try
            {
                switch (split[0].ToLower())
                {
                    case "clear":
                        Log.Clear();
                        break;
                    case "setout":
                        NetworkManager.RemoteNodes["DESKTOP-Roeny"].LocalDevices[string.Format("digital out {0}",split[1])].SetMasterField("On",bool.Parse(split[2]));
                        break;
                    case ">":
                        NetworkManager.InvokeCommand(split[1]);
                        break;
                    case "connect":
                        if (split.Length > 1)
                            new TCPConnection(split[1], ushort.Parse(split[2]), false);
                        else
                            new TCPConnection("127.0.0.1", 1000, false);
                        break;
                    case "listen":
                        if (split.Length > 1)
                            new TCPListener(ushort.Parse(split[1]));
                        else
                            new TCPListener(1000);
                        break;
                    case "load":
                        new MasterPlugin("k8055", System.IO.File.ReadAllText(split[1]));
                        break;
                    case "deletenetworkclasses":
                        NetworkManager.LocalNode.NetworkClasses.Clear();
                        Log.Write("user input", "The deed is done");
                        break;
                    case "map":
                        NetworkManager.RequestNetworkMap();
                        break;
                    case "break":
                        Debugger.Break();
                        break;
                    case "exit":
                        Process.GetCurrentProcess().Kill();
                        break;
                    default:
                        Log.Write("user input", Log.Line.Type.Error, "Unknown command {0}", _Command);
                        break;
                }
            }
            //catch (Exception e) { Log.Write("user input", e.Message.ToString()); }
            //Log.Write("user input", _Command); c_Input.Items.Add(_Command);
        }

        private void FrontEnd_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        int lasttick = Environment.TickCount;
        int lastfpsupdatetick = 0;
        private void t_Tick_Tick(object sender, EventArgs e)
        {
            NetworkManager.Update();
            int curtick = Environment.TickCount;
            int delta = curtick - lasttick;
            lasttick = curtick;
            if (lastfpsupdatetick < curtick - 1000)
            {
                lastfpsupdatetick = curtick;
                Log.Write(new Log.Variable("Default", "Framework FPS", 1000 / delta));
                if (update)
                {
                    UpdateAll();
                    update = false;
                }
            }
        }

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            DataGridView grid = d_Log;
            int width = grid.Columns[0].Width;
            e.Graphics.DrawLine(new Pen(Color.FromArgb(32, 32, 32), 2), width, 0, width, grid.Height);
            width += grid.Columns[1].Width;
            e.Graphics.DrawLine(new Pen(Brushes.Gray, 2), width, 0, width, grid.Height);
        }

        private void d_Variables_Paint(object sender, PaintEventArgs e)
        {
            DataGridView grid = d_Variables;
            int width = grid.Columns[0].Width;
            e.Graphics.DrawLine(new Pen(Color.FromArgb(32, 32, 32), 2), width, 0, width, grid.Height);
            width += grid.Columns[1].Width;
            e.Graphics.DrawLine(new Pen(Brushes.Gray, 2), width, 0, width, grid.Height);
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14) base.OnNotifyMessage(m);
        }
    }
}
