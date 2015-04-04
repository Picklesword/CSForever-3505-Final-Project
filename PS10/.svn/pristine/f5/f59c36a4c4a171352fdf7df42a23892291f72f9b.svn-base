using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoggleClient;

namespace PS9_Spawner
{
    /// <summary>
    /// Can open new BoggleClients and close all BoggleClients that it opened.
    /// </summary>
    static class OpenedBoggleClients
    {
        static List<BoggleClientForm> allOpenClients = new List<BoggleClientForm>();

        /// <summary>
        /// Closes all of the boggle clients that this class opened.
        /// </summary>
        public static void closeAll()
        {
            //Close all boggle clients
            foreach (BoggleClientForm b in allOpenClients)
                b.Close();
        }

        /// <summary>
        /// Opens more boggle clients.
        /// </summary>
        /// <param name="n">The number of clients to open.</param>
        public static void openMoreClients(int n)
        {
            //Create and show new boggle client form
             BoggleClientForm newBCF;
             for (int i = 0; i < n; i++)
             {
                 newBCF = new BoggleClientForm();
                 newBCF.Show();
                 //Remember the form so that we can force close it later.
                 allOpenClients.Add(newBCF);
             }
        }
    }
}
