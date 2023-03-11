using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class WorldObject
  {
    public string name { get; set; }
    public string type { get; set; }
    public string modelName { get; set; }
    public List<string> textures { get; set; }
    public string material { get; set; }
    public List<int> position { get; set; }
    public List<int> rotation { get; set; }
    public List<int> localScale { get; set; }
    public List<object> children { get; set; }
  }
}
