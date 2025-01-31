using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chapter3.Enums;

namespace Chapter3.Tetrimino
{
    internal class TetriminoFactory
    {
        private Random _random;
        private List<Tetriminoes> _SevenBag = new List<Tetriminoes>();

        public TetriminoFactory(int seed = -1)
        {
            if (seed < 0)
            {
                _random = new Random();
            }
            else
            {
                // This way we can have two factories generate identical sequences!
                // which is useful for versus mode- or a mode to try to get the best score with the same sequence.
                _random = new Random(seed);
            }
        }

        public Tetrimino GenerateRandom()
        {
            if (_SevenBag.Count == 0) // if the bag is empty...
            {
                // Create a new bag of pieces:
                _SevenBag.Add((Tetriminoes)0);
                _SevenBag.Add((Tetriminoes)1);
                _SevenBag.Add((Tetriminoes)2);
                _SevenBag.Add((Tetriminoes)3);
                _SevenBag.Add((Tetriminoes)4);
                _SevenBag.Add((Tetriminoes)5);
                _SevenBag.Add((Tetriminoes)6);
            }

            // the piece that is picked is taken at random from the list.
            int i = _random.Next(_SevenBag.Count);
            Tetrimino t = Generate(_SevenBag[i]);

            // remove it from the source list.
            _SevenBag.RemoveAt(i);

            return t;
        }


        public Tetrimino Generate(Tetriminoes shapetype)
        {
            Shape[] shapes = new Shape[4];
            Color color = Color.White;

            switch (shapetype)
            {
                // I - the long one
                case Tetriminoes.I:
                    {
                        color = Color.Cyan;
                        shapes[0] = new Shape(new string[]
                        {
                            "0000",
                            "1111",
                            "0000",
                            "0000"
                        });
                        shapes[1] = new Shape(new string[]
                        {
                            "0010",
                            "0010",
                            "0010",
                            "0010"
                        });
                        shapes[2] = new Shape(new string[]
                        {
                            "0000",
                            "0000",
                            "1111",
                            "0000"
                        });
                        shapes[3] = new Shape(new string[]
                        {
                            "0100",
                            "0100",
                            "0100",
                            "0100"
                        });

                        break;
                    }

                case Tetriminoes.O:
                    {
                        color = Color.Yellow;
                        shapes = new Shape[1];
                        shapes[0] = new Shape(new string[]
                        {
                            "11",
                            "11",
                        });
                        break;
                    }

                case Tetriminoes.T:
                    {
                        color = Color.DeepPink;
                        shapes[0] = new Shape(new string[]
                        {
                            "010",
                            "111",
                            "000"
                        });
                        shapes[1] = new Shape(new string[]
                        {
                            "010",
                            "011",
                            "010"
                        });
                        shapes[2] = new Shape(new string[]
                        {
                            "000",
                            "111",
                            "010"
                        });
                        shapes[3] = new Shape(new string[]
                        {
                            "010",
                            "110",
                            "010"
                        });
                        break;
                    }

                case Tetriminoes.J:
                    {
                        color = Color.Blue;
                        shapes[0] = new Shape(new string[]
                        {
                            "100",
                            "111",
                            "000"
                        });
                        shapes[1] = new Shape(new string[]
                        {
                            "011",
                            "010",
                            "010"
                        });
                        shapes[2] = new Shape(new string[]
                        {
                            "000",
                            "111",
                            "001"
                        });
                        shapes[3] = new Shape(new string[]
                        {
                            "010",
                            "010",
                            "110"
                        });
                        break;
                    }
                case Tetriminoes.L:
                    {
                        color = Color.Orange;
                        shapes[0] = new Shape(new string[]
                        {
                            "001",
                            "111",
                            "000"
                        });
                        shapes[1] = new Shape(new string[]
                        {
                            "010",
                            "010",
                            "011"
                        });
                        shapes[2] = new Shape(new string[]
                        {
                            "000",
                            "111",
                            "100"
                        });
                        shapes[3] = new Shape(new string[]
                        {
                            "110",
                            "010",
                            "010"
                        });

                        break;
                    }
                case Tetriminoes.S:
                    {
                        color = Color.Green;
                        shapes[0] = new Shape(new string[]
                        {
                            "011",
                            "110",
                            "000"
                        });
                        shapes[1] = new Shape(new string[]
                        {
                            "010",
                            "011",
                            "001"
                        });
                        shapes[2] = new Shape(new string[]
                        {
                            "000",
                            "011",
                            "110"
                        });
                        shapes[3] = new Shape(new string[]
                        {
                            "100",
                            "110",
                            "010"
                        });

                        break;
                    }
                case Tetriminoes.Z:
                    {
                        color = Color.Red;
                        shapes[0] = new Shape(new string[]
                        {
                            "110",
                            "011",
                            "000"
                        });
                        shapes[1] = new Shape(new string[]
                        {
                            "001",
                            "011",
                            "010"
                        });
                        shapes[2] = new Shape(new string[]
                        {
                            "000",
                            "110",
                            "011"
                        });
                        shapes[3] = new Shape(new string[]
                        {
                            "010",
                            "110",
                            "100"
                        });

                        break;
                    }


            }

            Tetrimino t = new Tetrimino();
            t.SetShape(shapes, shapetype, color);

            return t;
        }
    }
}
