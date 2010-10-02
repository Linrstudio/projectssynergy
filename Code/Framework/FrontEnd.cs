using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Synergy
{
    public partial class FrontEnd : Form
    {
        public bool update = true;
        public Dictionary<string, CheckBox> enabledlogs = new Dictionary<string, CheckBox>();
        public FrontEnd()
        {
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
            update = true;
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
                checkbox.Name = "logenabled_" + _name;
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
            ListView log = l_TextLog;
            log.Text = "";
            foreach (Log.Line line in Log.AllLines)
            {
                bool added = false;
                foreach (ListViewItem i in log.Items)
                {
                    if (i.Tag == line) added = true;
                }
                if (!added && line.LogName != null && (enabledlogs.ContainsKey(line.LogName) && enabledlogs[line.LogName].Checked))
                {
                    ListViewItem row = new ListViewItem(new string[] { line.Time.ToShortTimeString(), line.LogName, line.Message });
                    row.Tag = line;
                    switch (line.type)
                    {
                        case Log.Line.Type.Message: row.ForeColor = Color.White; break;
                        case Log.Line.Type.Error: row.ForeColor = Color.Red; break;
                        case Log.Line.Type.Warning: row.ForeColor = Color.Yellow; break;
                    }

                    int insertidx = log.Items.Count;
                    for (int i = 0; i < log.Items.Count; i++)
                    {
                        if (((Log.Line)log.Items[i].Tag).Time.Ticks > line.Time.Ticks) { insertidx = i; break; }
                    }
                    log.Items.Insert(insertidx, row);
                }
            }

            //check if any item needs to be removed
            while (true)
            {
                ListViewItem remove = null;
                foreach (ListViewItem i in log.Items)
                {
                    Log.Line line = (Log.Line)i.Tag;
                    if (!Log.AllLines.Contains(line) || !(enabledlogs.ContainsKey(line.LogName) && enabledlogs[line.LogName].Checked)) { remove = i; break; }
                }
                if (remove == null) break; log.Items.Remove(remove);
            }
            foreach (ColumnHeader c in log.Columns) c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);

            if (log.Items.Count > 0) log.Items[log.Items.Count - 1].EnsureVisible();
        }

        public void RebuildVariables()
        {
            ListView log = l_VariableLog;
            foreach (Log.Variable variable in Log.AllVariables)
            {
                bool added = false;
                foreach (ListViewItem i in log.Items)
                {
                    if (i.Tag == variable) added = true;
                }
                if (!added && variable.LogName != null && enabledlogs.ContainsKey(variable.LogName) && enabledlogs[variable.LogName].Checked)
                {
                    ListViewItem row = new ListViewItem(new string[] { variable.Time.ToShortTimeString(), variable.LogName, variable.VariableName, variable.Value.ToString() });
                    row.ForeColor = Color.White;
                    row.Tag = variable;

                    int insertidx = log.Items.Count;
                    for (int i = 0; i < log.Items.Count; i++)
                    {
                        if (((Log.Variable)log.Items[i].Tag).Time.Ticks > variable.Time.Ticks) { insertidx = i; break; }
                    }
                    log.Items.Insert(insertidx, row);
                }
            }
            //check if any item needs to be removed or update text if needed
            while (true)
            {
                ListViewItem remove = null;
                foreach (ListViewItem i in log.Items)
                {
                    Log.Variable line = (Log.Variable)i.Tag;
                    if (i.SubItems[3].Text != line.Value.ToString()) i.SubItems[3].Text = line.Value.ToString();
                    if (!Log.AllVariables.Contains(line) || !(enabledlogs.ContainsKey(line.LogName) && enabledlogs[line.LogName].Checked)) { remove = i; break; }
                }
                if (remove == null) break;
                log.Items.Remove(remove);
            }
            foreach (ColumnHeader c in log.Columns) c.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            if(log.Columns.Count>0)
            log.Width = log.Columns[0].Width + log.Columns[1].Width + log.Columns[2].Width + log.Columns[3].Width;
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
            ExecFunctions.Execute("run(\"startup.ss\");");
            //ExecFunctions.Execute("connect(\"192.168.1.93\",1000)");
        }

        private void c_Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                string text = c_Input.Text.Replace("\r", "");
                Log.Write("user input", text);
                ExecFunctions.Execute(text);
                c_Input.Items.Add(text);
                c_Input.Text = "";
            }
        }

        private void FrontEnd_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
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
            }
            if (update)
            {
                UpdateAll();
                update = false;
            }
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14) base.OnNotifyMessage(m);
        }

        private void l_TextLog_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.Graphics.DrawLine(Pens.White, 0, 0, 0, e.Bounds.Height);
        }

        private void c_Input_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("calc");
        }
    }
}

