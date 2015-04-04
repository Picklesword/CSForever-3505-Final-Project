using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PS9_Spawner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Try to bring up the Server
            try
            {
                Process.Start(@"PS8.exe", @"15 ..\..\..\dictionary.txt AOHUINHRTSOAFHTE");
            }
            catch
            {
                //if failed, then simply continue
            }

            //Bring up the main form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SpawnerForm()); //Blocks this thread until form closes

            //Close all of the clients that were spawned
            OpenedBoggleClients.closeAll();

            //If the server console is still up, then end the process.
            Process[] proc = Process.GetProcessesByName("PS8");
            if (proc.Count() > 0)
                proc[0].Kill();
        }
    }
}
