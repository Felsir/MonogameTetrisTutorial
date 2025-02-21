# Lock delay
The *lock delay* is the time it takes before a piece is locked in. In our current game, the tetrimino immediately locks in place once it touches a piece below.

The [Tetris Guide](https://tetris.wiki/Tetris_Guideline) mandates a 0.5 second lock delay on a soft drop.

## Strategy
Once the `droptimer` runs out, a check is made to see if the piece can move down a row- if it cannot, the lock delay should kick in. During the lock delay, the player can still move and rotate the piece. A (succesful) move or rotate will reset the timer. Because this would alow the player to keep it in play indefinetly, the number of resets is limited to 10.
If the piece would be able to drop, the drop timer is used again. This way the piece will move downwards if the player moved it away from a lockable position.
If all resets are exhausted, the piece locks immediately when touching the stack. 

## Implementation
To implement a lock delay we need a timer to keep track of the delay, our movement happens in the `Player` class, let's add these. Also things are easier if we keep track if the piece is in the lock delay mode:
```csharp
private double _lockDelayTimer; // The current lock delay time.
private int _lockDelayMoveCounter; // The count of the current moves while under lock delay.
private bool _lockDelayMode; // Keep track if the current piece is in delay mode.
private const int LOCKRESETS = 15; // total number of allowed moves during lock delay.
private const double LOCKDELAYTIME = 0.5d; // lock delay time setting
```
With these variables, the code in the `Update()` loop where the softlock is determined can be expanded to check for the `_lockDelayMode`: 

```csharp
    if (!_lockDelayMode)
    {
        // regular dropping mode.
        if (_dropTimer < 0)
        {
            // time for this row is over, can we drop the piece?
            if (_playfield.DoesShapeFitHere(_currentPiece, _x, _y + 1))
            {
                // yes
                _y++;
            }
            else
            {
                // Should the piece lock immediately?
                if (_lockDelayMoveCounter >= LOCKRESETS)
                {
                    // yes, we're run out of moves.
                    SoftlockPiece();
                }
                else
                {
                    // start the lock delay mode:
                    _lockDelayMode = true;
                    _lockDelayTimer = LOCKDELAYTIME;
                }
            }

            //reset the timer:
            _dropTimer += _dropSpeed;
        }
    }
    else
    {
        // Lock delay mode.
        _lockDelayTimer -= gameTime.ElapsedGameTime.TotalSeconds;


        if (_lockDelayTimer < 0 || _lockDelayMoveCounter >= LOCKRESETS || _dropTimer<0)
        {
            //the timer has run out, the piece should now soft lock.
            //or, the number of moves have exceeded.
            _lockDelayMode = false;

            if (_playfield.DoesShapeFitHere(_currentPiece, _x, _y + 1))
            {
                // yes, the piece can move downwards.
                _dropTimer = _dropSpeed;
            }
            else
            {
                SoftlockPiece();
            }
        }

    }
```

if you run the game now, notice how you can still move the piece for half a second before it locks in place! The first half of the mechanism is done!

The only thing left to do is to *count* every valid move under `_lockDelayMode` and reset the `_lockDelayTimer`.