using Firefly.Rendering;
using Firefly.World;
using Firefly.World.Scene;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Test
{
  public class RenderingBase
  {
    private Renderer renderer;
    public Scene scene;
    public Camera camera;

    private GameWindow window;

    public RenderingBase()
    {
      //WglBindingsContext context = new WglBindingsContext();
      //GL.LoadBindings(context);
      window = new GameWindow(GameWindowSettings.Default, new NativeWindowSettings()
      {
        APIVersion = new Version(4, 6),
        Size = new Vector2i()
        {
          X = 100,
          Y = 100
        },
        Title = "test",
        WindowState = WindowState.Normal,
      });
      renderer = new Renderer(100, 100);
      scene = new Scene();
      camera = new Camera();
      camera.Transform.Position = new Vector3(0.0f, 0.0f, -5f);
      scene.AssignCamera(camera);
    }

    public void Render()
    {
      renderer.Render(scene);
    }

    public void Close()
    {
      window.Close();
    }
  }
}
