using Chapter43CC.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter43CC
{
    public class CubeObject
    {
        public Vector3 Position;
        public float Size;
        public BoundingSphere BoundingSphere;

        public CubeObject(Vector3 position, float size=1)
        {
            Position = position;
            Size = size;

            BoundingSphere = Models.CubeObject.Meshes[0].BoundingSphere.Transform(Matrix.CreateScale(Size) * Matrix.CreateTranslation(Position));
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


