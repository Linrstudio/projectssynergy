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

        public void OnLogWrite(LogLine _LogLine)
        {
            RebuildEnabledLogsList();
            RebuildLogText();
        }

        public void RebuildLogText()
        {
            DataGridView log = dataGridView1;
            log.Rows.Clear();
            log.Text = "";
            foreach (Log l in Log.log.Values)
            {
                if (l.Name != null && enabledlogs.ContainsKey(l.Name) && enabledlogs[l.Name].Checked)
                {
                    foreach (LogLine line in l.Entries)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.Height = 14;
                        switch (line.type)
                        {
                            case LogLine.Type.Message:
                                row.DefaultCellStyle.ForeColor = Color.White;
                                break;
                            case LogLine.Type.Error:
                                row.DefaultCellStyle.ForeColor = Color.Red;
                                break;
                            case LogLine.Type.Warning:
                                row.DefaultCellStyle.ForeColor = Color.Yellow;
                                break;
                        }
                        row.CreateCells(log, line.Time.ToLocalTime(), l.Name, line.Message);
                        log.Rows.Add(row);
                    }
                }
            }
            log.Sort(log.Columns[0], ListSortDirection.Ascending);
            log.Text.Trim();
        }

        private void FrontEnd_Load(object sender, EventArgs e)
        {
            RebuildEnabledLogsList();
            RebuildLogText();
            Log.OnLogAdded += OnLogAdded;
            Log.OnLogWrite += OnLogWrite;
            c_Input.Select();
            CheckForIllegalCrossThreadCalls = false;
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
            switch (split[0])
            {
                case "try":
                    RebuildEnabledLogsList();
                    break;
                case "connect":
                    new TCPConnection(split[1], 1111, false);
                    break;
                case "listen":
                    try
                    {
                        new TCPListener(ushort.Parse(split[1]));
                    }
                    catch { Log.Write("user input","arguments make no sense at all -_-"); }
                    break;
                case "break":
                    Debugger.Break();
                    break;
            }
        }

        private void FrontEnd_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
