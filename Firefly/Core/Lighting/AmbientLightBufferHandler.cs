using Firefly.World.Lighting;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Lighting
{
  internal class AmbientLightBufferHandler : LightBufferHandler
  {
    const int AMBIENT_LIGHT_FLOAT_COUNT = 4;

    internal AmbientLightBufferHandler(int blockIndex) : base(blockIndex)
    {
      int memoryAllocation = AMBIENT_LIGHT_FLOAT_COUNT * 4;
      AllocateMemory(memoryAllocation);
    }

    public void BufferLightData(Color4 ambientLight)
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
