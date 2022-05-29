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
    public partial class Registration : Form
    {
        Game g = new Game();
        private SqlConnection con = new SqlConnection("Data Source =bmw-io.database.windows.net; Initial Catalog=SlowkaIO; User ID=bmw; Password=Azure321?");
        private SqlCommand cmd = new SqlCommand();

        [DllImport("user32.dll")]

        static extern bool HideCaret(IntPtr hWnd);

        public Registration()
        {
            InitializeComponent();
        }

        private void Registration_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox2;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"INSERT INTO ACCOUNTS(email,nick,password) 
                            VALUES(@email,@nick,@pass)";
            cmd.Parameters.AddWithValue("@email", textBox1.Text);
            cmd.Parameters.AddWithValue("@nick", textBox2.Text);
            cmd.Parameters.AddWithValue("@pass", textBox3.Text);
            con.Open();
            if(textBox3.Text.Equals(textBox4.Text))
            {
                if (isValid())
                {
                    MessageBox.Show("Login or E-mail already in use");
                    cmd.Parameters.Clear();
                }
                else
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        MessageBox.Show("Account Created");
                        this.Hide();
                        Login loginform = new Login();
                        loginform.Show();

                    }
                    else
                    {
                        MessageBox.Show("Error");
                        cmd.Parameters.Clear();
                    }
                }
            }
            else
            {
                MessageBox.Show("Passwords not equal");
                cmd.Parameters.Clear();
            }
            con.Close();

        }
        public bool isValid()
        {
            bool exists = false;
            SqlCommand cmd = new SqlCommand("Select count(*) from accounts where nick=@username OR email=@email", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@username", textBox1.Text);
            cmd.Parameters.AddWithValue("@email", textBox2.Text);
            exists = (int)cmd.ExecuteScalar() > 0;
            if (exists)
            {
                return true;
            }
            return false;
        }
    }
  
}
