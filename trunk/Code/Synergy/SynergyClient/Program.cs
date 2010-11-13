using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Synergy;

namespace SynergyClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            NetworkManager.Initialize();
            Application.Idle += new EventHandler(Application_Idle);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void Application_Idle(object sender, EventArgs e)
        {
            NetworkManager.Update();
        }
    }
}
