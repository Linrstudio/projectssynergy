using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Synergy;

namespace FrontEnd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Converter.Initialize();
            NetworkManager.Initialize();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Form mainform = new Synergy.FrontEnd();
            mainform.FormClosing += new FormClosingEventHandler(mainform_FormClosing);
            Application.Run(mainform);
        }

        static void mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
