using Chapter2.Assets;
using Chapter2.Scenes;
using Chapter2.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chapter2
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SceneManager SceneManager;
        
        public static Camera Camera;
        public static BasicEffect BasicEffect;

        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        protected override void Initialize()
        {

#if DEBUG
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            IsMouseVisible = true;
#else
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            IsMouseVisible = false;
#endif
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Models.Initialize(Content);
            Art.Initialize(Content);

            SceneManager = new SceneManager();

            Camera = new Camera(new Vector3(0,0,5),new Vector3(0,0,-5),_graphics.PreferredBackBufferWidth,_graphics.PreferredBackBufferHeight, MathHelper.ToRadians(60));

            // Use the Monogame Basic Effect:
            BasicEffect = new BasicEffect(GraphicsDevice);

            // The Camera is static, so we only need to set it once.
            BasicEffect.View = Camera.View;
            BasicEffect.Projection = Camera.Projection;

            // Setup some shader parameters to create a nice effect.
            BasicEffect.EnableDefaultLighting();
            BasicEffect.PreferPerPixelLighting = true;
            BasicEffect.SpecularPower = 16f;

            //TitleScreen titleScreen = new TitleScreen();
            //SceneManager.PushScene(titleScreen);

            //TestScene testScene = new TestScene();
            //SceneManager.PushScene(testScene);

            MarathonGame gameScene = new MarathonGame();
            SceneManager.PushScene(gameScene);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SceneManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            SceneManager.Draw(_spriteBatch, gameTime);

            base.Draw(gameTime);
        }
    }
}
