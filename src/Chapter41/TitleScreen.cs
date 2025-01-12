using Chapter1.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter1
{
    internal class TitleScreen : IScene
    {
        // Let's rotate the cube for a nice visual effect.
        private float _angle;

        public TitleScreen() 
        { 
        }

        public void Update(GameTime gameTime)
        {
            // Increase the angle framerate independant:
            // 0.75 radians per second, also keep the rotation within the 2 PI bound.
            _angle += 0.75f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _angle %= MathHelper.TwoPi;
        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            // Draw the cube mesh.
            foreach (ModelMesh m in Assets.Models.CubeObject.Meshes)
            {
                // This is generic- eventhough the cube only has one meshpart, 
                // Let's keep the code so you can experiment with different models.
                foreach (ModelMeshPart part in m.MeshParts)
                {
                    // Assign the shader effect to this meshpart:
                    part.Effect = GameRoot.MyEffect;

                    // Position the object in the world: Move it to the coordinates (0,0,-3), rotate the object around the Y axis and increase the scale by 20:
                    GameRoot.MyEffect.Parameters["World"].SetValue(Matrix.CreateScale(20) * Matrix.CreateRotationY(_angle) * Matrix.CreateTranslation(0, 0, -3f));

                    // Color this cube LightGray- which is multiplied by the texture. Shaders don't "know" colors, so RGB is represented as a Vector3.
                    GameRoot.MyEffect.Parameters["DiffuseColor"].SetValue(Color.LightGray.ToVector3());
                    GameRoot.MyEffect.Parameters["Texture"].SetValue(GameRoot.TestTexture);

                }
                m.Draw();
            }
        }
    }
}
