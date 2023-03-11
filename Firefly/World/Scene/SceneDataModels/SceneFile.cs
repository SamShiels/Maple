using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class SceneFile
  {
    public Camera camera { get; set; }
    public List<WorldObject> worldObjects { get; set; }
    public List<Material> materials { get; set; }
    public List<Cubemap> cubemaps { get; set; }
  }
}
