using Chapter2.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chapter2.Utils.InputManager;

namespace Chapter2.Play
{
    internal class Player
    {
        private Grid.Playfield _playfield;
        private Tetrimino.TetriminoFactory _pieceFactory;

        private InputManager _playerInput;

        private Tetrimino.Tetrimino _currentPiece; // the piece under player control
        private int _x, _y; // position of the current piece
        private int _ghostX, _ghostY; // position of the ghost piece

        public int Level = 1;
        private double _dropSpeed, _dropTimer;

        private const int SDF = 6; //Soft Drop Factor

        public Player(Grid.Playfield playfield)
        {
            _playfield = playfield; // assign a playfield to the player.
            _pieceFactory = new Tetrimino.TetriminoFactory(); // a way to generate new pieces.

            _dropSpeed = CalculateDropSpeed(Level); // what is the timing for the current level?
            _dropTimer = _dropSpeed; // to keep track of the timer for the current row. 

            _playerInput = new InputManager();

            GeneratePiece(); // Give the player a piece to start with.
        }

        public void Update(GameTime gameTime)
        {
            // update the state of playerinput.
            _playerInput.Update();
            _dropTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            CalculateGhostPiece();

            if (_playerInput.IsPressed(Controls.Left))
            {
                if (_playfield.DoesShapeFitHere(_currentPiece, _x - 1, _y))
                {
                    _x -= 1;
                }
            }

            if (_playerInput.IsPressed(Controls.Right))
            {
                if (_playfield.DoesShapeFitHere(_currentPiece, _x + 1, _y))
                {
                    _x += 1;
                }
            }

            if (_playerInput.IsPressed(Controls.RotateCW))
            {
                _currentPiece.RotateLeft();
                if (!_playfield.DoesShapeFitHere(_currentPiece, _x, _y))
                {
                    // it does not fit! Rotate it back:
                    _currentPiece.RotateRight();
                }
            }

            if (_playerInput.IsPressed(Controls.RotateCCW))
            {
                _currentPiece.RotateRight();
                if (!_playfield.DoesShapeFitHere(_currentPiece, _x, _y))
                {
                    // it does not fit! Rotate it back:
                    _currentPiece.RotateLeft();
                }
            }

            if (_playerInput.IsDown(Controls.SoftDrop))
            {
                _dropTimer -= (SDF * _dropSpeed) * gameTime.ElapsedGameTime.TotalSeconds;
            }


            if (_playerInput.IsPressed(Controls.HardDrop))
            {
                HardDrop();
            }

            if (_dropTimer < 0)
            {
                // time for this row is over, can we drop the piece?
                if (_playfield.DoesShapeFitHere(_currentPiece, _x, _y + 1))
                {
                    // yes
                    _y++;
                }
                else
                {
                    // no- lock the piece:
                    SoftlockPiece();
                }

                //reset the timer:
                _dropTimer += _dropSpeed;
            }
        }


        private void CalculateGhostPiece()
        {
            // to see where the piece lands, just keep moving down until we no longer can:
            _ghostX = _x;
            _ghostY = _y;

            while (_playfield.DoesShapeFitHere(_currentPiece, _ghostX, _ghostY + 1))
            {
                _ghostY++;
            }
        }

        private void HardDrop()
        {
            // lock the piece onto the playfield:
            _playfield.LockInPlace(_currentPiece, _ghostX, _ghostY);

            // line checking will be done later!

            // give the player a new piece:
            GeneratePiece();
        }

        private void SoftlockPiece()
        {
            // lock the piece onto the playfield:
            _playfield.LockInPlace(_currentPiece, _x, _y);

            // line checking will be done later!

            // give the player a new piece:
            GeneratePiece();
        }

        private double CalculateDropSpeed(int level)
        {
            // formula as found on The Tetris Guidelines.
            // 1 is the first (lowest) level,
            // The dropspeed does not increase above 20.
            level = MathHelper.Clamp(level, 1, 20);

            // (0.8-((Level-1)*0.007))^(Level-1)
            return Math.Pow((0.8d - (level - 1) * 0.007d), (level - 1));
        }


        private void GeneratePiece()
        {
            _currentPiece = _pieceFactory.GenerateRandom();
            _x = 5;
            _y = -2; //yes, the piece actually starts above the playfield.
        }

        public void Draw()
        {
            _playfield.Draw();
            _playfield.DrawTetrimino(_currentPiece, _x, _y);
        }
    }
}
