using Firefly.Core;
using Firefly.Texturing;
using Firefly.Utilities;
using OpenTK.Mathematics;

namespace Firefly.World
{
  public class Camera : WorldObject
  {
    private static uint nextId = 0;
    public uint Id { get; private set; }
    public uint DirtyId { get; private set; }

    public override string TYPE { get; protected set; } = "Camera";

    public ProjectionType ProjectionType = ProjectionType.Perspective;

    private float fieldOfView = (float)System.Math.PI / 2.5f;

    /// <summary>
    /// The vertical field of view in rads. Perspective projection only.
    /// </summary>
    public float FieldOfView
    {
      get
      {
        return fieldOfView;
      }
      set
      {
        fieldOfView = value;
        IncrementDirtyId();
      }
    }

    private float orthographicSize = 18f;

    /// <summary>
    /// Defines the amount of world units from the center of the screen to the top. Orthographic projection only.
    /// </summary>
    public float OrthographicSize
    {
      get
      {
        return orthographicSize;
      }
      set
      {
        orthographicSize = value;
        IncrementDirtyId();
      }
    }

    public Matrix4 projectionMatrix;

    public Texture RenderTexture { get; private set; }

    public Camera(Texture RenderTexture = null)
    {
      Id = nextId;
      nextId++;

      this.RenderTexture = RenderTexture;
    }

    private void IncrementDirtyId()
    {
      DirtyId++;
    }
  }
}
