using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Chapter42
{
    internal class InstancedCubeDrawing
    {
        private VertexBuffer _cubeVertexBuffer = null;
        private IndexBuffer _cubeIndexBuffer = null;
        private int _primitivecount;


        private DynamicVertexBuffer _instanceBuffer;
        private CubeInstanceData[] _instanceData = new CubeInstanceData[2500]; //reserve some space
        private int _instanceCount;
        private bool _beginCalled = false;

        private Effect _cubeInstanceEffect;

        private GraphicsDevice _graphicsDevice;

        public InstancedCubeDrawing(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void LoadContent(ContentManager content)
        {
            _cubeInstanceEffect = content.Load<Effect>("InstanceEffect");

            //Load the cube locally- we only need it to extract the data for the buffers.
            Model cube = content.Load<Model>("CubeObject");

            //extract the vertexdata for instanced cube drawing
            foreach (var mesh in cube.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    _cubeVertexBuffer = part.VertexBuffer;
                    _cubeIndexBuffer = part.IndexBuffer;
                    _primitivecount = part.PrimitiveCount;

                    break; // The cube is one mesh with one part so the first mesh is the one we need.
                }
            }

            VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1), // Row 0
                    new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Position, 2), // Row 1
                    new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 3), // Row 2
                    new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 4),  // Row 3
                    new VertexElement(64, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
                );

            _instanceBuffer = new DynamicVertexBuffer(_graphicsDevice, instanceVertexDeclaration, _instanceData.Length, BufferUsage.WriteOnly);

        }

        public void SetEffectParameters(Matrix view, Matrix projection)
        {
            _cubeInstanceEffect.Parameters["View"].SetValue(view);
            _cubeInstanceEffect.Parameters["Projection"].SetValue(projection);
        }


        public void BeginCubeInstance()
        {
            if (_beginCalled)
                throw new InvalidOperationException("Begin cannot be called without Ending the previous Instanced Draw session.");

            _instanceCount = 0;
            _beginCalled = true;
        }

        public void DrawInstancedCube(Matrix world, Color color)
        {
            if (!_beginCalled)
                throw new InvalidOperationException("BeginCubeInstance must be called first.");

            if (_instanceCount >= _instanceData.Length)
                return; //prevent overflow of instance buffer

            _instanceData[_instanceCount].World = world;
            _instanceData[_instanceCount].CustomColor = color.ToVector4();
            _instanceCount++;
        }

        public void EndCubeInstance()
        {
            if (!_beginCalled)
                throw new InvalidOperationException("BeginCubeInstance must be called first.");

            // Update the instance buffer with the data:
            _instanceBuffer.SetData(_instanceData);

            // Bind the buffers to the GraphicsDevice, first the mesh, next the instance data.
            _graphicsDevice.SetVertexBuffers(
                new VertexBufferBinding(_cubeVertexBuffer, 0, 0),
                new VertexBufferBinding(_instanceBuffer, 0, 1)
            );

            //Tell the GraphicsDevice what indices describe the triangles in the vertexbuffer.
            _graphicsDevice.Indices = _cubeIndexBuffer;

            //Use the instanced cube drawing shader:
            _cubeInstanceEffect.Techniques[0].Passes[0].Apply();

            //This is the single draw call to draw everything!
            _graphicsDevice.DrawInstancedPrimitives(
                PrimitiveType.TriangleList,
                0, // baseVertex, we begin at the first (zero-based) vertex.
                0, // startIndex, we begin also at the first (zero-based) datapoint.
                _primitivecount, // how many triangles does the mesh have?
                _instanceCount // Only draw the intances we've actually set.
            );

            //We're done! Reset the session!
            _beginCalled = false;
        }
    }
}
