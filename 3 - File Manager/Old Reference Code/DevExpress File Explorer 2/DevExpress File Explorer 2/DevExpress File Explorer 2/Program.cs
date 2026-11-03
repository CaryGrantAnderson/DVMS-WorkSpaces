using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraGrid.Demos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress_File_Explorer_2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new Form1());
            Application.Run(new WinExplorer());


        }
    }
}
