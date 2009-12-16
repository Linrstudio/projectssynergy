using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Framework;

namespace FrontEnd
{
    public partial class FrontEnd : Form
    {
        public Dictionary<string, CheckBox> enabledlogs = new Dictionary<string, CheckBox>();
        public FrontEnd()
        {
            InitializeComponent();
            Log.Write("user input", "user input log added");
        }

        public void RebuildEnabledLogsList()
        {
            foreach (string s in Log.log.Keys)
            {
                AddLog(s, true);
            }
        }

        public void OnLogChecked(object _Sender, EventArgs _Args)
        {
            RebuildLogText();
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

        public void OnLogAdded(Log _AddedLog)
        {
            RebuildEnabledLogsList();
            RebuildLogText();
        }

        public void OnLogWrite(Log.Line _LogLine)
        {
            RebuildEnabledLogsList();
            RebuildLogText();
        }

        public void RebuildLogText()
        {
            DataGridView log = dataGridView1;
            log.Rows.Clear();
            log.Text = "";
            foreach (Log.Line line in Log.AllEntries)
            {
                if (line.LogName != null && enabledlogs.ContainsKey(line.LogName) && enabledlogs[line.LogName].Checked)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Height = 16;
                    //row.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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
                    row.CreateCells(log, line.Time.ToLocalTime(), line.LogName, line.Message);
                    log.Rows.Add(row);
                }
            }
        }

        private void FrontEnd_Load(object sender, EventArgs e)
        {
            RebuildEnabledLogsList();
            RebuildLogText();
            Log.OnLogAdded += OnLogAdded;
            Log.OnLogWrite += OnLogWrite;
            c_Input.Select();
            ExecuteCommand(@"openui");
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
            string[] split = _Command.Split(' ');
            try
            {
                switch (split[0].ToLower())
                {
                    case "try":
                        NetworkManager.LocalNode.NetworkClasses["analog out 1"].GetMethods();
                        break;
                    case "connect":
                        if (split.Length > 1)
                            new TCPConnection(split[1], ushort.Parse(split[2]), false);
                        else
                            new TCPConnection("127.0.0.1", 1111, false);
                        break;
                    case "listen":
                        if (split.Length > 1)
                            new TCPListener(ushort.Parse(split[1]));
                        else 
                            new TCPListener(1111);
                        break;
                    case "load":
                        PluginManager.LoadPlugin(split[1]);
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
                    case "openui":
                        UI ui = new UI();
                        ui.Show();
                        break;
                    default:
                        Log.Write("user input", Log.Line.Type.Error, "Unknown command {0}", _Command);
                        throw new Exception();
                }
            }
            catch (Exception e) { Log.Write("user input", e.Message.ToString()); }
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
            if(lastfpsupdatetick<curtick-100)
            {
                lastfpsupdatetick=curtick;
                Text = string.Format("FPS:{0}", 1000 / delta);
            }
        }

        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            DataGridView grid = dataGridView1;
            int width = grid.Columns[0].Width + grid.Columns[1].Width;
            e.Graphics.DrawLine(new Pen(Brushes.White, 2), width, 0, width, grid.Height);
        }
    }
}
