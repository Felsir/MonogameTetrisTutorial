using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chapter4
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private InstancedCubeDrawing _instancedCubeDrawing;
        private List<CubeObject> _CubeObjects;

        private Matrix _view, _projection;
        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _view = Matrix.CreateLookAt(new Vector3(8.75f,7,3.5f), new Vector3(8.75f,0,-10), Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), _graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight, 0.01f, 500);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _instancedCubeDrawing = new InstancedCubeDrawing(GraphicsDevice);
            _instancedCubeDrawing.LoadContent(Content);
            _instancedCubeDrawing.SetEffectParameters(_view, _projection);

            _CubeObjects = new List<CubeObject>();

            //let's create 2500 cubes!
            Random r = new Random();
            for (int x = 0; x < 50; x++)
            {
                for (int z = 0; z < 50; z++)
                {
                    _CubeObjects.Add(new CubeObject(x, z, 2 + (float)r.NextDouble() * 7));
                }
            }
        }



        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var cubeObject in _CubeObjects)
            {
                cubeObject.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            _instancedCubeDrawing.BeginCubeInstance();

            foreach (var cubeObject in _CubeObjects)
            {
                cubeObject.Draw(_instancedCubeDrawing);
            }


            _instancedCubeDrawing.EndCubeInstance();

            base.Draw(gameTime);
        }
    }
}
