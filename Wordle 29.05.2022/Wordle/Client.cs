using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperSimpleTcp;

namespace Wordle
{
    public class Client
    {
        SimpleTcpClient client;
        public string messageC, test;
        public int recived_words = 0;
        public void startClient(string txtIP)
        {
            try
            {
                client = new SimpleTcpClient(txtIP);
                client.Events.Connected += Events_Connected;
                client.Events.DataReceived += Events_DataReceived;
                client.Events.Disconnected += Events_Disconnected;
                client.Connect();
                // MessageBox.Show("Connected as client");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            // MessageBox.Show("Client connected");
        }

        public void clear()
        {
            messageC = "";
        }

        public event Action przyszloC;
        public event Action play_againC;
        public event Action change_game_stateC;

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            messageC = $"{Encoding.UTF8.GetString(e.Data)}";
            if (Multiplayer.message_type == "player")
            {
                Multiplayer.spell_check = true;
                recived_words++;
                //przyszloC?.Invoke();
               // Multiplayer.message_type = "system";
            }
            //if (Multiplayer.message_type == "system")
            // {
                test = messageC.Substring(0, 3);
                if (test == "yes")
                {
                    Multiplayer.answer = "yes";
                }
                else if (test == "los" || test == "win")
                {
                    play_againC?.Invoke();
                    Multiplayer.e_game_state = test;
                }
                else if (test == "trw")
                {
                    Multiplayer.e_game_state = test;
                }
          //  }
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            // MessageBox.Show("Client disconnected");
        }
        public bool is_connected()
        {
            return client.IsConnected;
        }

        public void send(string message)
        {
            client.Send(message);
        }

        public string return_message()
        {
            //string message = messageC;
            //messageC = "";
            return messageC;
        }
    }
}
