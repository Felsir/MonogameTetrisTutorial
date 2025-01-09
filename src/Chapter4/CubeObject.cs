using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chapter4
{
    internal class CubeObject
    {

        private const float GRAVITY=9.81f;
        private float _a, _maxA;
        private Vector3 _position;
        private float _size;

        private Matrix _world;
        private Color _color;

        public CubeObject(float x,float z, float a) 
        {
            _position = new Vector3(x, 0, -z) * 0.35f;
            _a = a;
            _maxA = a;

            _size = 2 * (0.5f+(1-(a / 9)));

            // give each cube an unique color:
            _color = new Color(1-(x/50f), ((x+z) / 100f), 1-(z/50f), 1f);
        }

        public void Update(GameTime gt)
        {
            //simply bounce the cube on the floor of the gameworld!
            _a -= GRAVITY * (float)gt.ElapsedGameTime.TotalSeconds;
            _position.Y += _a * (float)gt.ElapsedGameTime.TotalSeconds;
            if (_position.Y < 0)
            {
                _a = _maxA; // we hit the floor, set the accelerate to bounce back up!
            }

            _world = Matrix.CreateScale(_size)*Matrix.CreateTranslation(_position);
        }

        public void Draw(InstancedCubeDrawing instancedCubeDrawing)
        {
            instancedCubeDrawing.DrawInstancedCube(_world, _color);
        }
    }
}
