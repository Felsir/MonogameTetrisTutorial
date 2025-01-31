using Chapter3.Grid;
using Chapter3.Tetrimino;
using Chapter3.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chapter3.Utils.InputManager;

namespace Chapter3.Play
{
    internal class Player
    {
        private Grid.Playfield _playfield;
        private Tetrimino.TetriminoFactory _pieceFactory;

        private InputManager _playerInput;

        private Tetrimino.Tetrimino _currentPiece; // the piece under player control
        private int _x, _y; // position of the current piece
        private int _ghostX, _ghostY; // position of the ghost piece

        public int Level = 1; // To track the current level
        public int Lines = 0; // To track the number of lines this player has completed
        public int Score = 0;

        private double _dropSpeed, _dropTimer;

        private const int SDF = 6; //Soft Drop Factor

        // Event handlers
        public event EventHandler<LevelIncreasedEventArgs> LevelIncreasedEvent;
        public event EventHandler<ScoreAwardedEventArgs> ScoreAwardedEvent;
        public event EventHandler<GameOverEventArgs> GameOverEvent;
        

        private enum PlayerStates
        {
            Playing,                 // the player is in control of the piece.
            WaitingForClearComplete, // the player needs to wait for the playfield to clear the lines.
            GameOver                 // the player has messed up, no new piece can be spawned.
        }
        private PlayerStates _state;

        public Player(Grid.Playfield playfield)
        {
            _playfield = playfield; // assign a playfield to the player.
            _pieceFactory = new Tetrimino.TetriminoFactory(); // a way to generate new pieces.

            _dropSpeed = CalculateDropSpeed(Level); // what is the timing for the current level?
            _dropTimer = _dropSpeed; // to keep track of the timer for the current row. 

            _playerInput = new InputManager();

            GeneratePiece(); // Give the player a piece to start with.

            _state = PlayerStates.Playing; // At the moment the player can immediately play!

            _playfield.LinesClearedCompleteEvent += PlayfieldLinesClearedCompleteEvent; // subscribe to this event!
        }

        private void PlayfieldLinesClearedCompleteEvent(object sender, LinesClearedEventArgs e)
        {
            _state = PlayerStates.Playing; // the player can enjoy playing again!

            // Score is based on what the level _was_ before the lines were added.
            switch (e.NumberOfClearedLines)
            {
                case 1:
                    AwardScore(100 * Level);
                    break;
                case 2:
                    AwardScore(300 * Level);
                    break;
                case 3:
                    AwardScore(500 * Level);
                    break;
                case 4:
                    AwardScore(800 * Level);
                    break;
            }

            // Lines were completed so increase the count and check if the difficulty must be increased:
            Lines += e.NumberOfClearedLines;

            CheckLevel(Lines);
        }

        public void AwardScore(int value)
        {
            if(value==0) 
                return; //prevent unneeded events. (hhey perhaps negative score is possible in your game!)

            Score += value;

            RaiseScoreAwardedEvent(new ScoreAwardedEventArgs() { Score = Score, Increment=value });
        }

        public void Update(GameTime gameTime)
        {
            // update the state of playerinput.
            _playerInput.Update();

            // update the playfield
            _playfield.Update(gameTime);

            switch (_state)
            {
                case PlayerStates.WaitingForClearComplete:
                    {
                        // do nothing... just wait.
                        break;
                    }

                case PlayerStates.GameOver:
                    {
                        // do nothing... just wait.
                        // this should be handled by the Gameplay scene.
                        break;
                    }

                case PlayerStates.Playing:
                    {

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

                            if (_dropTimer < 0)
                            {
                                AwardScore(1);
                            }
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
                        break;
                    }
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
            if (!_playfield.LockInPlace(_currentPiece, _ghostX, _ghostY))
            {
                GameOver();
            }

            //award score:
            AwardScore(2 * (_ghostY - _y));

            if (_playfield.ValidateField() > 0)
            {
                _state = PlayerStates.WaitingForClearComplete;
            }

            // give the player a new piece:
            GeneratePiece();
        }

        private void SoftlockPiece()
        {
            // lock the piece onto the playfield:
            if (!_playfield.LockInPlace(_currentPiece, _x, _y))
            {
                GameOver();
            }

            if (_playfield.ValidateField() > 0)
            {
                _state = PlayerStates.WaitingForClearComplete;
            }

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

        private void CheckLevel(int lines)
        {
            int lvl = (lines / 10) + 1; // there is no level 0, so we +1 for this.

            if (lvl <= Level)
                return; // we're at the same or higher level.

            // Level should increase!
            Level = lvl;

            // Update settings that are tied to the level.
            _dropSpeed = CalculateDropSpeed(Level); // what is the timing for the current level?
            _dropTimer = _dropSpeed; // set the starting value. 

            // notify others something interesting has happened:
            RaiseLevelIncreasedEvent(new LevelIncreasedEventArgs() { Level = lvl });
        }

        protected virtual void RaiseLevelIncreasedEvent(LevelIncreasedEventArgs e)
        {
            EventHandler<LevelIncreasedEventArgs> handler = LevelIncreasedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseScoreAwardedEvent(ScoreAwardedEventArgs e)
        {
            EventHandler<ScoreAwardedEventArgs> handler = ScoreAwardedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaiseGameOverEvent(GameOverEventArgs e)
        {
            EventHandler<GameOverEventArgs> handler = GameOverEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void GeneratePiece()
        {
            _currentPiece = _pieceFactory.GenerateRandom();
            _x = 4;
            _y = -2; //yes, the piece actually starts above the playfield.

            if (!_playfield.DoesShapeFitHere(_currentPiece, _x, _y))
            {
                // an new Tetrimono doesn't fit here! 
                GameOver();
            }
        }

        public void GameOver()
        {
            _state = PlayerStates.GameOver;
            RaiseGameOverEvent(new GameOverEventArgs() { Level = Level, Score = Score, Lines = Lines });
        }

        public void Draw()
        {
            _playfield.Draw();

            if (_state == PlayerStates.Playing)
            {
                _playfield.DrawTetrimino(_currentPiece, _x, _y);
                _playfield.DrawGhostTetrimino(_currentPiece, _ghostX,_ghostY);
            }
        }
    }
}
