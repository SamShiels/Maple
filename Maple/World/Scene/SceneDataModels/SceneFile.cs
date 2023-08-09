using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.World.Scene.SceneDataModels
{
  internal class SceneFile
  {
    public Camera camera { get; set; }
    public List<WorldObject> worldObjects { get; set; }
    public Assets assets { get; set; }
  }
}
