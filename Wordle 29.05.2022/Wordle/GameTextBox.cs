using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wordle
{
    class GameTextBox : TextBox
    {
        private Point position;
        private bool check;

        public Point Position
        {
            get => position;
            set => position = value;
        }

        public bool Check
        {
            get => check;
            set
            {
                check = value;
            }
        }
        public GameTextBox()
        {
        }
    }
}