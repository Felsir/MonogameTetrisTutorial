using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D11;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter3.Assets
{
    internal class Art
    {
        public static SpriteFont GameFont;

        public static void Initialize(ContentManager content)
        {
            GameFont = content.Load<SpriteFont>("IngameFont");
        }
    }
}
