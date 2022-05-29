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

/*
 * Działająca gra -  skończone
 * Funkcjonalność klawiatury fizycznej i ekranowej w panelu programu - Keypreview w panelu?
 * Wyśrodkowanie panelu ze słowami
 * Przeskakiwanie na kolejne pole - Keypreview w panelu?
 * Wyłączenie "input caret" w textboxie
 * Przechwytywanie przez program podawanych znaków i wstawianie ich w odpowiednie miejsce  - Keypreview w panelu?
*/

namespace Wordle
{
    public partial class Game : Form
    {
        private SqlConnection con = new SqlConnection("Data Source =bmw-io.database.windows.net; Initial Catalog=SlowkaIO; User ID=bmw; Password=Azure321?");
        private SqlCommand cmd = new SqlCommand();
        private Random gen = new Random();
        private const int fieldSize = 20;
        private GameTextBox[] letters = new GameTextBox[42]; // tablica textboxów
        private int text_box_index, attempt = 0, x = 0, focus_index = 1;
        private string word;
        private int word_id, progress;
        private bool newGame;
        [System.ComponentModel.Browsable(false)]
        public event System.Windows.Forms.KeyPressEventHandler KeyPress;

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public Game()
        {
            KeyPreview = true;
            InitializeComponent();
            con.Open();
            PrepareNewGame(6);
            // MessageBox.Show(word);
        }

        private void newWord()
        {
            word_id = gen.Next(8214);
            SqlCommand cmd = new SqlCommand("select UPPER(word) from words where id_word =" + word_id, con);
            cmd.CommandType = CommandType.Text;
            word = (string)cmd.ExecuteScalar();
            text_box_index = 0;
        }
        private void panelGame_Paint(object sender, PaintEventArgs e)
        {
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            letters[0].Focus();
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Text == "ENTER")
            {
                SendKeys.Send("{ENTER}");
            }
            else if (btn.Text == "BACKSPACE")
            {
                SendKeys.Send("{BACKSPACE}");
            }
            else
            {
                SendKeys.Send(btn.Text);
            }

            letters[x].Focus();
        }

        private void panelGame_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        { }

        private void PrepareNewGame(int gameHeight)
        {
            panelGame.Controls.Clear();
            newWord();
            int gameWidth = word.Length;
            attempt = 0;

            for (int y = 0; y < gameHeight; y++)
            {
                for (int x = 0; x < gameWidth; x++)
                {
                    GameTextBox b = new GameTextBox();
                    if (y == 0 && x == 0)
                    {
                        b.Enabled = true;
                    }
                    else
                    {
                        b.Enabled = false;
                    }
                    b.MaxLength = 1;
                    b.CharacterCasing = CharacterCasing.Upper;
                    b.TextAlign = HorizontalAlignment.Center;
                    b.BackColor = Color.Black;
                    b.ForeColor = Color.White;
                    b.AutoSize = false;
                    b.Size = new Size(50, 50);
                    if (word.Length == 4)
                    {
                        b.Location = new Point(320 + 100 * x, 50 + 75 * y);
                    }
                    else if (word.Length == 5)
                    {
                        b.Location = new Point(270 + 100 * x, 50 + 75 * y);
                    }
                    else if (word.Length == 6)
                    {
                        b.Location = new Point(220 + 100 * x, 50 + 75 * y);
                    }
                    else
                    {
                        b.Location = new Point(175 + 100 * x, 50 + 75 * y);
                    }
                    b.Check = false;
                    b.Font = new Font(b.Font.FontFamily, 30);
                    panelGame.Controls.Add(b);
                    letters[text_box_index] = b;
                    text_box_index++;
                    b.KeyPress += new KeyPressEventHandler(this.b_KeyPress);
                }
            }
            letters[0].Focus();
            newGame = true;
            focus_index = 0;
            x = 0;
        }
        void b_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Sprawdzanie zgodności wpisanego słowa z wylosowanym słowem 
                newGame = false;
                for (int i = 0; i < word.Length; i++)
                {
                    if (word.Substring(i, 1) == letters[i + attempt * word.Length].Text)
                    {
                        letters[i + attempt * word.Length].BackColor = Color.Green;
                        letters[i + attempt * word.Length].ForeColor = Color.Blue;
                        letters[i + attempt * word.Length].Check = true;
                    }
                }
                for (int i = 0; i < word.Length; i++)
                {

                    for (int j = 0; j < word.Length; j++)
                    {
                        if (word.Substring(j, 1) != letters[i + attempt * word.Length].Text && letters[i + attempt * word.Length].Check == false)
                        {
                            letters[i + attempt * word.Length].BackColor = Color.Gray;
                            letters[i + attempt * word.Length].ForeColor = Color.Red;
                        }
                    }

                    for (int j = 0; j < word.Length; j++)
                    {
                        if (word.Substring(j, 1) == letters[i + attempt * word.Length].Text && letters[i + attempt * word.Length].Check == false && letters[j + attempt * word.Length].Check == false)
                        {
                            letters[i + attempt * word.Length].BackColor = Color.Yellow;
                            letters[i + attempt * word.Length].ForeColor = Color.Blue;
                        }
                    }

                }

                progress = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    if (letters[i + attempt * word.Length].Check == true)
                    {
                        progress++;
                    }
                }
                if (progress == word.Length)
                {
                    DialogResult dialogresult = MessageBox.Show("Czy chcesz zagrać ponownie?", "Gratulujemy zgadnięcia słowa B)", MessageBoxButtons.YesNo);
                    if (dialogresult == DialogResult.Yes)
                    {
                        PrepareNewGame(6);
                    }
                    else
                    {
                        Application.Exit();
                    }
                }

                // Przechodzenie do nowej linijki po wciśnięciu "Enter" narazie tylko na ekranowej klawiaturze
                if (!newGame)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        letters[i + attempt * word.Length].Enabled = false;
                    }
                    if (attempt + 1 < 6)
                    {
                        attempt++;
                        x = (attempt * word.Length);
                        letters[x].Enabled = true;
                        letters[x].Focus();
                    }
                    else
                    {
                        DialogResult dialogresult = MessageBox.Show("Niestety Ci się nie udało zgadnąć słowa :(", "Czy chcesz zagrać ponownie?", MessageBoxButtons.YesNo);
                        if (dialogresult == DialogResult.Yes)
                        {
                            PrepareNewGame(6);
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }
                }
            }
            else if (e.KeyChar == (char)8)
            {
                if (x != attempt * word.Length)
                {
                    if (letters[x].Text == "")
                    {
                        letters[x].Enabled = false;
                        letters[x - 1].Enabled = true;
                        letters[x - 1].Focus();
                        letters[x - 1].Text = "";
                        x--;
                    }
                }

            }
            else
            {
                if (x == ((attempt + 1) * word.Length) - 1)
                {

                }
                else
                {
                    letters[x].Enabled = false;
                    x++;
                    letters[x].Enabled = true;
                    letters[x].Focus();
                }

            }
        }
    }
}