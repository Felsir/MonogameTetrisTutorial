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
}
```