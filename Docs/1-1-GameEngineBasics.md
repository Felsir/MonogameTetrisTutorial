# Game Engine Basics
In this part we set up our Monogame project and add a few basic concepts, these will help out later!
It is by no means a "you should do it like this" and you are free to skip it. It serves as an indication on how I structure my code. Ofcourse, if you are using other Monogame libraries such as [Monogame.Extended](https://github.com/craftworkgames/MonoGame.Extended) or [Nez](https://github.com/prime31/Nez), you may want to do things differently- if you are already that advanced, skipping this section will be fine!

## Create the project
First let's start with a blank Monogame project. For this tutorial I use the Desktop DX version, but most stuff will translate to any project type. I assume you already know how to do this- otherwise, please have a look [here](https://docs.monogame.net/articles/getting_started/index.html).

# My way of work
Some of the following sections are personal preference. It might be worth reading just for glimpse into my mind, or just for fun. If you are an experienced Monogamer, you might have different tricks so feel free to skip!

## The GameRoot
The first thing I usually do is change the `Game1` class to `GameRoot`. This is optional, I just don't like the template feel of a class named "Game1". In Visual Studio, you can rightclick the filename `Game1.cs` pick *Rename* and change the name. This way the references should also change. Easy!

## Scene management
This section my main focus is on keeping the core game loop as simple as possible. The game may have a title screen, options and various game modes; it would become messy if all these are handled in our GameRoot- so it is best to offload them to a separate class. So how to handle this?

### Scenes
First let's start by the concept: Let's say we have a Title scene, and Options scene and a Gameplay scene. We can see these scenes as a stack of cards. The Title card is shown, we put if on the table. If the player starts a game, we put the Gameplay card on top of the stack of cards. Game over? Easy, remove the Gameplay card from the stack and the title is shown.

So let's implement the scenemanager as using a `Stack`! By simply `Push()` new scenes and `Pop()` old scenes we can manipulate the active scene. Also the scene that is on top can be looked at using `Peek()`, so our basic scene management look like this:

Let's add an interface to have the basics of a scene:
```csharp
public interface IScene
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}
```

And the scene manager:
```csharp
public class SceneManager
{
    private Stack<IScene> _scenes;

    public SceneManager()
    {
        _scenes = new Stack<IScene>();
    }

    public void PushScene(IScene scene)
    {
        _scenes.Push(scene);
    }

    public void PopScene()
    {
        if(_scenes.Count>0)
        {
            _scenes.Pop()
        }
    }

    public void Update(GameTime gameTime)
    {
        if(_scenes.Count>0)
        {
            _scenes.Peek().Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch,GameTime gameTime)
    {
        if(_scenes.Count>0)
        {
            _scenes.Peek().Draw(spriteBatch,gameTime);
        }
    }
}
```
### Example usage
Next an example usage- this assumes you have a `TitleScene` class that implements the `IScene` interface:

In `GameRoot` (the class formerly known as `Game1`):
```csharp
    // let's make this a static so you can easily access it from other points in your game.
    public static SceneManager SceneManager;

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        SceneManager=new SceneManager();

        // Assuming all content has been loaded and your title screen is ready to go!
        TitleScene myTitleScene = new TitleScene();
        SceneManager.Push(TitleScene);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        //Update the active scene:
        SceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);    

        //Draw the active scene:
        SceneManager(spriteBatch, gameTime);

        base.Draw(gameTime);
    }
```
Inside the TitleScene, when a player starts a game, simply push the game scene to the `GameRoot.SceneManager` and pop it when it is done! 

That's it! This way we can pushing and popping scenes and the top of the stack will be the one that's active! This class can be expanded with cleanup code- for example call a `OnPop()` method right before the scene is popped. Or a `OnWakeUp()` method that is called if a lower scene in the stack becomes the new top scene.

## Conditional Compilation Symbols
Another thing that I often use are the conditional symbols. In Visual Studio you can find these under the properties in your project, under *Build*. The most common one is the `DEBUG` variable. Below is an example usage in the GameRoot initialize:

```csharp
        protected override void Initialize()
        {

#if DEBUG
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
#else
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
#endif
            _graphics.ApplyChanges();

            ///...
            base.Initialize();
        }
```
This example will run the game windowed in debug mode, and fullscreen- default resolution in release mode. Another example would be only showing a frames-per-second counter in debug mode.
