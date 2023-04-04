using Silk.NET.OpenGL;

namespace Firefly.Core
{
  /// <summary>
  /// A decorator for any internal component that requires GL.
  /// </summary>
  internal abstract class RendererComponent
  {
    protected GL GL;

    internal RendererComponent(GL GLContext)
    {
      GL = GLContext;
    }
  }
}
