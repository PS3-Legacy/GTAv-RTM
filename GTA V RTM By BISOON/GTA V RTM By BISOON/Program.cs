using System;
using System.Windows.Forms;

namespace GTA_V_RTM_By_BISOON
{
    static class Program
    {
        public static mainFrm main;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            main = new mainFrm();
            Application.Run(main);
        }
    }
}
