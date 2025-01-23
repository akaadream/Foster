# Getting Started

This documentation will provides you all the basics you may know before starting using Foster.

## Installation

Foster is available by 2 ways:
- You can as a [NuGeT package](https://www.nuget.org/packages/FosterFramework):
> Note that current NuGeT package is not up to date with the latest changes of the framework at time of writing. This documentation is covering the current `app-refactor` branch which will be soon merged onto the main branch.

```
dotnet add package FosterFramework --version 0.2.0
```
- Or you can clone the repository and add the project inside yours and link it as a reference.

## Your first game!

To create your first game using Foster, you create an App. This is a base class of the Foster's Framework that you must inherit.
Inside it you'll have all you need to create the life-cycle or your game:
```csharp
public class MyGame : App
{
    public MyGame(): base("Name of the game", 1600, 900)
    {

    }

    protected override void Startup()
    {

    }

    protected override void Shutdown()
    {

    }

    protected override void Update()
    {

    }

    protected override void Render()
    {

    }
}
```
The `App` class constructor takes 3 arguments, the title of your game (written as the title of the game's window), and 2 integers which are respectively the width and the height of the window.

Run this code and... nothing happened, or maybe you got an error.
Well you created the class for your game but you never instanciated it.
To do this, just create a `Program.cs` if it does not yet exists and put this code inside it:
```csharp
using var app = new MyGame();
app.Run();
```
Annnd that's it! Now you can run your project and you'll see a window with the name and the size you gave and a nice black background!

Now, let's say you want to change the color of the background. You will need a batcher.

> The batcher is the class that will "create" all the drawings you want to display your game content (images, texts, rectangles, ...) by communicating with your graphics card.

Let's create the batcher:
```diff
public class MyGame : App
{
+    public Batcher Batcher { get; set; }

    public MyGame(): base("My game title" 1600, 900)
    {
+        Batcher = new();
    }

    // ...
    protected override Render()
    {
+        // Clear the window with a nice purple
+        Window.Clear(0x897897);

+        // Rendering on the window
+        Batcher.Render(Window);
+        Batcher.Clear();
    }
}
```
You can finally run your project and see that beautiful window with the purple background.

In the next section, we'll take a closer look at the batcher and discover its functions.