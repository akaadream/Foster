# Batcher

As I sad in the `Getting started` article, the batcher help you to draw the content of your game, like images, texts, and so on.

> Sometimes, it's very helpful to draw some shapes like circle, rectangles, and more during the debug of your game. The batcher of Foster contains a lot of these shapes. We'll look at this in more detail later.

## Push, Pop

With this framework, you don't have to do the Begin/End things that you must respect in the XNA frameworks.
Instead, Foster works with a system of Push and Pop functions. This means that you can define some settings with `PushXXX(...)` and revert the settings with `PopXXX()`.

As an example, let's say we have a moving camera, and we want to make the rendering of the game at the camera's position. You will do something like this:
```csharp
Batcher.PushMatrix(-(Point2)Camera.Position);

// Rendering of the game here


Batcher.PopMatrix();
```

## Debug rendering

As I said earlier, you can draw debug shapes with the batcher. Here is some examples:
```csharp

```