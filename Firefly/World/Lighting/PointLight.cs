using Firefly.Core;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Firefly.World.Lighting
{
  public class PointLight : WorldObject
  {
    private static uint nextId = 0;
    public uint Id { get; private set; }

    public float Radius = 5.0f;
    public float Intensity = 1.0f;
    public Vector4D<float> Diffuse = new Vector4D<float>(1f, 1f, 1f, 1f);

    public PointLight()
    {
      Id = nextId;
      nextId++;
    }
  }
}
