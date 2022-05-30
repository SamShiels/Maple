using System;
using System.Collections.Generic;
using System.Text;
using Firefly;
using Firefly.Core;
using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.Utilities;
using Firefly.World;
using Firefly.World.Mesh;
using OpenTK.Mathematics;

namespace SceneEditor.Editor
{
  internal class RendererManager
  {
    private Renderer renderer;

    internal RendererManager(int frameBufferHandle)
    {
      Material material = new Material(ShaderLibrary.Instance.GetShader("canvas"));
      renderer = new Renderer(1280, 720, 1280, 720, material, 0, 0);
      renderer.ProjectionType = ProjectionType.Perspective;
      renderer.VerticalFieldOfView = 90;
      renderer.UpdateBackgroundColor(new Color4(0.5f, 0.5f, 0.5f, 1.0f));
    }

    internal void Render(Scene scene)
    {
      renderer.RenderRaw(scene);
    }

    internal void Resize(int width, int height)
    {
      renderer.UpdateWindowDimensions(width, height);
      renderer.UpdateResolution(width, height);
    }
  }
}
