using Silk.NET.Maths;
using Silk.NET.Windowing;
using System;

namespace SpriteExample
{
  class Program
  {
    private static IWindow window;

    static void Main(string[] args)
    {
      var options = WindowOptions.Default;
      options.Size = new Vector2D<int>(800, 600);
      options.Title = "LearnOpenGL with Silk.NET";

      window = Window.Create(options);

      //Assign events.
      window.Load += OnLoad;
      window.Update += OnUpdate;
      window.Render += OnRender;

      //Run the window.
      window.Run();
    }

    private static void OnLoad()
    {
      //Set-up input context.
    }

    private static void OnRender(double obj)
    {
      //Here all rendering should be done.
    }

    private static void OnUpdate(double obj)
    {
      //Here all updates to the program should be done.
    }
  }
}
