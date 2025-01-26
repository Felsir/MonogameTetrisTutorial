using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chapter2.Enums;

namespace Chapter2.Tetrimino
{
    internal class Tetrimino
    {
        public Shape[] Shapes; // the shape definitions
        private int _shapeID; //the current active shape
        public Color Color; //each tetrimino has its own color
        public Tetriminoes ShapeType; //what shape is it?

        public Tetrimino()
        {
            _shapeID = 0;
        }

        public Shape CurrentShape
        {
            get
            {
                return Shapes[_shapeID];
            }
        }

        public void SetShape(Shape[] shapes, Tetriminoes type, Color color)
        {
            Shapes = shapes;
            ShapeType = type;
            Color = color;
        }

        public void RotateLeft()
        {
            _shapeID++;
            if (_shapeID > Shapes.Length - 1)
            {
                _shapeID = 0;
            }
        }

        public void RotateRight()
        {
            _shapeID--;
            if (_shapeID < 0)
            {
                _shapeID = Shapes.Length - 1;
            }
        }

        public void Draw(Matrix world, float alpha=1)
        {
            for (int y = 0; y < CurrentShape.shapeBit.Length; y++)
            {

                for (int x = 0; x < CurrentShape.shapeBit[y].Length; x++)
                {
                    if (CurrentShape.shapeBit[y][x] == false)
                        continue;

                    foreach (ModelMesh m in Assets.Models.CubeObject.Meshes)
                    {
                        foreach (ModelMeshPart part in m.MeshParts)
                        {
                            part.Effect = GameRoot.BasicEffect;

                            GameRoot.BasicEffect.World = Matrix.CreateTranslation(0.2f * x, 0.2f * -y, 0) * world ;
                            GameRoot.BasicEffect.DiffuseColor = Color.ToVector3();
                            GameRoot.BasicEffect.Alpha = alpha;
                        }
                        m.Draw();
                    }
                }
            }
        }
    }
}
