using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter1.Assets
{
    internal static class Models
    {
        public static Model CubeObject;

        public static void Initialize(ContentManager content)
        {
            CubeObject = content.Load<Model>("CubeObject");
        }
    }
}
