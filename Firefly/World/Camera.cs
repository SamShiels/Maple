using Firefly.Core;
using Firefly.Texturing;
using Firefly.Utilities;

namespace Firefly.World
{
  public class Camera : WorldObject
  {
    public override string TYPE { get; protected set; } = "Camera";

    public ProjectionType ProjectionType = ProjectionType.Perspective;

    public float VerticalFov = (float)System.Math.PI / 2.5f;
    public float OrthographicSize = 18f;

    public Texture RenderTexture { get; private set; }

    public Camera(Texture RenderTexture = null)
    {
      this.RenderTexture = RenderTexture;
    }
  }
}
