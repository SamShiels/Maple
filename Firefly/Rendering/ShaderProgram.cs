
namespace Firefly.Rendering
{
  public class ShaderProgram
  {
    private static int NextId = 0;

    /// <summary>
    /// Global shader Id.
    /// </summary>
	  public int Id { get; private set; }
    /// <summary>
    /// The source code of the vertex shader.
    /// </summary>
    public string VertexShaderSource { get; private set; }
    /// <summary>
    /// The source code of the fragment shader.
    /// </summary>
    public string FragmentShaderSource { get; private set; }

    public ShaderProgram(string VertexShaderSource, string FragmentShaderSource)
    {
      this.VertexShaderSource = VertexShaderSource;
      this.FragmentShaderSource = FragmentShaderSource;

      Id = NextId;
      NextId++;
    }

    public void Destroy()
    {
      VertexShaderSource = null;
      FragmentShaderSource = null;
    }
  }
}
