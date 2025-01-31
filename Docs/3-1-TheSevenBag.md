# The 7 Bag
If you have played the game from the code, you will notice the randomness of the pieces isn't always fun. In some games you'll get a bunch of Z pieces and no I pieces[^1]. To counter this, the concept of the "Seven Bag" was introduced in the Tetris Guideline[^2].

The Seven Bag is a virtual bag where the 7 tetrimino pieces are thrown in and a random piece is drawn from the bag. The bag is empties first before a new set of the 7 pieces is thrown in the bag. 

## Implementation
The bag is implemented as a `List`. We simply draw a piece from a random index in the list, the item is then removed from the list, until the list is empty. The empty list is simply filled again with all 7 pieces.

The random generator is implemented in the `TetriminoFactory` class, by reimplementing the `GenerateRandom` function:
```csharp
private List<Tetriminoes> _SevenBag = new List<Tetriminoes>();

public Tetrimino GenerateRandom()
{
    if(_SevenBag.Count==0) // if the bag is empty...
    {
        // Create a new bag of pieces:
        _SevenBag.Add((Tetriminoes)0);
        _SevenBag.Add((Tetriminoes)1);
        _SevenBag.Add((Tetriminoes)2);
        _SevenBag.Add((Tetriminoes)3);
        _SevenBag.Add((Tetriminoes)4);
        _SevenBag.Add((Tetriminoes)5);
        _SevenBag.Add((Tetriminoes)6);
    }

    // the piece that is picked is taken at random from the list.
    int i=_random.Next(_SevenBag.Count);
    Tetrimino t = Generate(_SevenBag[i]);

    // remove it from the source list.
    _SevenBag.RemoveAt(i);

    return t;
}
```

[^1]: Check the [Tetris God clip](https://www.youtube.com/watch?v=Alw5hs0chj0)...
[^2]: [Tetris Wiki](https://tetris.wiki/Random_Generator)