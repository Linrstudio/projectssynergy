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
            //Framework.PluginManager.LoadPlugin(@".\plugins\k8055.cs");
            var bob = new Framework.LocalPlugin("k8055", System.IO.File.ReadAllText(@".\plugins\k8055.cs"));
            bob.InvokeEntries();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrontEnd());
        }
    }
}
