﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Framework;

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
            Framework.NetworkManager.Initialize();
            new TCPConnection("127.0.0.1", 1000, false);
            //new Framework.TCPListener(1000);

            Application.Idle += new EventHandler(Application_Idle);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void Application_Idle(object sender, EventArgs e)
        {
            Framework.NetworkManager.Update();
        }
    }
}