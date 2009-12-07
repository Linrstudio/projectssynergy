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
            Framework.NetworkManager.StartUpdateAsync();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrontEnd());
        }
    }
}
