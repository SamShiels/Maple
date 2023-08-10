using Maple.Rendering;

namespace Maple.Rendering
{
  public class Material
  {
    private static int NextId = 0;

    /// <summary>
    /// Global material Id.
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// Shader program.
    /// </summary>
    public Shader Shader { get; private set; }
    /// <summary>
    /// Uniform data and definitions.
    /// </summary>
    public Uniform[] Uniforms { get; private set; }
    /// <summary>
    /// Raster technique.
    /// </summary>
    public PrimitiveType PrimitiveType { get; set; }
    /// <summary>
    /// Raster technique.
    /// </summary>
    public DepthFunction DepthFunction { get; set; }

    public Material(Shader Shader, Uniform[] Uniforms)
    {
      this.Shader = Shader;
      this.Uniforms = Uniforms;
      this.PrimitiveType = PrimitiveType.Triangles;
      this.DepthFunction = DepthFunction.Lequal;

      Id = NextId;
      NextId++;
    }

    public Material(Shader Shader)
    {
      this.Shader = Shader;
      this.Uniforms = null;
      this.PrimitiveType = PrimitiveType.Triangles;

      Id = NextId;
      NextId++;
    }

    public void Destroy()
    {
      Shader.Destroy();
      Shader = null;
      Uniforms = null;
    }
  }
}
