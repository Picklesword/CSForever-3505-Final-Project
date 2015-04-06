using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace spreadsheetclient
{
    public partial class ssClient : Form
    {
        public ClientModel model;
        private Action SetupInv(Action a)
        {
            return () => { try { this.Invoke((MethodInvoker)delegate { a(); }); } catch { } };
        }

        private Action<T> SetupInv<T>(Action<T> a)
        {
            return (t) => { try { this.Invoke((MethodInvoker)delegate { a(t); }); } catch { } };
        }

        private Action<Exception, string> SetupInv(Action<Exception, string> a)
        {
            return (e, s) => { try { this.Invoke((MethodInvoker)delegate { a(e, s); }); } catch { } };
        }

        private Action<int, int> SetupInv(Action<int, int> a)
        {
            return (one, two) => { try { this.Invoke((MethodInvoker)delegate { a(one, two); }); } catch { } };
        }

        public ssClient()
        {
            InitializeComponent();
        }

        private void LoginNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerIPTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void connectedToServer()
        {
            model.SendName(LoginNameTextBox.Text);
            ConnectButton.Text = "Waiting for partner...";
        }

        private void failedToConnectToServer(String errorMessage)
        {
            ConnectButton.Enabled = true;
            MessageBox.Show("Could not connect to Server!\n" + errorMessage.ToString());
            ConnectButton.Text = "Connect";
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //  MessageBox.Show(model.ConnectToServer("localhost"));
            //changeView(view.play);
            if (LoginNameTextBox.Text == "")
            {
                MessageBox.Show("Please enter a name.");
                return;
            }
            if (ServerIPTextBox.Text == "")
            {
                MessageBox.Show("Please enter the server's IP Address.");
                return;
            }

            ConnectButton.Enabled = false;
            ConnectButton.Text = "Connecting...";
            model.ConnectToServer(ServerIPTextBox.Text,
                SetupInv(connectedToServer),
                SetupInv<string>(failedToConnectToServer));
        }
    }
}
