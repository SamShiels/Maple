using Renderer.Core;
using Renderer.Textures;
using Renderer.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renderer.Cameras
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
