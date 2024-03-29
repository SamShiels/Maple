﻿using Maple.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.World.Lighting
{
  public class PointLight : WorldObject
  {
    private static uint nextId = 0;
    public uint Id { get; private set; }

    public float Radius = 5.0f;
    public float Intensity = 1.0f;
    public Color4 Diffuse = Color4.White;

    public PointLight()
    {
      Id = nextId;
      nextId++;
    }
  }
}
