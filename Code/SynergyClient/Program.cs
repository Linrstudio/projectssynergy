using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SynergyNode;

namespace SynergyClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ConnectionManager.Init();
            ConnectionManager.UpdateAsync();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new f_Main());
        }
    }
}
