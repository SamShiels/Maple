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
      renderer = new Renderer(1280, 720);
      renderer.ProjectionType = ProjectionType.Perspective;
      renderer.VerticalFieldOfView = 90;
      renderer.ClearColor = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
    }

    internal void Render(Scene scene)
    {
      renderer.RenderRaw(scene);
    }

    internal void Resize(int width, int height)
    {
      renderer.UpdateGLViewport(width, height);
      renderer.ResolutionWidth = width;
      renderer.ResolutionHeight = height;
    }
  }
}
