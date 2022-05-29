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
using FontAwesome.Sharp;

namespace Wordle
{
    public partial class mainMenu : Form
    {

        private IconButton currentButton;
        private Panel leftBorderButton;
        private Form currentChildForm;
        public mainMenu()
        {
            
            InitializeComponent();
            leftBorderButton = new Panel();
            leftBorderButton.Size = new Size(7, 60);
            panelMenu.Controls.Add(leftBorderButton);

            //Form
            this.Text = String.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;

        }

        private struct RGBColors
        {
            public static Color color1 = Color.FromArgb(172, 126, 241);
            public static Color color2 = Color.FromArgb(249, 118, 176);
            public static Color color3 = Color.FromArgb(253, 138, 114);
            public static Color color4 = Color.FromArgb(95, 77, 221);
            public static Color color5 = Color.FromArgb(249, 88, 155);
            public static Color color6 = Color.FromArgb(24, 161, 251);
            public static Color color7 = Color.FromArgb(51, 51, 76);
            public static Color color11 = Color.FromArgb(142, 96, 211);
            public static Color color22 = Color.FromArgb(219, 88, 146);
            public static Color color33 = Color.FromArgb(223, 108, 84);
            public static Color color44 = Color.FromArgb(65, 47, 191);
        }

        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                //Button
                currentButton = (IconButton)senderBtn;
                currentButton.BackColor = Color.FromArgb(37, 36, 81);
                currentButton.ForeColor = color;
                currentButton.TextAlign = ContentAlignment.MiddleCenter;
                currentButton.IconColor = color;
                currentButton.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentButton.ImageAlign = ContentAlignment.MiddleRight;
                //Left border button
                leftBorderButton.BackColor = color;
                leftBorderButton.Location = new Point(0, currentButton.Location.Y);
                leftBorderButton.Visible = true;
                leftBorderButton.BringToFront();

                iconCurrent.IconChar = currentButton.IconChar;
                iconCurrent.IconColor = color;

            }
        }

        private void DisableButton()
        {
  
            if (currentButton != null)
            {
                        currentButton.BackColor = Color.FromArgb(51, 51, 75);
                        currentButton.ForeColor = Color.Gainsboro;
                        currentButton.TextAlign = ContentAlignment.MiddleLeft;
                        currentButton.IconColor = Color.Gainsboro;
                        currentButton.TextImageRelation = TextImageRelation.ImageBeforeText;
                        currentButton.ImageAlign = ContentAlignment.MiddleLeft;
            }
    
        }

        private void OpenChildForm(Form childForm)
        {
            if(currentChildForm != null)
            {
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();   
            childForm.Show();
            lblTitle.Text = childForm.Text;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color1);
            panelTitleBar.BackColor = RGBColors.color1;
            btnHome.BackColor = RGBColors.color11;
            OpenChildForm(new Game());

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            panelTitleBar.BackColor = RGBColors.color2;
            btnHome.BackColor = RGBColors.color22;
            OpenChildForm(new Login());

        }

        private void btnRegistration_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color3);
            panelTitleBar.BackColor = RGBColors.color3;
            btnHome.BackColor = RGBColors.color33;
            OpenChildForm(new Registration());

        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color4);
            panelTitleBar.BackColor = RGBColors.color4;
            btnHome.BackColor = RGBColors.color44;
            OpenChildForm(new ClientType());
            //OpenChildForm(new Multiplayer());
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color5);
            Application.Exit();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            panelTitleBar.BackColor = RGBColors.color7;
            btnHome.BackColor = RGBColors.color7;
            currentChildForm.Close();
            Reset();
        }

        private void Reset()
        {
            DisableButton();
            leftBorderButton.Visible = false;
            iconCurrent.IconChar = IconChar.Home;
            iconCurrent.IconColor = Color.Gainsboro;
            lblTitle.Text = "Home";
        }

        //Drag Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }
            base.WndProc(ref m);
        }



        private void iconPictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void iconPictureBox2_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
                WindowState = FormWindowState.Maximized;
            else
                WindowState = FormWindowState.Normal;
        }

        private void iconPictureBox3_Click(object sender, EventArgs e)
        {

            WindowState = FormWindowState.Minimized;
            
        }

        private void mainMenu_Load(object sender, EventArgs e)
        {

        }
    }
}
