using Chapter3.Assets;
using Chapter3.Scenes;
using Chapter3.Tetrimino;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chapter3.Enums;

namespace Chapter3
{
    internal class TitleScreen : IScene
    {
        // Let's rotate the cube for a nice visual effect.
        private float _angle;
        private Matrix _world;

        private Tetrimino.Tetrimino _tetrimino;

        public TitleScreen() 
        { 
            TetriminoFactory factory = new TetriminoFactory();
            _tetrimino = factory.Generate(Tetriminoes.J);
        }

        public void Update(GameTime gameTime)
        {
            // Increase the angle framerate independant:
            // 0.75 radians per second, also keep the rotation within the 2 PI bound.
            _angle += 0.75f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _angle %= MathHelper.TwoPi;

            _world= Matrix.CreateScale(5) * Matrix.CreateRotationY(_angle) * Matrix.CreateTranslation(0, 0, -3f);
        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _tetrimino.Draw(_world);
        }
    }
}
