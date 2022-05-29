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
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Wordle
{
    public partial class Login : Form
    {
        private SqlConnection con = new SqlConnection("Data Source =bmw-io.database.windows.net; Initial Catalog=SlowkaIO; User ID=bmw; Password=Azure321?");
        private SqlCommand cmd = new SqlCommand();
        Multiplayer g = new Multiplayer();

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.Open();
            string username = textBoxUsername.Text;
            string email = textBoxEmail.Text;
            string password = textBoxPassword.Text;
            //cmd.CommandText = "insert into Accounts (email,nick,password) values ('" + email + "','" + username + "','" + password + "')";


            SqlCommand cmd = new SqlCommand("select id_user from accounts where email='"+email+"' AND nick='"+username+ "' AND password='"+password+"'", con);
            int result = Convert.ToInt32(cmd.ExecuteScalar());

            if (result != 0)
            {
                MessageBox.Show("Logged");
                this.Hide();
                g.Location = this.Location;
                g.Show();
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Podane konto nie istnieje.\nCzy chcesz je teraz utworzyć? ", "Error",MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Hide();
                    Registration registration = new Registration();
                    registration.ShowDialog();
                }
            }
            con.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBoxEmail;
            /*textBoxEmail.GotFocus += (s1, e1) => { HideCaret(textBoxEmail.Handle); };
            textBoxUsername.GotFocus += (s1, e1) => { HideCaret(textBoxUsername.Handle); };
            textBoxPassword.GotFocus += (s1, e1) => { HideCaret(textBoxPassword.Handle); };*/

        }
    }
}
