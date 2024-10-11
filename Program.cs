using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.IO;

namespace GetDataToExcel
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
            conn = File.ReadAllText("conn");
            comm = File.ReadAllText("comm", System.Text.Encoding.UTF8);
            ablaknev = File.ReadAllText("ablaknev");
            Application.Run(new Form1());
        }

        public static string conn;
        public static string comm;
        public static string ablaknev;

    }
}
