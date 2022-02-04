using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FSIndexer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Directory.GetFiles(@"E:\Downloads\NG\_Decoded\lp").Select(i => new FileInfo(i)).ToList().ForEach(f => File.Move(f.FullName, Path.Combine(f.DirectoryName, "lp." + f.Name)));

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main(args));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
