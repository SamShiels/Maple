using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class Camera
  {
    public int projectionType { get; set; }
    public double fov { get; set; }
    public int farClip { get; set; }
    public double nearClip { get; set; }
    public List<int> position { get; set; }
    public List<int> rotation { get; set; }
    public string skybox { get; set; }
  }
}
