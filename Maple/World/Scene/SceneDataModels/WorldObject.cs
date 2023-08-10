using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Maple.World.Scene.SceneDataModels
{
  internal class WorldObject
  {
    public string name { get; set; }
    public string type { get; set; }
    public List<float> position { get; set; }
    public List<float> rotation { get; set; }
    public List<float> localScale { get; set; }
    public List<WorldObject> children { get; set; }
    public JsonElement properties { get; set; }
  }
}
