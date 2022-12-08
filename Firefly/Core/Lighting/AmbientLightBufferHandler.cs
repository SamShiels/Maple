using Firefly.World.Lighting;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Lighting
{
  internal class AmbientLightBufferHandler : LightBufferHandler
  {
    const int AMBIENT_LIGHT_FLOAT_COUNT = 4;
    const int DIRECTIONAL_LIGHT_FLOAT_COUNT = 4;

    internal AmbientLightBufferHandler(GL GLContext, uint blockIndex) : base(GLContext, blockIndex)
    {
      uint memoryAllocation = AMBIENT_LIGHT_FLOAT_COUNT * 4;
      AllocateMemory(memoryAllocation);
    }

    public void BufferLightData(Color ambientLight)
    {
      float[] lightsArray = new float[AMBIENT_LIGHT_FLOAT_COUNT];
      lightsArray[0] = ambientLight.R;
      lightsArray[1] = ambientLight.G;
      lightsArray[2] = ambientLight.B;
      lightsArray[3] = 1f;

      BufferData(lightsArray);
    }
  }
}
