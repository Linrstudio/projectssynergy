using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
            Framework.NetworkManager.Initialize();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Form mainform = new Framework.FrontEnd();
            mainform.FormClosing += new FormClosingEventHandler(mainform_FormClosing);
            Application.Run(mainform);
        }

        static void mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }


    }
}
