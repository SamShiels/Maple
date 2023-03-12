using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class MeshObjectProperties
  {
    public string modelName { get; set; }
    public List<string> textures { get; set; }
    public string material { get; set; }
  }
}
