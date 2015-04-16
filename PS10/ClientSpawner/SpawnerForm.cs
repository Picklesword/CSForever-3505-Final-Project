using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PS9_Spawner
{
    public partial class SpawnerForm : Form
    {
        public SpawnerForm()
        {
            InitializeComponent();

            //Center the text to make it look nicer  //Why can't I find a way to display that in the form design??
            richTextBox1.SelectAll();
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void plus1_Click(object sender, EventArgs e)
        {
            //Open one more boggle client
            OpenedBoggleClients.openMoreClients(1);
        }

        private void plus2_Click(object sender, EventArgs e)
        {
            //Open two more boggle clients
            OpenedBoggleClients.openMoreClients(2);
        }

        private void closeAllClients_Click(object sender, EventArgs e)
        {
            //Close all boggle clients
            OpenedBoggleClients.closeAll();
        }

    }
}
