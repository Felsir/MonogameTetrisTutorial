using Chapter3.Scenes;
using Chapter3.Play;
using Chapter3.Grid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chapter3.Assets;

namespace Chapter3
{
    internal class MarathonGame : IScene
    {

        private Player _player;
        private Playfield _playfield;

        private int _level, _lines, _score;

        private enum MarathonStates
        {
            Playing,
            GameOver
        }

        private MarathonStates _state;

        public MarathonGame()
        {
            // The playfield's origin is topleft:
            // The playfield is 2 units wide and 4 units high; so -1,2,0 puts the playfield in the center of our view. 
            _playfield = new Playfield(new Vector3(-1f, 2f, 0));

            // Create a player and assing the playfield object:
            _player = new Player(_playfield);

            // Notify me when the level increases:
            _level = _player.Level;
            _lines = _player.Lines;

            // We're interested in these events, when does the level increase? 
            _player.LevelIncreasedEvent += PlayerLevelIncreasedEvent;
            // Does the player score lines?
            _playfield.LinesClearedCompleteEvent += PlayfieldLinesClearedCompleteEvent;
            _player.ScoreAwardedEvent += PlayerScoreAwardedEvent;
            _player.GameOverEvent += PlayerGameOverEvent;

            _state = MarathonStates.Playing;
        }

        private void PlayerGameOverEvent(object sender, GameOverEventArgs e)
        {
            _state = MarathonStates.GameOver;
        }

        private void PlayerScoreAwardedEvent(object sender, ScoreAwardedEventArgs e)
        {
            _score = e.Score;
            //maybe you want to show the increments as a nice effect somewhere? Now is your chance!
        }

        private void PlayfieldLinesClearedCompleteEvent(object sender, LinesClearedEventArgs e)
        {
            _lines += e.NumberOfClearedLines;
        }

        private void PlayerLevelIncreasedEvent(object sender, LevelIncreasedEventArgs e)
        {
            _level=e.Level;
        }

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
            _player.Draw();

            spriteBatch.Begin();
            spriteBatch.DrawString(Art.GameFont,string.Format("Level: {0}", _level.ToString("00")), new Vector2(100,100),Color.White);
            spriteBatch.DrawString(Art.GameFont, string.Format("Lines: {0}", _lines.ToString("000")), new Vector2(100, 135), Color.White);
            spriteBatch.DrawString(Art.GameFont, string.Format("Score: {0}", _score.ToString("000000")), new Vector2(100, 170), Color.White);

            spriteBatch.DrawString(Art.GameFont, string.Format("LDT: {0}", _player._lockDelayTimer.ToString("0.000")), new Vector2(100, 300), Color.White);
            spriteBatch.DrawString(Art.GameFont, string.Format("LDC: {0}", _player._lockDelayMoveCounter.ToString("00")), new Vector2(100, 335), Color.White);
            spriteBatch.DrawString(Art.GameFont, string.Format("LD: {0}", _player._lockDelayMode.ToString()), new Vector2(100, 370), Color.White);



            if (_state == MarathonStates.GameOver)
            {
                spriteBatch.DrawString(Art.GameFont, "- G A M E   O V E R -", new Vector2(100, 220), Color.White);
            }

            spriteBatch.End();
        }
    }
}
