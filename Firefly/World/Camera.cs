using Firefly.Core;
using Firefly.Texturing;
using Firefly.Utilities;

namespace Firefly.World
{
  public class Camera : WorldObject
  {
    public override string TYPE { get; protected set; } = "Camera";

    public ProjectionType ProjectionType = ProjectionType.Perspective;

    private float verticalFov = (float)System.Math.PI / 2.5f;

    public float VerticalFov
    {
      get
      {
        return verticalFov;
      }
      set
      {
        verticalFov = value;
      }
    }

    public float orthographicSize = 18f;

    public float OrthographicSize
    {
      get
      {
        return orthographicSize;
      }
      set
      {
        orthographicSize = value;
      }
    }

    public Texture RenderTexture { get; private set; }

    public Camera(Texture RenderTexture = null)
    {
      this.RenderTexture = RenderTexture;
    }
  }
}
