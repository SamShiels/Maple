using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class Cubemap
  {
    public string name { get; set; }
    public string right { get; set; }
    public string left { get; set; }
    public string top { get; set; }
    public string bottom { get; set; }
    public string front { get; set; }
    public string back { get; set; }
  }
}
