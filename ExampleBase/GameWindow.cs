using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;

namespace ExampleBase
{
  public class GameWindow
  {
    protected Controller game;
    protected IWindow window;

    public GameWindow(int width, int height, string title) {

      var options = WindowOptions.Default;
      options.Size = new Vector2D<int>(width, height);
      options.Title = title;

      window = Window.Create(options);

      window.Load += OnLoad;
      window.Update += OnUpdate;
      window.Render += OnRender;
    }

    protected virtual void OnLoad()
    {
      game = new Controller(window.Size.X, window.Size.Y);
      game.OnLoad();
    }

    protected virtual void OnUpdate(double obj)
    {
      game.OnUpdate();
    }

    protected virtual void OnRender(double obj)
    {
      game.OnRender();
    }
  }
}
