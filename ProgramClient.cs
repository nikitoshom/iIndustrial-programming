using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chatClient
{
    static class ProgramClient
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]


        // Main))   
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new chatForm());
        }
    }
}
