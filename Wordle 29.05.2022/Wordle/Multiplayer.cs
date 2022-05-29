using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wordle
{
    public partial class Multiplayer : Form
    {
        private SqlConnection con = new SqlConnection("Data Source =bmw-io.database.windows.net; Initial Catalog=SlowkaIO; User ID=bmw; Password=Azure321?");
        private SqlCommand cmd = new SqlCommand();
        private Random gen = new Random();
        private const int fieldSize = 20;
        private GameTextBox[] letters = new GameTextBox[42]; // tablica textboxów
        private GameTextBox[] enemyletters = new GameTextBox[42]; // tablica textboxów
        private int text_box_index, attempt = 0, x = 0, focus_index = 1, enemy_attempt = 0;
        private string word, player_message, game_state;
        private string[] enemy_words = new string[6];
        private int word_id, progress, enemy_words_counter = 0;
        private bool newGame;
        [System.ComponentModel.Browsable(false)]
        public event System.Windows.Forms.KeyPressEventHandler KeyPress;
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        Host H = new Host();
        Client C = new Client();
        public static string message_type, e_game_state, answer;
        public static bool spell_check, wait = true;
        public Multiplayer()
        {
            KeyPreview = true;
            InitializeComponent();
            con.Open();
            H.przyszloH += przyszlo_H;
            C.przyszloC += przyszlo_C;
            C.play_againC += play_again;
            H.play_againH += play_again;
        }

        //public delegate void myDelegat();
        //public event myDelegat NewGame;

        private void play_again()
        {
            DialogResult dialogresult = MessageBox.Show("Czy chcesz zagrać ponownie?", "Koniec gry!)", MessageBoxButtons.YesNo);
            if (dialogresult == DialogResult.Yes)
            {
                message_type = "system";
                if (ClientType.client_type == "host")
                {
                    H.send("yes");
                }
                else if (ClientType.client_type == "client")
                {
                    C.send("yes");
                }
                if (ClientType.client_type == "host")
                {
                    Thread.Sleep(10000);
                    PrepareNewGameHost(6);
                }
                else if (ClientType.client_type == "client")
                {
                    while (wait)
                    {
                        if (e_game_state != "win" && e_game_state != "los")
                        {              
                            break;
                        }
                    }
                    word = C.messageC;
                    MessageBox.Show(word);
                    C.clear();
                    PrepareNewGameClient(6);
                }
            }
            else
            {
                Application.Exit();
            }
        }
        private void przyszlo_H()
        {
            //MessageBox.Show("ASD");
            if (spell_check == true)
            {
                if (ClientType.client_type == "host")
                {
                    for (int i = enemy_attempt; i < H.recived_words; i++)
                    {
                        string enemy_word = H.return_message();
                        enemy_words[i] = enemy_word.Substring(word.Length * i, word.Length);
                    }
                    for (int i = enemy_attempt; i < H.recived_words; i++)
                    {
                        enemy_spell_check(enemy_words[enemy_attempt]);
                    }
                }
                else if (ClientType.client_type == "client")
                {
                    for (int i = enemy_attempt; i < C.recived_words; i++)
                    {
                        enemy_words[i] = C.return_message().Substring(word.Length * i, word.Length);
                    }
                    for (int i = enemy_attempt; i < C.recived_words; i++)
                    {
                        enemy_spell_check(enemy_words[enemy_attempt]);
                    }
                }
                spell_check = false;
            }
        }
        private void przyszlo_C()
        {
            //MessageBox.Show("ASD");
            if (spell_check == true)
            {
                if (ClientType.client_type == "host")
                {
                    for (int i = enemy_attempt; i < H.recived_words; i++)
                    {
                        string enemy_word = H.return_message();
                        enemy_words[i] = enemy_word.Substring(word.Length * i, word.Length);
                    }
                    for (int i = enemy_attempt; i < H.recived_words; i++)
                    {
                        enemy_spell_check(enemy_words[enemy_attempt]);
                    }
                }
                else if (ClientType.client_type == "client")
                {
                    for (int i = enemy_attempt; i < C.recived_words; i++)
                    {
                        enemy_words[i] = C.return_message().Substring(word.Length * i, word.Length);
                    }
                    for (int i = enemy_attempt; i < C.recived_words; i++)
                    {
                        enemy_spell_check(enemy_words[enemy_attempt]);
                    }
                }
                spell_check = false;
            }
        }

        public string get_message_type()
        {
            return message_type;
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {
   
        }

        private void button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if ( btn.Text == "ENTER" )
            {
                SendKeys.Send("{ENTER}");
            }
            else if ( btn.Text == "BACKSPACE" )
            {
                SendKeys.Send("{BACKSPACE}");
            }
            else
            {
                SendKeys.Send(btn.Text);
            }
            
            letters[x].Focus();
        }

        private void Multiplayer_Load(object sender, EventArgs e)
        {
            //127.0.0.1
            ///var H = new Host();
            if (ClientType.client_type == "host")
            {
                H.startServer(ClientType.ip);
            }
            else if (ClientType.client_type == "client")
            {
                C.startClient(ClientType.ip);
            }
            else
            {
                MessageBox.Show("Niedziała serwer/klient");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (ClientType.client_type == "host")
            {
                PrepareNewGameHost(6);
                letters[0].Focus();
            }
            else if (ClientType.client_type == "client")
            {
                PrepareNewGameClient(6);
                letters[0].Focus();
                message_type = "player";
            }
            else
            {
                MessageBox.Show("Niedziała if");
            }

        }

        private void newWord()
        {
            word_id = gen.Next(8214);
            SqlCommand cmd = new SqlCommand("select UPPER(word) from words where id_word =" + word_id, con);
            cmd.CommandType = CommandType.Text;
            word = (string)cmd.ExecuteScalar();
            text_box_index = 0;
            //var H = new Host();
            if (H.is_listening())
            {
                message_type = "word";
                H.send(word);
                message_type = "player";
            }
            MessageBox.Show(word);
        }

        private void panelGame_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        { }

        private void PrepareNewGameHost(int gameHeight)
        {
           // H.send("trwa");
           // Thread.Sleep(1000);
            text_box_index = 0; enemy_attempt = 0; attempt = 0; player_message = ""; game_state = "trwa"; e_game_state = ""; answer = "";
            spell_check = false;
            panelMY.Controls.Clear();
            newWord();
            for (int i = 0; i < 6; i++)
            {
                enemy_words[i] = "1";
            }
            int gameWidth = word.Length;

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
                    panelMY.Controls.Add(b);
                    letters[text_box_index] = b;
                    text_box_index++;
                    b.KeyPress += new KeyPressEventHandler(this.b_KeyPress);
                }
            }
            panelENEMY.Controls.Clear();
            text_box_index = 0;
            for (int y = 0; y < gameHeight; y++)
            {
                for (int x = 0; x < gameWidth; x++)
                {
                    GameTextBox c = new GameTextBox();
                    c.Enabled = false;
                    c.MaxLength = 1;
                    c.CharacterCasing = CharacterCasing.Upper;
                    c.TextAlign = HorizontalAlignment.Center;
                    c.BackColor = Color.Black;
                    c.ForeColor = Color.White;
                    c.AutoSize = false;
                    c.Size = new Size(50, 50);
                    if (word.Length == 4)
                    {
                        c.Location = new Point(320 + 100 * x, 50 + 75 * y);
                    }
                    else if (word.Length == 5)
                    {
                        c.Location = new Point(270 + 100 * x, 50 + 75 * y);
                    }
                    else if (word.Length == 6)
                    {
                        c.Location = new Point(220 + 100 * x, 50 + 75 * y);
                    }
                    else
                    {
                        c.Location = new Point(175 + 100 * x, 50 + 75 * y);
                    }
                    c.Check = false;
                    c.Font = new Font(c.Font.FontFamily, 30);
                    panelENEMY.Controls.Add(c);
                    enemyletters[text_box_index] = c;
                    text_box_index++;
                }
            }
            newGame = true;
            focus_index = 0;
            x = 0;
            e_game_state = "trwa";
        }

        private void PrepareNewGameClient(int gameHeight)
        {
            text_box_index = 0; enemy_attempt = 0; attempt = 0; player_message = ""; game_state = "trwa"; e_game_state = ""; answer = "";
            e_game_state = "trwa";
            spell_check = false;
            panelMY.Controls.Clear();
            for (int i = 0; i < 6; i++)
            {
                enemy_words[i] = "1";
            }
            word = C.messageC;
            int gameWidth = word.Length;

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
                    panelMY.Controls.Add(b);
                    letters[text_box_index] = b;
                    text_box_index++;
                    b.KeyPress += new KeyPressEventHandler(this.b_KeyPress);
                }
            }
            panelENEMY.Controls.Clear();
            text_box_index = 0;
            for (int y = 0; y < gameHeight; y++)
            {
                for (int x = 0; x < gameWidth; x++)
                {
                    GameTextBox c = new GameTextBox();
                    c.ForeColor = c.BackColor;
                    c.Enabled = false;
                    c.MaxLength = 1;
                    c.CharacterCasing = CharacterCasing.Upper;
                    c.TextAlign = HorizontalAlignment.Center;
                    c.BackColor = Color.Black;
                    c.ForeColor = Color.White;
                    c.AutoSize = false;
                    c.Size = new Size(50, 50);
                    if (word.Length == 4)
                    {
                        c.Location = new Point(320 + 100 * x, 50 + 75 * y);
                    }
                    else if (word.Length == 5)
                    {
                        c.Location = new Point(270 + 100 * x, 50 + 75 * y);
                    }
                    else if (word.Length == 6)
                    {
                        c.Location = new Point(220 + 100 * x, 50 + 75 * y);
                    }
                    else
                    {
                        c.Location = new Point(175 + 100 * x, 50 + 75 * y);
                    }
                    c.Check = false;
                    c.Font = new Font(c.Font.FontFamily, 30);
                    c.ForeColor = Color.Black;
                    c.BackColor = Color.Black;
                    panelENEMY.Controls.Add(c);
                    enemyletters[text_box_index] = c;
                    text_box_index++;
                }
            }
            newGame = true;
            focus_index = 0;
            x = 0;
        }

        void b_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (spell_check == true)
            {
                if (ClientType.client_type == "host")
                {
                    for (int i = enemy_attempt; i < H.recived_words; i++)
                    {
                        string enemy_word = H.return_message();
                        enemy_words[i] = enemy_word.Substring(word.Length * i, word.Length);
                    }
                    for (int i = enemy_attempt; i < H.recived_words; i++)
                    {
                        enemy_spell_check(enemy_words[enemy_attempt]);
                    }
                }
                else if (ClientType.client_type == "client")
                {
                    for (int i = enemy_attempt; i < C.recived_words; i++)
                    {
                        enemy_words[i] = C.return_message().Substring(word.Length * i, word.Length);
                    }
                    for (int i = enemy_attempt; i < C.recived_words; i++)
                    {
                        enemy_spell_check(enemy_words[enemy_attempt]);
                    }
                }
                spell_check = false;
            }
            if (e.KeyChar == (char)13)
            {
                if (ClientType.client_type == "host")
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        player_message = player_message + letters[i + attempt * word.Length].Text;
                    }
                    message_type = "player";
                    H.send(player_message);                    
                }
                else if (ClientType.client_type == "client")
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        player_message = player_message + letters[i + attempt * word.Length].Text;
                    }
                    message_type = "player";
                    C.send(player_message);
                }
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
                        message_type = "system";
                        if (ClientType.client_type == "host")
                        {
                            H.send("win");
                        }
                        else if (ClientType.client_type == "client")
                        {
                            C.send("win");
                        }
                        while (wait)
                        {
                            // czekam na odpowiedź
                            if (answer == "yes")
                            {
                                break;
                            }
                            else if (answer == "no")
                            {
                                Application.Exit();
                            }
                        }
                        if (answer == "yes")
                        {
                            if (ClientType.client_type == "host")
                            {
                                Thread.Sleep(1000);
                                PrepareNewGameHost(6);
                            }
                            else if (ClientType.client_type == "client")
                            {

                                Thread.Sleep(2000);
                                PrepareNewGameClient(6);
                            }
                        }
                    }
                    else
                    {
                        Application.Exit();
                    }
                }

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
                            if (ClientType.client_type == "host")
                            {
                                H.send("lose");
                            }
                            else if (ClientType.client_type == "client")
                            {
                                C.send("lose");
                            }
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

        public void enemy_spell_check(string enemy_word)
        {
            for (int i = 0; i < enemy_word.Length; i++)
            {
                enemyletters[i + enemy_attempt * enemy_word.Length].Text = enemy_word.Substring(i, 1);
            }
            for (int i = 0; i < enemy_word.Length; i++)
            {
                if (word.Substring(i, 1) == enemyletters[i + enemy_attempt * enemy_word.Length].Text)
                {
                    enemyletters[i + enemy_attempt * enemy_word.Length].Enabled = true;
                    enemyletters[i + enemy_attempt * enemy_word.Length].BackColor = Color.Green;
                    enemyletters[i + enemy_attempt * enemy_word.Length].Check = true;
                    enemyletters[i + enemy_attempt * enemy_word.Length].Enabled = false;
                }
            }
            for (int i = 0; i < enemy_word.Length; i++)
            {

                for (int j = 0; j < enemy_word.Length; j++)
                {
                    if (word.Substring(j, 1) != enemyletters[i + enemy_attempt * enemy_word.Length].Text && enemyletters[i + enemy_attempt * enemy_word.Length].Check == false)
                    {
                        enemyletters[i + enemy_attempt * enemy_word.Length].Enabled = true;
                        enemyletters[i + enemy_attempt * enemy_word.Length].BackColor = Color.Gray;
                        enemyletters[i + enemy_attempt * enemy_word.Length].Enabled = false;
                    }
                }

                for (int j = 0; j < word.Length; j++)
                {
                    if (word.Substring(j, 1) == enemyletters[i + enemy_attempt * enemy_word.Length].Text && enemyletters[i + enemy_attempt * enemy_word.Length].Check == false && enemyletters[j + enemy_attempt * enemy_word.Length].Check == false)
                    {
                        enemyletters[i + enemy_attempt * enemy_word.Length].Enabled = true;
                        enemyletters[i + enemy_attempt * enemy_word.Length].BackColor = Color.Yellow;
                        enemyletters[i + enemy_attempt * enemy_word.Length].Enabled = false;
                    }
                }
            }

            //--------------------

            for (int i = 0; i < enemy_word.Length; i++)
            {
                enemyletters[i + enemy_attempt * enemy_word.Length].Text = "";
            }
            enemy_attempt++;
        }

    }
    

}
