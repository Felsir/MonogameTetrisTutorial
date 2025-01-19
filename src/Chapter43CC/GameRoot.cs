using Chapter43CC.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Chapter43CC
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

        //for visualisation purposes
        private bool _showCascades = false;

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

            // The rendertarget is RGBA so we can encode depth data in each color component.
            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 4096, 4096, false,
                                        SurfaceFormat.Color,
                                        DepthFormat.Depth24);


            Camera = new Camera.Camera(new Vector3(2,0.75f,2f),new Vector3(0,-0.75f,0), _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, MathHelper.ToRadians(60));
            Light = new Light.ShadowCastingLight(new Vector3(0.5f, 0.75f, 0.75f));

            //Setup some fixed parameters in the shader:
            Shaders.DiffuseEffect.Parameters["Projection"].SetValue(Camera.Projection);
            Shaders.DiffuseEffect.Parameters["LightDirection"].SetValue(Light.LightDirection);
            Shaders.DiffuseEffect.Parameters["ShadowTexture"].SetValue(_shadowMapRenderTarget);
            Shaders.DiffuseEffect.Parameters["CascadeSplits"].SetValue(Camera.Splits);

            // Have some Yellowish cubes
            Shaders.DiffuseEffect.Parameters["DiffuseColor"].SetValue(Color.Goldenrod.ToVector4());

            // Setting up the scene:
            //
            cubeObjects = new CubeObject[60];

            // Create 3 cubes for the close up visuals.
            // Cube 0 will be rotated in the Update() loop to show the shadows move.
            cubeObjects[0] = new CubeObject(new Vector3(0, 0, 0), 0.5f);
            cubeObjects[1] = new CubeObject(new Vector3(0, -0.1f, 0), 1);
            cubeObjects[2] = new CubeObject(new Vector3(-0.125f, -0.85f, -0.25f), 5);

            // Add a row to demonstrate deeper cascades
            for (int i = 0; i < 33; i++)
                cubeObjects[3 + i] = new CubeObject(new Vector3(0, -0.2f, -0.3f - i * 0.25f), 1);

            // Add another row of sightly bigger cubes so a shadow is cast
            for (int i = 0; i < 23; i++)
                cubeObjects[3 + 33 + i] = new CubeObject(new Vector3(-0.125f, -0.85f, -1f - i * 0.7f), 3);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                _showCascades = true;
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                _showCascades = false;
            }
            Shaders.DiffuseEffect.Parameters["ShowCascades"].SetValue(_showCascades);

            #region Camera movement
            // Add some dynamics into the scene:
            // Rotate cube 0 around cube 1
            _cubeRotation += gameTime.ElapsedGameTime.TotalSeconds;
            _cubeRotation %= MathHelper.Pi * 2;

            Vector3 cubePos = new Vector3((float)(Math.Sin(_cubeRotation) * 0.23f), 0.0f, (float)(Math.Cos(_cubeRotation) * 0.23f));
            cubeObjects[0].Position = cubePos;

            // Rotate the camera around the scene.
            _cameraRotation += 0.5d * gameTime.ElapsedGameTime.TotalSeconds;
            _cameraRotation %= MathHelper.Pi * 2;

            Camera.SetCameraPosition(new Vector3((float)Math.Sin(_cameraRotation) * 2f, 0.75f, (float)Math.Cos(_cameraRotation) * 2f));
            Camera.SetCameraTarget(new Vector3(0, -0.25f, 0));
            #endregion


            // Because the camera has changed, we need to recalculate the light projection:

            // Update all parameters that have changed:
            Shaders.DiffuseEffect.Parameters["View"].SetValue(Camera.View);

            // We need to tell the shader the matrices of each cascade:
            Shaders.DiffuseEffect.Parameters["LightViewProjection"].SetValue(new Matrix[3] {
                Light.CalculateMatrix(Camera.View, Camera.CascadeProjection[0]),
                Light.CalculateMatrix(Camera.View, Camera.CascadeProjection[1]),
                Light.CalculateMatrix(Camera.View, Camera.CascadeProjection[2])
            });

            // Changes in the shadow map shader will be done during the draw phase as each cascade will have new parameters.

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Switch to our shadowmap render target so we can render from the lightsource viewpoint:
            GraphicsDevice.SetRenderTarget(_shadowMapRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            // The scene is rendered as normal, with the shadowmap effect:
            for (int cascade = 0; cascade < 3; cascade++)
            {
                // For each cascade we need to clear the depth buffer-
                // we are going to render the entire scene anew for each cascade
                // so we must have a depth buffer during the casecase but
                // not carry it over to the next.
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                // We have a blendstate for each cascade because we only want to affect the
                // the colors that are in that particular cascase
                // Red for near, Green for mid and Blue for far distances.
                GraphicsDevice.BlendState = Light.CascadeBlendState(cascade);

                Matrix cascadeViewProjection = Light.CalculateMatrix(Camera.View, Camera.CascadeProjection[cascade]);
                
                // Set the view projection uniform in our shader, and tell the shader what cascade we're rendering.
                Shaders.ShadowMapEffect.Parameters["ViewProjection"].SetValue(cascadeViewProjection);
                Shaders.ShadowMapEffect.Parameters["Cascade"].SetValue(cascade);

                // Render all cubes for this cascade.
                for (int i = 0; i < 59; i++)
                {
                    cubeObjects[i].Draw(Shaders.ShadowMapEffect);
                }
            }

            // Now to render the final scene with cascade shadow effect
            // Switch back to the backbuffer
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Navy);
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Rendere everything with the normal diffuse effect
            for (int i = 0; i < 59; i++)
            {
                cubeObjects[i].Draw(Shaders.DiffuseEffect);
            }

            // Toggle the left control/left shift key to see the generated shadowmap:
            if (_showCascades)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_shadowMapRenderTarget, new Rectangle(0, 0, 300, 300), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
