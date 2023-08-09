using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.World.Scene.SceneDataModels
{
  internal class Material
  {
    public string name { get; set; }
    public string shaderName { get; set; }
    public List<MaterialUniform> uniforms { get; set; }
  }
}
