using Chapter43.Assets;
using Chapter43.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection.Metadata;

namespace Chapter43
{
    public class GameRoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _shadowMapRenderTarget;
        
        public static Camera.Camera Camera;
        public static Light.ShadowCastingLight Light;

        // A few cubes set the scene
        private static CubeObject[] cubeObjects;

        //make the scene a bit more dynamic!
        private static double _cubeRotation, _cameraRotation;

        public GameRoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.IsFullScreen = false;
            IsMouseVisible = true;

            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Models.Initialize(Content);
            Shaders.Initialize(Content);

            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 4096, 4096, false,
                                        SurfaceFormat.Single,
                                        DepthFormat.Depth24);


            Camera = new Camera.Camera(new Vector3(2,0.75f,2f),new Vector3(0,-0.75f,0), _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, MathHelper.ToRadians(60));
            Light = new Light.ShadowCastingLight(new Vector3(0.5f, 0.75f, 0.75f));

            cubeObjects = new CubeObject[3];

            //Create 3 cubes to set up our scene.
            cubeObjects[0] = new CubeObject(new Vector3( 0     ,  0    , 0    ), 0.5f);
            cubeObjects[1] = new CubeObject(new Vector3( 0     , -0.1f , 0    ), 1);
            cubeObjects[2] = new CubeObject(new Vector3(-0.125f, -0.85f,-0.25f), 5);

            //Setup some fixed parameters in the shader:
            Shaders.DiffuseEffect.Parameters["Projection"].SetValue(Camera.Projection);
            Shaders.DiffuseEffect.Parameters["LightDirection"].SetValue(Light.LightDirection);
            Shaders.DiffuseEffect.Parameters["ShadowTexture"].SetValue(_shadowMapRenderTarget);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Add some dynamics into the scene:

            // Rotate cube 0 around cube 1
            _cubeRotation += gameTime.ElapsedGameTime.TotalSeconds;
            _cubeRotation %= MathHelper.Pi * 2;

            Vector3 cubePos = new Vector3((float)(Math.Sin(_cubeRotation) * 0.23f), 0.0f, (float)(Math.Cos(_cubeRotation) * 0.23f));
            cubeObjects[0].Position = cubePos;

            // Rotate the camera around the scene.
            _cameraRotation += 0.5d * gameTime.ElapsedGameTime.TotalSeconds;
            _cameraRotation %= MathHelper.Pi * 2;

            Camera.SetCameraPosition(new Vector3((float)Math.Sin(_cameraRotation) * 2, 0.75f, (float)Math.Cos(_cameraRotation) * 2));
            Camera.SetCameraTarget(new Vector3(0, -0.75f, 0));


            // Because the camera has changed, we need to recalculate the light projection:
            Matrix lightviewprojection = Light.CalculateMatrix(Camera.View, Camera.Projection);

            // Update all parameters that have changed:
            // In the shadowmap shader:
            Shaders.ShadowMapEffect.Parameters["ViewProjection"].SetValue(lightviewprojection);
            // In the diffuse shader:
            Shaders.DiffuseEffect.Parameters["View"].SetValue(Camera.View);
            Shaders.DiffuseEffect.Parameters["LightViewProjection"].SetValue(lightviewprojection);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Switch to our shadowmap render target so we can render from the lightsource viewpoint:
            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            
            // The scene is rendered as normal, with the shadowmap effect:
            for (int i = 0; i < 3; i++)
            {
                cubeObjects[i].Draw(Shaders.ShadowMapEffect);
            }

            // Switch back to the backbuffer
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            // Rendere everything with the normal diffuse effect
            for (int i = 0; i < 3; i++)
            {
                cubeObjects[i].Draw(Shaders.DiffuseEffect);
            }

            base.Draw(gameTime);
        }
    }
}
