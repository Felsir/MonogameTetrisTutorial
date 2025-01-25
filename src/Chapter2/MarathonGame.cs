using Chapter2.Scenes;
using Chapter2.Play;
using Chapter2.Grid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter2
{
    internal class MarathonGame : IScene
    {

        private Player _player;
        private Playfield _playfield;

        public MarathonGame()
        {
            // The playfield's origin is topleft:
            // The playfield is 2 units wide and 4 units high; so -1,2,0 puts the playfield in the center of our view. 
            _playfield = new Playfield(new Vector3(-1f, 2f, 0));

            // Create a player and assing the playfield object:
            _player = new Player(_playfield);
        }

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _player.Draw();
        }
    }
}
