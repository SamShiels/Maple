using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class WorldObject<T>
  {
    public string name { get; set; }
    public string type { get; set; }
    public List<int> position { get; set; }
    public List<int> rotation { get; set; }
    public List<int> localScale { get; set; }
    public List<WorldObject> children { get; set; }
    public T properties { get; set; }
  }
}
