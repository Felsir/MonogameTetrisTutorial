using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chapter2.Tetrimino;
using System.Reflection.Metadata;

namespace Chapter2.Grid
{
    internal class Playfield
    {
        private Cell[][] _cells;
        private const int COLUMNS = 10;
        private const int LINES = 20;

        private Vector3 _position;

        private List<int> CompletedLines = new List<int>();

        private double _lineClearTimer; // keep track of the highligh duration
        private const double HIGHLIGHTTIME = 0.5d; // the actual duration of the highlight effect

        // Event handlers
        public event EventHandler<LinesClearedEventArgs> LinesClearedCompleteEvent;

        public Playfield(Vector3 position)
        {
            _position = position;

            _cells = new Cell[LINES][];

            for (int i = 0; i < LINES; i++)
            {
                _cells[i] = new Cell[COLUMNS];

                for (int j = 0; j < COLUMNS; j++)
                    _cells[i][j] = new Cell() { Occupied = false, Color = Color.Black };
                // Color doesn't really matter as we're not going to draw unoccupied spaces.
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_lineClearTimer > 0)
            {
                _lineClearTimer -= gameTime.ElapsedGameTime.TotalSeconds;

                if (_lineClearTimer < 0)
                {
                    // clear the completed lines
                    ClearLines();
                }
            }
        }

        public bool LockInPlace(Tetrimino.Tetrimino shape, int leftcolumn, int topline)
        {
            //if the piece is outside the array of the grid: it cannot be placed!
            if (topline < 0)
                return false;

            for (int y = 0; y < shape.CurrentShape.shapeBit.Length; y++)
            {
                for (int x = 0; x < shape.CurrentShape.shapeBit[y].Length; x++)
                {
                    if (shape.CurrentShape.shapeBit[y][x])
                    {
                        _cells[topline + y][leftcolumn + x].Occupied = true;

                        _cells[topline + y][leftcolumn + x].Color = shape.Color;
                    }
                }
            }
            return true;
        }

        public bool DoesShapeFitHere(Tetrimino.Tetrimino shape, int leftcolumn, int topline)
        {
            //loop over the bits in our shape:
            for (int y = 0; y < shape.CurrentShape.shapeBit.Length; y++)
            {
                for (int x = 0; x < shape.CurrentShape.shapeBit[y].Length; x++)
                {
                    //We only need to check bits that are set to true
                    if (shape.CurrentShape.shapeBit[y][x])
                    {
                        //so this is a filled bit of the shape!

                        //we return false if the shape tries to fit in the border:
                        //check for bottom:
                        if (topline + y >= LINES)
                            return false;

                        //check for left wall:
                        if (leftcolumn + x < 0)
                            return false;

                        //check for right wall:
                        if (leftcolumn + x >= COLUMNS)
                            return false;

                        //We're not checking the top- a piece spawns above the playfield!
                        //bonus, we prevent the array out of bounds
                        if (topline + y < 0)
                            continue;

                        //now for the grid:
                        //if both the bit in the shape is true
                        //and the cell is occupied, 
                        //the shape can not fit!
                        if (_cells[topline + y][leftcolumn + x].Occupied)
                            return false;
                    }
                }
            }

            //all checks came out clear, so it fits!
            return true;
        }
        

        public int ValidateField()
        {
            CompletedLines.Clear();

            for (int y = 0; y < LINES; y++)
            {
                bool lineclear = true;
                for (int x = 0; x < COLUMNS; x++)
                {
                    if (!_cells[y][x].Occupied)
                    {
                        //unoccupied space in this line, no need to check further.
                        lineclear = false;
                        break;
                    }
                }

                if (lineclear)
                {
                    CompletedLines.Add(y);
                }
            }

            if (CompletedLines.Count > 0)
            {
                _lineClearTimer = HIGHLIGHTTIME;
            }

            return CompletedLines.Count;
        }

        public void ClearLines()
        {
            foreach (int line in CompletedLines)
                ClearLine(line);

            // tell the interested objects that we've completed the line clearing sequence!
            RaiseClearedLinesCompleteEvent(new LinesClearedEventArgs() { NumberOfClearedLines = CompletedLines.Count });
        }

        private void ClearLine(int y)
        {
            for (int line = y; line > 0; line--)
            {
                _cells[line] = CopyLine(line - 1);
            }

            for (int column = 0; column < COLUMNS; column++)
            {
                Cell c = new Cell() { Occupied = false, Color = Color.Transparent };
                _cells[0][column] = c;
            }
        }

        private Cell[] CopyLine(int line)
        {
            Cell[] cells = new Cell[COLUMNS];
            for (int column = 0; column < COLUMNS; column++)
            {
                Cell c = new Cell() { Occupied = _cells[line][column].Occupied, Color = _cells[line][column].Color };
                cells[column] = c;
            }

            return cells;
        }

        public void Draw()
        {
            for (int y = 0; y < LINES; y++)
            {
                //left border:
                Assets.Models.DrawCube(
                    Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(-1 * 0.2f, -y * 0.2f, 0),
                    Color.Gray);

                //right border:
                Assets.Models.DrawCube(
                    Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(COLUMNS * 0.2f, -y * 0.2f, 0),
                    Color.Gray);

                //cells in this line:
                for (int x = 0; x < COLUMNS; x++)
                {
                    //if the cell is empty, skip to the next one.
                    if (!_cells[y][x].Occupied)
                        continue;

                    if (_lineClearTimer > 0)
                    {
                        if (CompletedLines.Contains(y))
                        {
                            Assets.Models.DrawCube(
                                Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(x * 0.2f, -y * 0.2f, 0),
                                Color.White * (float)(_lineClearTimer/HIGHLIGHTTIME));

                            // this cube is hilighted and we can carry onto the next cube.
                            continue;
                        }
                    }

                    Assets.Models.DrawCube(
                        Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(x * 0.2f, -y * 0.2f, 0),
                        _cells[y][x].Color);
                }
            }

            //Draw bottom of the grid
            for (int x = -1; x < COLUMNS + 1; x++)
            {
                Assets.Models.DrawCube(Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(x * 0.2f, -20 * 0.2f, 0),
                Color.Gray);
            }
        }

        public void DrawTetrimino(Tetrimino.Tetrimino t, int column, int row)
        {
            Matrix world = Matrix.CreateTranslation(_position) *
                        Matrix.CreateTranslation(column * 0.2f, -row * 0.2f, 0);

            t.Draw(world);
        }

        public void DrawGhostTetrimino(Tetrimino.Tetrimino t, int column, int row)
        {
            Matrix world = Matrix.CreateTranslation(_position) *
                        Matrix.CreateTranslation(column * 0.2f, -row * 0.2f, 0);

            t.Draw(world,0.4f);
        }


        protected virtual void RaiseClearedLinesCompleteEvent(LinesClearedEventArgs e)
        {
            EventHandler<LinesClearedEventArgs> handler = LinesClearedCompleteEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
