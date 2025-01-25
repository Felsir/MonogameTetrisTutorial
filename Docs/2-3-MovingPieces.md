# Moving pieces
In our previous section we have all components made to start our basic game.

So let's give the player control and play the game!

## Handling input
In our example game, the player will interact using the keyboard. To keep things simple, while at the same time give room to improve, the input will be handles by a separate class. This paves the way for further customisation in the input, adding gamepad or user defined control schemes without having to change the gameplay logic.
There are many ways to do this, so adapt to suit your needs.

```csharp
internal class InputManager
{
    private KeyboardState _oldState, _newState;

    public enum Controls
    {
        Left,
        Right,
        SoftDrop,
        HardDrop,
        RotateCW,
        RotateCCW
    }

    Dictionary<Controls, Keys> Controlscheme;

    public InputManager()
    {
        //initialize these states.
        _newState = Keyboard.GetState();
        _oldState = _newState;

        //define the controlscheme for our game:
        Controlscheme = new Dictionary<Controls,Keys>();

        Controlscheme.Add(Controls.Left, Keys.Left);
        Controlscheme.Add(Controls.Right, Keys.Right);
        Controlscheme.Add(Controls.SoftDrop, Keys.Down);
        Controlscheme.Add(Controls.HardDrop, Keys.Up);
        Controlscheme.Add(Controls.RotateCW, Keys.LeftControl);
        Controlscheme.Add(Controls.RotateCCW,Keys.LeftShift);

    }

    public void Update()
    {
        _oldState = _newState;
        _newState=Keyboard.GetState();
    }

    public bool IsPressed(Controls key)
    {
        if(!Controlscheme.ContainsKey(key))
            throw new Exception("Control key does not exist.");

        Keys k = Controlscheme[key];
        return _oldState.IsKeyUp(k)&&_newState.IsKeyDown(k); 
    }

    public bool IsDown(Controls key)
    {
        if(!Controlscheme.ContainsKey(key))
            throw new Exception("Control key does not exist.");

        Keys k = Controlscheme[key];
        return _newState.IsKeyDown(k); 
    }

    public bool IsReleased(Controls key)
    {
        if(!Controlscheme.ContainsKey(key))
            throw new Exception("Control key does not exist.");

        Keys k = Controlscheme[key];
        return _oldState.IsKeyDown(k)&&_newState.IsKeyUp(k);
    }
}
```

In many games, the behavior of key presses depends on whether a key was just pressed, is being continuously held, or was just released. To handle this, we update the current `KeyboardState` every frame and retain the previous frame's state. By comparing these two states, we can determine the exact timing of key actions, such as whether a key was just pressed.

Additionally, using a dictionary to map game controls to specific keys decouples the game logic from hardcoded key bindings. While the constructor currently assigns hardcoded values, this can easily be replaced with values from a configuration file, allowing dynamic updates to key mappings during runtime.

Finally, this approach can be expanded to support other input devices, such as gamepads. By implementing similar logic for gamepad states, input handling remains unified and game logic does not need to change, making it straightforward to support multiple input methods.

### One more thing!
The [Tetrimino factory](2-1-Tetriminos.md) should generate pieces at random[^1]. We could assign a `seed` to the factory so it generates the same sequence every time- useful if you want to expand this game to a versus battle where both players should have equal opportunities. For now we simply add a randomizer to generate a random piece. So in our `TetriminoFactory` class:

```csharp
    private Random _random=new Random();

    public Tetrimino GenerateRandom()
    {
        return Generate((Tetriminoes)_random.Next(7)); // pick one of 7 possible pieces.
    }
```

[^1]:There is a specific algorithm in Tetris for random pieces, which is another topic later. For now, we stick with the simple random function.

## The Player
Finally we have arrived at the player! What does the player object control? 
* The current dropping piece
* It's reference to the playing field on which the player is playing
* Getting the next piece
* Keeping track of the player score and level

There are many subtle mechanics in Tetris, but right now it is time to focus on the basics.

```csharp
internal class Player
{
    private Grid.Playfield _playfield;
    private Tetrimino.TetriminoFactory _pieceFactory;

    private InputManager _playerInput;

    private Tetrimino.Tetrimino _currentPiece; // the piece under player control
    private int _x,_y; // position of the current piece

    public Player(Grid.Playfield playfield)
    {
        _playfield = playfield; // assign a playfield to the player.
        _pieceFactory = new Tetrimino.TetriminoFactory(); // a way to generate new pieces.

        _playerInput = new InputManager();

        GeneratePiece(); // Give the player a piece to start with.
    }

    private void GeneratePiece()
    {
        _currentPiece = _pieceFactory.GenerateRandom();
        _x=5;
        _y=-2; //yes, the piece actually starts above the playfield.
    }
}
```
As you can see, the player is initialized with a playfield. The reason this is done is because if the gamemode is a 2 player version, the actual game screen is the one that determines where this player's game is shown. Since we want to contain the logic of the Tetris game as much as possible in one place, the `Player` class handles most of the gameplay logic. 

```csharp
    public void Draw()
    {
        _playfield.Draw();
    }
```

But wait- the player object has no clue where the playfield is in the 3D world- This is the responsibility of the playfield; so let's add a draw function to the playfield to draw a piece at the game coordinates (row, column) in the right place in the 3D world.

In the `Playfield` class:
```csharp
    public void DrawTetrimino(Tetrimino.Tetrimino t, int column, int row)
    {
        Matrix world = Matrix.CreateTranslation(_position) * 
                    Matrix.CreateTranslation(column * 0.2f, -row * 0.2f, 0);

        t.Draw(world); 
    }
```

Now we can complete the `Draw()` method in the `Player` class:

```csharp
    public void Draw()
    {
        _playfield.Draw();
        _playfield.DrawTetrimino(_currentPiece, _x, _y);
    }
```

Let's add the `Update()` method in the `Player` class to execute our gameplay logic.

```csharp
    public void Update(GameTime gt)
    {

    }
```

Okay, we now have almost everything to start our gameplay. For completeness we're going to introduce the `Marathon` scene- Marathon is the default gameplay of Tetris where a player tries to score as many points and keep the game going for as long as possible.

This class will hold the player object and keep track of the win or lose conditions. This way we can introduce *sprint*, *ultra* and *versus* gamemodes.[^2]

```csharp

```

[^2]: *Sprint* is a complete a set number of lines as fast as possible, *ultra* is score as many points in a set timelimit, *versus* is a competitive mode where scoring lines gives the opponent so called 'garbage' lines.