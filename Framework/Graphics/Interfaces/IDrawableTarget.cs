namespace Foster.Framework;

/// <summary>
/// A graphical resource that can be drawn to
/// </summary>
public interface IDrawableTarget
{
	public GraphicsDevice GraphicsDevice { get; }
	public int WidthInPixels { get; }
	public int HeightInPixels { get; }
}

public static class IDrawableTargetExt
{
	public static void Clear(this IDrawableTarget target, ReadOnlySpan<Color> color, float depth, int stencil, ClearMask mask)
	{
		target.GraphicsDevice.Clear(target, color, depth, stencil, mask);
	}

	public static void Clear(this IDrawableTarget target, Color color, float depth, int stencil, ClearMask mask)
	{
		target.GraphicsDevice.Clear(target, [color], depth, stencil, mask);
	}

	public static void Clear(this IDrawableTarget target, Color color)
	{
		target.Clear(color, 0, 0, ClearMask.Color);
	}
}