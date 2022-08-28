﻿using System;
using System.Collections.Generic;
using System.Text;
using Firefly.World.Lighting;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Firefly.Core
{

  internal class LightBufferHandler
  {
    private int bufferHandler;
    private int blockIndex;

    private Dictionary<uint, float[]> lightList;

    /// <summary>
    /// Create a new instance of LightBufferHandler
    /// </summary>
    /// <param name="blockIndex">The block index to bind to.</param>
    public LightBufferHandler(int blockIndex)
    {
      string light = @"
        vec4 ambientLight; // 0, 1, 2, 3

        struct Light {
          vec3 position; // 0, 1, 2
          float radius; // 3
          vec4 color // 4, 5, 6, 7;
        }
      ";

      this.blockIndex = blockIndex;

      bufferHandler = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.UniformBuffer, bufferHandler);
      GL.BufferData(BufferTarget.UniformBuffer, 32, IntPtr.Zero, BufferUsageHint.StaticDraw);
      GL.BindBufferBase(BufferRangeTarget.UniformBuffer, blockIndex, bufferHandler);
      GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void BufferLightData(List<PointLight> lights, Color4 ambientLight)
    {
      int ambientLightFloatAmount = 4;
      int pointLightFloatAmount = 4;

      float[] lightData = ConstructLightBuffer(lights, ambientLight, ambientLightFloatAmount, pointLightFloatAmount);
      int totalFloatCountAllocation = lightData.Length * pointLightFloatAmount;// + ambientLightFloatAmount;

      GL.BindBuffer(BufferTarget.UniformBuffer, bufferHandler);
      GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, totalFloatCountAllocation, lightData);
      GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public int GetBlockIndex()
    {
      return blockIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lights"></param>
    /// <param name="pointLightFloatAmount">Amount of float values used by a single point light</param>
    /// <returns></returns>
    private float[] ConstructLightBuffer(List<PointLight> lights, Color4 ambientLight, int ambientLightFloatAmount, int pointLightFloatAmount)
    {
      float[] lightsArray = new float[lights.Count * pointLightFloatAmount];// + ambientLightFloatAmount];

      for (int i = 0; i < lights.Count; i++)
      {
        PointLight light = lights[i];

        int floatArrayPosition = i * pointLightFloatAmount;

        float x = light.Transform.Position.X;
        float y = light.Transform.Position.Y;
        float z = light.Transform.Position.Z;
        float radius = light.Radius;

        float r = light.Diffuse.R;
        float b = light.Diffuse.G;
        float g = light.Diffuse.B;
        float a = light.Diffuse.A;

        lightsArray[floatArrayPosition    ] = x;
        lightsArray[floatArrayPosition + 1] = y;
        lightsArray[floatArrayPosition + 2] = z;
        lightsArray[floatArrayPosition + 3] = radius;
        //lightsArray[floatArrayPosition + 4] = r;
        //lightsArray[floatArrayPosition + 5] = g;
        //lightsArray[floatArrayPosition + 6] = b;
        //lightsArray[floatArrayPosition + 7] = a;
      }

      int ambientLightPosition = lights.Count * pointLightFloatAmount;

      float ra = ambientLight.R;
      float ba = ambientLight.G;
      float ga = ambientLight.B;
      float aa = ambientLight.A;

      //lightsArray[ambientLightPosition    ] = ra;
      //lightsArray[ambientLightPosition + 1] = ga;
      //lightsArray[ambientLightPosition + 2] = ba;
      //lightsArray[ambientLightPosition + 3] = aa;

      return lightsArray;
    }
  }
}
