using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;

namespace FrontEnd
{
    public partial class UI : Form
    {
        public UI()
        {
            InitializeComponent();
            PluginManager.LoadPlugin(@".\plugins\k8055.cs");
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
            Update();
        }

        bool lastthree = false;
        bool lastfour = false;

        public void Update()
        {
            bool three = (bool)NetworkManager.LocalNode.NetworkClasses["digital in 3"].GetField("On");
            bool four = (bool)NetworkManager.LocalNode.NetworkClasses["digital in 4"].GetField("On");

            if (three != lastthree && three)
            {
                Turn(0, false);
                Turn(1, false);
            }

            if (four != lastfour && four)
            {
                Turn(0, true);
                Turn(1, true);    
            }

            lastthree = three;
            lastfour = four;

            pictureBox1.BackColor = (bool)NetworkManager.LocalNode.NetworkClasses["digital in 1"].GetField("On") ? Color.Green : Color.Red;
            pictureBox2.BackColor = (bool)NetworkManager.LocalNode.NetworkClasses["digital in 2"].GetField("On") ? Color.Green : Color.Red;
        }

        public void Toggle(int Channel)
        {
            bool on = (bool)NetworkManager.LocalNode.NetworkClasses["digital out " + (Channel + 1).ToString()].GetField("On");
            NetworkManager.LocalNode.NetworkClasses["digital out " + (Channel + 1).ToString()].SetField("On", !on);
        }

        public void Turn(int Channel, bool _On)
        {
            if ((bool)NetworkManager.LocalNode.NetworkClasses["digital in "+(Channel+1).ToString()].GetField("On") != _On)
                Toggle(Channel);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Toggle(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Toggle(1);
        }

        private void UI_Load(object sender, EventArgs e)
        {

        }
    }
}
