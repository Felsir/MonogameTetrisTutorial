using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3.Scenes
{
    public class SceneManager
    {
        private Stack<IScene> _scenes;

        /// <summary>
        /// The SceneManager controls what scene is displayed.
        /// </summary>
        public SceneManager()
        {
            _scenes = new Stack<IScene>();
        }

        /// <summary>
        /// Let a new scene play
        /// </summary>
        /// <param name="scene">The Scene that will be on top of the stack.</param>
        public void PushScene(IScene scene)
        {
            _scenes.Push(scene);
        }

        /// <summary>
        /// Remove the current scene.
        /// </summary>
        public void PopScene()
        {
            if (_scenes.Count > 0)
            {
                _scenes.Pop();
            }
        }

        /// <summary>
        /// Update the current scene.
        /// </summary>
        /// <param name="gameTime">Monogame frametime</param>
        public void Update(GameTime gameTime)
        {
            if (_scenes.Count > 0)
            {
                _scenes.Peek().Update(gameTime);
            }
        }

        /// <summary>
        /// Draw the current scene.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch object to use</param>
        /// <param name="gameTime">Monogame frametime</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_scenes.Count > 0)
            {
                _scenes.Peek().Draw(spriteBatch, gameTime);
            }
        }
    }
}
