using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter2.Play
{
    internal class GameOverEventArgs:EventArgs
    {
        public int Level;
        public int Score;
        public int Lines;
    }
}
