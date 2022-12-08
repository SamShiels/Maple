using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;

namespace ExampleBase
{
  public class ExampleWindow
  {
    protected Controller game;

    private IWindow window;

    public ExampleWindow(int width, int height, string title)
    {
      var options = WindowOptions.Default;
      options.Size = new Vector2D<int>(width, height);
      options.Title = "LearnOpenGL with Silk.NET";

      window = Window.Create(options);

      //Assign events.
      window.Load += OnLoad;
      window.Update += OnUpdate;
      window.Render += OnRender;

      //Run the window.
      window.Run();
    }

    protected void OnLoad()
    {
      game = new Controller(window.BorderSize.Size.X, window.BorderSize.Size.Y);
      game.OnLoad();
    }

    protected void OnUpdate(double obj)
    {
      game.OnUpdate();
    }

    protected void OnRender(double obj)
    {
      game.OnRender();
    }
  }
}
