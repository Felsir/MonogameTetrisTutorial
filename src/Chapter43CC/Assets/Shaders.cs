using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Chapter43CC.Assets
{
    internal class Shaders
    {
        public static Effect ShadowMapEffect;
        public static Effect DiffuseEffect;

        public static void Initialize(ContentManager content)
        {
            ShadowMapEffect = content.Load<Effect>("ShadowMap"); // used to render the shadowmap
            DiffuseEffect = content.Load<Effect>("DiffuseEffect"); // used to render the final scene
        }
    }
}
