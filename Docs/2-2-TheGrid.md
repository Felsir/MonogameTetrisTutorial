# The Grid
The grid is the playfield of the game. It has a couple of important functions:
* Check the player piece against the occupied cells.
* Fill cells when the player piece can no longer move downwards.
* Check for completed lines.
* Remove completed lines.

## Cells
The core of the grid is a cell. The cell tells us if that space is occupied or not. Additionally we want to keep record of what color the piece had that was dropped there. So our cell structure is quite simple:

```csharp
    internal struct Cell
    {
        public bool Occupied;
        public Color Color;
    }
```

Now it is time to construct the grid.

## The playfield
As mentioned before- the grid has a few tasks, but first we need to construct it.


```csharp
    internal class Playfield
    {
        private Cell[][] _cells;
        private const int COLUMNS = 10;
        private const int LINES = 20;

        private Vector3 _position;

        public Playfield(Vector3 position)
        {
            _position = position;

            _cells = new Cell[LINES][];

            for (int i = 0; i < LINES; i++)
            {
                _cells[i] = new Cell[COLUMNS];

                for (int j = 0; j < COLUMNS; j++)
                    _cells[i][j] = new Cell() { Occupied = false, Color=Color.Black }; 
                    // Color doesn't really matter as we're not going to draw unoccupied spaces.
            }
        }
    }
```

This initializes our playfield. I also added a `position` variable, so we can position the playfield in our game, once we start to visualize it. Keep in mind that the grid is structured `[lines][columns]` which may be counter intuitive as lines are on the y axis and columns on the x axis. The reason for this is that we are going to deal with lines later, so it is easier to have that as our primary index. If it is confusing now, you'll see later.

### Drawing the playfield
Drawing is quite simple, you will see that there isn't all that much of a difference if we were to construct this game in 2D- most of the code and calculations are quite similar! Let's have a look:

The playfield is made up out of cubes. Since we're going to draw a few of them, let's create a little helper function, we can use the `Asset.Models` class for this:
```csharp
    internal static class Models
    {

        //...

        public void DrawCube(Matrix world, Color color)
        {
            foreach (ModelMesh m in CubeObject.Meshes)
            {
                foreach (ModelMeshPart part in m.MeshParts)
                {
                    part.Effect = GameRoot.BasicEffect;
                    GameRoot.BasicEffect.World = world
                    GameRoot.BasicEffect.DiffuseColor = color.ToVector3();
                }
                m.Draw();
            }
        }
    }
```

Now we can draw a bunch of cubes to show the grid. Note how the calculation is based on simple x and y grid of 0.2f units per cube. Also note how there are 2 translations added: the individual cube position and the position of the grid itself as defined by th `_position` variable. The top left of the actual playfield is at `_position`, the border extends one cube outwards (therefore the cubes on the left side border are at `x= -1 * 0.2f`, meaning *one cube left of `_position.X`):

```csharp
        public void Draw()
        {
            for (int y = 0; y < LINES; y++)
            {
                //left border:
                Assets.Models.DrawCube(
                    Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(-1 * 0.2f, -y * 0.2f, 0), 
                    Color.Gray);

                //right border:
                Assets.Models.DrawCube(
                    Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(COLUMNS * 0.2f, -y * 0.2f, 0), 
                    Color.Gray);

                //cells in this line:
                for (int x = 0; x < COLUMNS; x++)
                {
                    //if the cell is empty, skip to the next one.
                    if (!_cells[y][x].Occupied)
                        continue;

                        Assets.Models.DrawCube(
                            Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(x * 0.2f, -y * 0.2f, 0), 
                            _cells[y][x].Color);
                }
            }

            //Draw bottom of the grid
            for (int x = -1; x < COLUMNS+1; x++)
            {
                Assets.Models.DrawCube(Matrix.CreateTranslation(_position) * Matrix.CreateTranslation(x * 0.2f, -20 * 0.2f, 0), 
                Color.Gray);
            }
        }
```

## Check the player's piece against the grid
An important task is to actually know if the piece the player controls can go where the player wants it to go. Since the player's shape is defined by a set of booleans and our grid `IsOccupied` variable is a boolean; checking is really simple! I have added comments in the code for each check. We can simply return out of these loops, as soon as only 1 check fails, the entire shape cannot fit in the designated area!

```csharp
        public bool DoesShapeFitHere(Tetrimino shape, int leftcolumn, int topline)
        {
            //loop over the bits in our shape:
            for (int y = 0; y < shape.CurrentShape.shapeBit.Length; y++)
            {
                for (int x = 0; x < shape.CurrentShape.shapeBit[y].Length; x++)
                {
                    //We only need to check bits that are set to true
                    if (shape.CurrentShape.shapeBit[y][x])
                    {
                        //so this is a filled bit of the shape!

                        //we return false if the shape tries to fit in the border:
                        //check for bottom:
                        if (topline + y >= LINES)
                            return false;

                        //check for left wall:
                        if (leftcolumn + x < 0)
                            return false;

                        //check for right wall:
                        if (leftcolumn + x >= COLUMNS)
                            return false;

                        //We're not checking the top- a piece spawns above the playfield!
                        //bonus, we prevent the array out of bounds
                        if (topline + y < 0)
                            continue;

                        //now for the grid:
                        //if both the bit in the shape is true
                        //and the cell is occupied, 
                        //the shape can not fit!
                        if (_cells[topline+y][leftcolumn+x].Occupied)
                            return false;
                    }
                }
            }

            //all checks came out clear, so it fits!
            return true;
        }
```

