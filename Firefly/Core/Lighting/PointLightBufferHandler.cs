using Firefly.World.Lighting;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Lighting
{
  internal class PointLightBufferHandler : LightBufferHandler
  {
    const int POINT_LIGHT_COUNT = 32;
    const int POINT_LIGHT_FLOAT_COUNT = 8;

    internal PointLightBufferHandler(int blockIndex) : base(blockIndex)
    {
      int memoryAllocation = POINT_LIGHT_COUNT * POINT_LIGHT_FLOAT_COUNT * 4;
      AllocateMemory(memoryAllocation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lights"></param>
    public void BufferLightData(List<PointLight> lights)
    {
      float[] lightData = ConstructLightBuffer(lights, POINT_LIGHT_FLOAT_COUNT);
      BufferData(lightData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lights"></param>
    /// <param name="pointLightFloatAmount">Amount of float values used by a single point light</param>
    /// <returns></returns>
    private float[] ConstructLightBuffer(List<PointLight> lights, int pointLightFloatAmount)
    {
      float[] lightsArray = new float[Math.Min(pointLightFloatAmount * lights.Count, POINT_LIGHT_COUNT * POINT_LIGHT_FLOAT_COUNT)];

      for (int i = 0; i < lights.Count; i++)
      {
        PointLight light = lights[i];

        int floatArrayPosition = i * pointLightFloatAmount;

        float x = light.Transform.Position.X;
        float y = light.Transform.Position.Y;
        float z = light.Transform.Position.Z;
        float radius = light.Radius;

        float r = light.Diffuse.R;
        float g = light.Diffuse.G;
        float b = light.Diffuse.B;
        float intensity = light.Intensity;

        lightsArray[floatArrayPosition] = x;
        lightsArray[floatArrayPosition + 1] = y;
        lightsArray[floatArrayPosition + 2] = z;
        lightsArray[floatArrayPosition + 3] = radius;

        lightsArray[floatArrayPosition + 4] = r;
        lightsArray[floatArrayPosition + 5] = g;
        lightsArray[floatArrayPosition + 6] = b;
        lightsArray[floatArrayPosition + 7] = intensity;
      }

      return lightsArray;
    }
  }
}
