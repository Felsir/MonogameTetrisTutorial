using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter2.Tetrimino
{
    internal class Shape
    {
        public bool[][] shapeBit;

        public Shape(bool[][] shapebits)
        {
            shapeBit = shapebits;
        }

        public Shape(string[] shapedefinition)
        {
            shapeBit = new bool[shapedefinition.Length][];
            int j = 0;
            foreach (string s in shapedefinition)
            {
                shapeBit[j] = new bool[shapedefinition.Length];
                int i = 0;
                foreach (char c in s)
                {
                    shapeBit[j][i] = (c == '1');
                    i++;
                }
                j++;
            }
        }
    }
}
