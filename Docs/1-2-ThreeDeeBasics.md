# 3D Basics
This time we're going to make the game using the most basic 3D shape: a cube. Since this cube is still a 3D-model you can apply the same technique to other shapes. There is only one caveat- the shapes are static- animated meshes are a different cup of tea. For the purpose of this tutorial that is sufficient.

## Orientation in 3D space
In our 2D world everything is quite straightforward. The dimensions are measured in pixels, and the amount of pixels are determined by the size of the (windowed) screen. Sure, there are 2D camera classes and the gameworld can be way bigger than the actual screen- but the concept is fairly straightforward. 

In 3D, the relation to the items in the world are not as straightforward. Let's break it down in a few components:

#### World
The world has three dimensions, X, Y and Z. At (0,0,0) lies the world origin.
The convention is that, when looking the neutral direction:
* X-axis goes from left (negative) to right (positive). 
* Y-axis goes from up (positive) to down (negative)
* Z-axis goed from far (negative) to near (positive)
So any object can be put in the world at any 3D position represented by a Vector3.

#### View
It depends what direction you're looking, so our 'camera' is defined by two points: where the camera is positioned and where the camera is pointed at. This seems simple enough, but keep in mind- the camera can also be tilted, so we need to add another thing: which direction is 'up'. 
* The camera is positioned somewhere in the 3D world: a Vector3 Position.
* The camera is pointed at something in the 3D world: a Vector3 Look at target.
* The camera needs to know which direction is the top: a Vector3 pointing upwards.

#### Projection
The final component is the projection- think of it as a frame through which the world is viewed. On top of that, the lens- how much can be seen. Finally it determines the type of projection.
* The field of view, how wide the 'lens' of the camera is.
* The aspect ratio of the viewport.
* The type of projection- perspective, orthogonal.

## Matrices
How are these things defined? Well, Matrices are the answer. A 4x4 matrix can contain both rotation, translation information. Good thing we don't need to know *how* these work, ([Matrix calculus](https://en.wikipedia.org/wiki/Matrix_(mathematics)) is a different topic all together!). Lucky for us, most of the heavy lifting is done for us.
The View and Projection matrices as quite simple- we'll get to that in a minute. The World matrix needs special attention. 
As stated before- an object will be moved according to the World coordinates. Since each object also has it's own 'origin'- you can see it when the object is at position (0,0,0). The World matrix contains all information to move the object as desired.
Let's say we want to scale the object twice its size, rotate it by 45 degrees along its Y axis and move it to (1,0,-1):
```csharp
Matrix World = Matrix.CreateScale(2) * Matrix.CreateRotationZ(MathHelper.ToRadians(45))*Matrix.CreateTranslation(New Vector3(1,0,-1));
```
Note how the order of things is important. The movement is in relation to the objects orientation! Easiest way is to read the chain of matrix multiplications from back to front. So first the object is moved to (1,0,-1) *then* it is rotated. Imagine what would happen if we rotate the object first: it would be moved to a completely different place!

## Camera object
To make things easier, we will make a very simple 3D camera class. Like so:

```csharp
public class Camera
{
    private int _screenWidth, _screenHeight;
    private float _fieldOfView;
    private Vector3 _position, _target;
    
    private Matrix _view, _projection;

    //these two values determine the distance a projection can "see". 
    //In this case the closest object we can render is 0.01 unit from the camera.
    //The furthest the camera can see is 100 units away. 
    private const float nearPlane = 0.01f;
    private const float farPlane = 100f;

    public Camera(Vector3 position, Vector3 target, int screenWidth, int screenHeight, float fieldOfView)
    {
        _screenWidth=screenWidth;
        _screenHeight=screenHeight;
        _fieldOfView=fieldOfView;

        _position = position;
        _target = target;

        CalculateMatrices();
    }

    private void CalculateMatrices()
    {
        _view = Matrix.CreateLookAt(_position, _target, Vector3.Up);
        float aspect = (float)_screenWidth/(float)_screenHeight;
        _projection = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, nearPlane, farPlane);
    }

    public Matrix View
    {
        get
        {
            return _view;
        }
    }

    public Matrix Projection
    {
        get
        {
            return _projection;
        }
    }

    public void SetCameraPosition(Vector3 position)
    {
        _position=position;
        CalculateMatrices();
    }

    public void SetCameraTarget(Vector3 target)
    {
        _target=target;
        CalculateMatrices();
    }
}
```
The camera we need for this project is very simple- it doesn't need to move much. We point the camera at the origin and we're almost ready to view some 3D objects!

## Shaders
Yes, we cannot go 3D without discussing shaders. Monogame comes with a built in shader called `BasicEffect`. We can use that one and create our game. But what is a shader?

A shader is a small program that runs on the videocard- the GPU. In most cases it uses two steps: the Vertex shader and the Fragment (or Pixel) shader. The Vertex shader calculates, based on the 3D object data, what pixels onscreen are affected. The pixel shader in turn calculates for each pixel what color that pixel should be. If you really want to do a deep dive, I highly recommend watching [this video](https://www.youtube.com/watch?v=C8YtdC8mxTU).

For now, we'll use the `BasicEffect` built in by Monogame, and revisit the [shader topic later](4-1-Shaders.md)!

#### Continue
Next step: [The cube](1-3-TheCube.md).