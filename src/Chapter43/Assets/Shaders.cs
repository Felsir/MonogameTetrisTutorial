using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter43.Assets
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
