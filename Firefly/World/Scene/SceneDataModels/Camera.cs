using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.SceneDataModels
{
  internal class Camera
  {
    public int projectionType { get; set; }
    public float fov { get; set; }
    public float farClip { get; set; }
    public float nearClip { get; set; }
    public List<float> position { get; set; }
    public List<float> rotation { get; set; }
    public string skybox { get; set; }
  }
}
