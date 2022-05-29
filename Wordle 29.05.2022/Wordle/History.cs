using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Wordle
{
    public partial class ClientType : Form
    {
        Multiplayer g = new Multiplayer();
        public static string client_type;
        public static string ip;
        public ClientType()
        {
            InitializeComponent();
            //groupBox2.Hide();
        }

        private void btnHost_Click(object sender, EventArgs e)
        {
            ip = txtIP.Text + ":9000";
            //var H = new Host();
            //H.startServer(ip);
            //if (H.is_listening())
            //{
                //btnConnect.Hide();
                //btnHost.Hide();
                //txtIP.Hide();
                //label1.Hide();
                //groupBox2.Show();
                client_type = "host";
                this.Hide();
                g.Location = this.Location;
                g.Show();
            //}
        }

        

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ip = txtIP.Text + ":9000";
            //var C = new Client();
            //C.startClient(ip);
            //if (C.is_connected())
            //{
                //btnConnect.Hide();
                //btnHost.Hide();
                //txtIP.Hide();
                //label1.Hide();
                //groupBox2.Show();
                client_type = "client";
                this.Hide();
                g.Location = this.Location;
                g.Show();
            //}
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
