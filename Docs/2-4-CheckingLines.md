# Checking lines
Or better: playing the actual game. 

The game is split into four components:
1. **The Marathon scene**: this is the "game mode" it controls the win and lose conditions, in this case: When the player is topped out, the player has lost.
2. **The Playfield object**: The representation of the game in our scene. It knows what cells are occupied, if a piece fits and what lines are completed.
3. **The Player object**: This is where the user interacts with the playfield.
4. **The tetrimino**: the active player piece that the player can manipulate.

Until now, the components interact directly by calling *methods*. In this section I'm introducing *events*. This way we can set things up by subscribing to things that happen without having to ask every frame.

Imagine this: the player completes a line. We want the playfield to highlight the line for a second and then remove the line. During that second of highlighting, the player has to wait for the gameplay to continue. There are a couple of things we can do:
1. Keep a timer in the player class to wait a second.
2. Keep checking every frame if something is highlighted and switch accordingly.
3. Or have the playfield class tell the player class once the highlighting is done.   