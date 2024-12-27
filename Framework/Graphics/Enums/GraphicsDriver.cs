namespace Foster.Framework;

public enum GraphicsDriver
{
	None,
	Private,
	Vulkan,
	D3D12,
	Metal,
	OpenGL
}

public static class GraphicsDriverExt
{
	public static string GetShaderExtension(this GraphicsDriver driver) => driver switch
	{
		GraphicsDriver.None => string.Empty,
		GraphicsDriver.Private => "spv",
		GraphicsDriver.Vulkan => "spv",
		GraphicsDriver.D3D12 => "dxil",
		GraphicsDriver.Metal => "msl",
		GraphicsDriver.OpenGL => "glsl",
		_ => throw new NotImplementedException(),
	};
}