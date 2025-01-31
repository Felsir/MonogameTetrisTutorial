using Chapter3.Scenes;
using Chapter3.Tetrimino;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3
{
    internal class TestScene : IScene
    {

        private Grid.Playfield _playfield;

        public TestScene()
        {
            // The playfield's origin is topleft:
            // The playfield is 2 units wide and 4 units high; so -1,2,0 puts the playfield in the center of our view. 
            _playfield = new Grid.Playfield(new Vector3(-1f,2f,0));

            //let's generate a tetrimino:
            TetriminoFactory factory = new TetriminoFactory();

            Tetrimino.Tetrimino t1 = factory.Generate(Enums.Tetriminoes.O);
            Tetrimino.Tetrimino t2 = factory.Generate(Enums.Tetriminoes.L);
            Tetrimino.Tetrimino t3 = factory.Generate(Enums.Tetriminoes.J);
            Tetrimino.Tetrimino t4 = factory.Generate(Enums.Tetriminoes.O);

            _playfield.LockInPlace(t1, 0, 18);
            _playfield.LockInPlace(t2, 2, 18);
            _playfield.LockInPlace(t3, 5, 18);
            _playfield.LockInPlace(t4, 8, 18);

            int lines = _playfield.ValidateField();
            _playfield.ClearLines();

        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _playfield.Draw();
        }

    }
}
