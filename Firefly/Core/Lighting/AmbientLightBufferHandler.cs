using Silk.NET.Maths;

namespace Firefly.Core.Lighting
{
  internal class AmbientLightBufferHandler : LightBufferHandler
  {
    const int AMBIENT_LIGHT_FLOAT_COUNT = 4;
    const int DIRECTIONAL_LIGHT_FLOAT_COUNT = 4;

    internal AmbientLightBufferHandler(int blockIndex) : base(blockIndex)
    {
      int memoryAllocation = (AMBIENT_LIGHT_FLOAT_COUNT) * 4;
      AllocateMemory(memoryAllocation);
    }

    public void BufferLightData(Vector4D<float> ambientLight)
    {
      float[] lightsArray = new float[AMBIENT_LIGHT_FLOAT_COUNT];
      lightsArray[0] = ambientLight.X;
      lightsArray[1] = ambientLight.Y;
      lightsArray[2] = ambientLight.Z;
      lightsArray[3] = 1f;

      BufferData(lightsArray);
    }
  }
}
