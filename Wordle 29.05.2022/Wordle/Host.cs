using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SuperSimpleTcp;

namespace Wordle
{
    public class Host
    {
        SimpleTcpServer server;

        public string messageH, test;
        public string ClientIP;
        public int recived_words = 0;
        public void startServer(string txtIP)
        {
            server = new SimpleTcpServer(txtIP);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
            server.Start();
            // MessageBox.Show("Connected as Host");
        }

        public bool is_listening()
        {
            return server.IsListening;
        }

        public void send(string message)
        {
            server.Send(ClientIP, message);
        }

        public void clear()
        {
            messageH = "";
        }

        public string return_message()
        {
            //string message = messageH;
            //messageH = "";
            return messageH;
        }

        public event Action przyszloH;
        public event Action play_againH;
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            messageH = $"{Encoding.UTF8.GetString(e.Data)}";
            if (Multiplayer.message_type == "player")
            {
                // Multiplayer.message_type = "system";
                Multiplayer.spell_check = true;
                recived_words++;
                //przyszloH?.Invoke();
            }
            //if (Multiplayer.message_type == "system")
            //{
                test = messageH.Substring(0, 3);
                if (test == "yes")
                {
                    Multiplayer.answer = "yes";
                }
                else if (test == "los" || test == "win")
                {
                    play_againH?.Invoke();
                    Multiplayer.e_game_state = test;
                }
                else if (test == "trw")
                {
                    Multiplayer.e_game_state = test;
                }
            // }
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            // MessageBox.Show("Client connected");
            ClientIP = e.IpPort;
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            // MessageBox.Show("Client disconnected");
        }
    }
}
