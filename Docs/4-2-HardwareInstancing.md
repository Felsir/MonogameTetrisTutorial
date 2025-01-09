# GPU Hardware Instancing
Hardware instancing is a graphics rendering technique used to efficiently draw multiple instances of the same object with slight variations (such as position, and color) in a single draw call to the GPU. Instead of sending redundant geometry data for each instance, only the base object data and instance-specific parameters are transmitted. The GPU then replicates the base geometry using the per-instance data to render the scene. 

This approach minimizes CPU-GPU communication overhead, boosts performance, and is especially useful in scenarios like rendering forests, crowds, or particle systems where many similar objects need to be displayed. 

In our Tetris example, our main object is a cube, which has different positions and colors. So effectively all our cubes could be drawn in one single Draw action, enormously cutting down on overhead. While our game probably can cope with the limited amount of cubes being drawn, it is an excellent exercise of how to apply this technique.

## Basic principles
The way Monogame draws models, is by sending all model data and the parameters such as positioning data to the GPU. So let's assume our basic cube model consists of 288 vertices, each with position, normal and texture coordinates- in total 9 KB of data for one mesh. 

Not a lot, but let's draw 1,000 cubes: 
- 1,000 times the vertexdata of 9 KB = ~9.21 MB 
- 1,000 times position data (64 bytes) ~ 64 KB
Total ~9.28 MB and 1,000 drawcalls.

In the current state of videocards, this is still very little. However it does illustrate the massive difference if we were to use hardware instancing. In that case we only send the vertex data once (it is the same for every object we draw) and send the instance data:

- 1 time the vertexdata of 9 KB
- 1,000 times the position data of ~64 KB
Total 73 KB (or 0.071 MB) in other words, we save 99% of data and 999 drawcalls! Imagine how much you would save if your object had thousands of vertices!

## Instancing
Monogame doesn't do instancing out of the box- it does have the basic tools in the toolbox to do it though. So what do we need:
-  a vertexbuffer that contains the mesh data of the object we want to instance.
- a datastructure that holds our instance specific data, in such format the GPU can understand.
- a shader that can process the data.

Let's have a look at each component:

## The cube model buffers
This example uses the cube object as our test subject. The Monogame `Model` class holds the information we need, but we cannot use it in the instancing method. So we need to extract the vertex data and put it in our own vertex buffer.
A `VertexBuffer` is a data structure that a GPU can understand- a vertex is datapoint that is part of the object and holds information such as position, normal-vectors, color and texture coordinates of a point in 3D space. The `IndexBuffer` is a datastructure that tells the GPU what vertices form a triangle that the GPU can draw. Finally we need the count the number of triangles so we can tell the GPU how many datapoints it can expect (so we will need this later for our drawcall). 

The following code extracts the vertex data of our cube into the buffers we will need later. Note that the cube object has only one `Model.Mesh` and only one `Mesh.MeshPart`, so adapt the code accordingly to get the right data.  

```csharp
        private VertexBuffer cubeVertexBuffer = null;
        private IndexBuffer cubeIndexBuffer = null;
        private int primitivecount;

        /// ...

        public override void LoadContent()
        {
            /// ...

            Cube = content.Load<Model>("cube");

            //extract the vertexdata for instanced cube drawing
            foreach (var mesh in Cube.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    cubeVertexBuffer = part.VertexBuffer;
                    cubeIndexBuffer = part.IndexBuffer;
                    primitivecount = part.PrimitiveCount;

                    break; // The cube is one mesh with one part so the first mesh is the one we need.
                }
            }

            // ...
        }
```

## The instance data buffer
Next is we need to tell the GPU what instanced data it can expect. For our example we want to draw cubes in different positions, orientation and scale- in other words our `world` matrix. We also want to control each the `Color` for each cube. 

Our `world` data could be summarized into one `Matrix` datatype next to our `Color` datatype. 

So we need to define a structure that contains these datatypes. This datatype to tell the GPU what to expect is called a `VertexDeclaration`. Monogame has a couple of predefined of these declarations but we can also define one of our own.

The complexity is that you need to know what the size of each data component is (the "stride"). This information is used by the GPU to decode the datastructure. 

We need to send a `Matrix` and a `Color` to the GPU for each instance. Now, a `Matrix` doesn't exist- but it can be reconstructed by 4 rows of 4 floats:

```
A 4x4 Matrix:
{ 
    M11, M12, M13, M14,
    M21, M22, M23, M24,
    M31, M32, M33, M34,
    M41, M42, M43, M44
}
```
...also 4 floats can be represented by a `Vector4(x, y, z, w)`. To calculate the stride, a float is 4 bytes; so a `Vector4` is 16 bytes. We will classify the data as Position data in our declaration. 
To summarize: we declare a matrix as 4 `Vector4` elements, one for each row. Each row has a size of 16 bytes. 

This produces the following declaration for a Matrix and Color component:

```csharp
            VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1), // Row 0
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Position, 2), // Row 1
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 3), // Row 2
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 4),  // Row 3
                new VertexElement(64 , VertexElementFormat.Vector4, VertexElementUsage.Color,0)
            );
```
Note how each new piece of the declaration increase by the 16 bytes needed for the previous row. Basically row 3 starts 48 bytes from the start.

Now we need something to store this data in so we can transport it to the GPU. For this we're going to use a `DynamicVertexBuffer`- it tells Monogame and the GPU that this is a buffer optimised for writing, as we are going to rewrite the data every frame (in each frame the data can be different).

We also need to reserve space in the buffer- for now let's reserve space for 2500 cubes.

```csharp
instanceBuffer = new DynamicVertexBuffer(_gd, instanceVertexDeclaration, 2500, BufferUsage.WriteOnly);
```



