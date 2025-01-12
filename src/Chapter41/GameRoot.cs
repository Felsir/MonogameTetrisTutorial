using Chapter1.Assets;
using Chapter1.Scenes;
using Chapter1.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chapter1
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static SceneManager SceneManager;
        
        public static Camera Camera;
        public static BasicEffect BasicEffect;
        public static Effect MyEffect;

        public static Texture2D TestTexture;

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

            TestTexture = Content.Load<Texture2D>("testtexture");

            Models.Initialize(Content);

            SceneManager = new SceneManager();

            Camera = new Camera(new Vector3(0,0,5),new Vector3(0,0,-5),_graphics.PreferredBackBufferWidth,_graphics.PreferredBackBufferHeight, MathHelper.ToRadians(60));

            // Use the Monogame Basic Effect:
            BasicEffect = new BasicEffect(GraphicsDevice);

            // Load myEffect
            MyEffect = Content.Load<Effect>("MyEffect");

            // The Camera is static, so we only need to set it once.
            BasicEffect.View = Camera.View;
            BasicEffect.Projection = Camera.Projection;

            MyEffect.Parameters["View"].SetValue(Camera.View);
            MyEffect.Parameters["Projection"].SetValue(Camera.Projection);


            // Setup some shader parameters to create a nice effect.
            BasicEffect.EnableDefaultLighting();
            BasicEffect.PreferPerPixelLighting = true;
            BasicEffect.SpecularPower = 16f;

            TitleScreen titleScreen = new TitleScreen();
            SceneManager.PushScene(titleScreen);
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

            SceneManager.Draw(_spriteBatch, gameTime);

            base.Draw(gameTime);
        }
    }
}
