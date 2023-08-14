using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;

namespace ExampleBase
{
  public class Window : GameWindow
  {
    protected Controller game;
    private Vector2i resolution;

    public Window(int width, int height, string title) : base(
      GameWindowSettings.Default,
      new NativeWindowSettings()
      {
        APIVersion = new Version(3, 3),
        Size = new Vector2i()
        {
          X = width,
          Y = height
        },
        Title = title,
        WindowState = WindowState.Normal,
      })
    {
      VSync = VSyncMode.On;
      resolution = new Vector2i()
      {
        X = width,
        Y = height
      };
    }

    protected override void OnLoad()
    {
      game = new Controller(Size.X, Size.Y);
      game.OnLoad();
      base.OnLoad();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      game.OnUpdate();
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      game.OnRender();
      Context.SwapBuffers();
      base.OnRenderFrame(args);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
    }

    protected override void OnUnload()
    {
      base.OnUnload();
    }
  }
}
