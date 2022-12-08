using Firefly.Core;
using Firefly.Texturing;
using Firefly.Utilities;

namespace Firefly.World
{
  public class Camera : WorldObject
  {
    private static uint nextId = 0;
    public uint Id { get; private set; }
    public uint DirtyId { get; private set; }

    public override string TYPE { get; protected set; } = "Camera";

    public ProjectionType ProjectionType = ProjectionType.Perspective;

    private float nearClipPlane = 0.01f;
    /// <summary>
    /// The distance of the near clip plane from the camera.
    /// </summary>
    public float NearClipPlane
    {
      get
      {
        return nearClipPlane;
      }
      set
      {
        NearClipPlane = value;
        IncrementDirtyId();
      }
    }

    private float farClipPlane = 1000f;
    /// <summary>
    /// The distance of the far clip plane from the camera.
    /// </summary>
    public float FarClipPlane
    {
      get
      {
        return farClipPlane;
      }
      set
      {
        FarClipPlane = value;
        IncrementDirtyId();
      }
    }

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

    public RenderTexture RenderTexture { get; set; }

    public Camera()
    {
      Id = nextId;
      nextId++;
    }

    private void IncrementDirtyId()
    {
      DirtyId++;
    }
  }
}
