using Firefly.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Lighting
{
  public class PointLight : WorldObject
  {
    public float Radius = 5.0f;
    public Color4 Diffuse = Color4.White;
    public Color4 Specular = Color4.White;
  }
}
