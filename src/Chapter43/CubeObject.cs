using Chapter43.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter43
{
    public class CubeObject
    {
        public Vector3 Position;
        public float Size;

        public CubeObject(Vector3 position, float size=1)
        {
            Position = position;
            Size = size;
        }

        public void Draw(Effect effect)
        {
            //We assume all specific parameters are already set in the shader.
            //so we only calculate the world matrix to position our object.
            Matrix world = Matrix.CreateScale(Size) * Matrix.CreateTranslation(Position);

            foreach (ModelMesh m in Models.CubeObject.Meshes)
            {
                foreach (ModelMeshPart part in m.MeshParts)
                {
                    // Assign the shader effect to this meshpart:
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world);
                }
                m.Draw();
            }
        }
    }
}


