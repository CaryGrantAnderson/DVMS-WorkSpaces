using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Drawing;
using System.Text;
using System.Reflection;

namespace LnkFileExplorer
{
    static class Program
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string assemblyPath = Program.assembly.GetName().Name.Replace(' ', '_');

        public static System.IO.Stream GetResource(string fileName)
        {
            return Program.assembly.GetManifestResourceStream(Program.assemblyPath + '.' + fileName);
        }
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}