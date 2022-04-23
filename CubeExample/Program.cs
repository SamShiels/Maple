using System;

namespace SpriteExample
{
  class Program
  {
    static void Main(string[] args)
    {
      using (Example example = new Example(1920, 1080, "Game"))
      {
        example.Run();
      }
    }
  }
}
