using Firefly.World.Lighting;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Lighting
{
  internal class DirectionalLightBufferHandler : LightBufferHandler
  {
    const int DIRECTIONAL_LIGHT_COUNT = 4;
    const int DIRECTIONAL_LIGHT_FLOAT_COUNT = 8;

    internal DirectionalLightBufferHandler(int blockIndex) : base(blockIndex)
    {
      int memoryAllocation = DIRECTIONAL_LIGHT_COUNT * DIRECTIONAL_LIGHT_FLOAT_COUNT * 4;
      AllocateMemory(memoryAllocation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lights"></param>
    public void BufferLightData(List<DirectionalLight> lights)
    {
      float[] lightData = ConstructLightBuffer(lights, DIRECTIONAL_LIGHT_FLOAT_COUNT);
      BufferData(lightData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lights"></param>
    /// <param name="directionalLightFloatAmount">Amount of float values used by a single point light</param>
    /// <returns></returns>
    private float[] ConstructLightBuffer(List<DirectionalLight> lights, int directionalLightFloatAmount)
    {
      float[] lightsArray = new float[Math.Min(directionalLightFloatAmount * lights.Count, DIRECTIONAL_LIGHT_COUNT * DIRECTIONAL_LIGHT_FLOAT_COUNT)];

      for (int i = 0; i < lights.Count; i++)
      {
        DirectionalLight light = lights[i];

        int floatArrayPosition = i * directionalLightFloatAmount;

        Vector3 forward = light.Transform.Forward;
        float x = forward.X;
        float y = forward.Y;
        float z = forward.Z;

        float r = light.Diffuse.R;
        float g = light.Diffuse.G;
        float b = light.Diffuse.B;
        float intensity = light.Intensity;

        lightsArray[floatArrayPosition] = x;
        lightsArray[floatArrayPosition + 1] = y;
        lightsArray[floatArrayPosition + 2] = z;
        lightsArray[floatArrayPosition + 3] = intensity;

        lightsArray[floatArrayPosition + 4] = r;
        lightsArray[floatArrayPosition + 5] = g;
        lightsArray[floatArrayPosition + 6] = b;
        lightsArray[floatArrayPosition + 7] = 0f;
      }

      return lightsArray;
    }
  }
}
