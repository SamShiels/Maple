using Firefly.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Lighting
{
  public class DirectionalLight : WorldObject
  {
    private static uint nextId = 0;
    public uint Id { get; private set; }

    public float Intensity = 1.0f;
    public Color4 Diffuse = Color4.White;
    public bool CastShadows = true;

    public DirectionalLight()
    {
      Id = nextId;
      nextId++;
    }
  }
}
