using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace Firefly.Core.Buffer
{
  internal class IndexBufferObject : VertexBufferObject<int>
  {
    private int[] fixedData;
    private List<int> dynamicData;

    public IndexBufferObject(GL GLContext, DrawType DrawType, bool FixedSize) : base(GLContext, DrawType, FixedSize) {


      if (FixedSize)
      {
        fixedData = new int[MaximumSize];
      }
      else
      {
        dynamicData = new List<int>();
      }
    }

    /// <summary>
    /// Bind OpenGL to this buffer.
    /// </summary>
    public override void Bind()
    {
      GL.BindBuffer(GLEnum.ElementArrayBuffer, GLBuffer);
    }

    /// <summary>
    /// Push data to the buffer list.
    /// </summary>
    /// <param name="data"></param>
    public void PushData(int[] data)
    {
      if (FixedSize)
      {
        data.CopyTo(fixedData, CurrentBufferSize);
      }
      else
      {
        dynamicData.AddRange(data);
      }

      CurrentBufferSize += data.Length;
      BufferPositionBytes += data.Length * sizeof(int);
    }

    /// <summary>
    /// Upload list to the GPU.
    /// </summary>
    public unsafe override void BufferData()
    {
      Bind();
      if (FixedSize)
      {
        fixed (void* fixedData = this.fixedData)
        {
          GL.BufferData(GLEnum.ElementArrayBuffer, (nuint)this.fixedData.Length, fixedData, DrawType);
        }
      }
      else
      {
        fixed (void* dynamicData = this.dynamicData.ToArray())
        {
          GL.BufferData(GLEnum.ArrayBuffer, sizeof(int) * (nuint)this.dynamicData.Count, dynamicData, DrawType);
        }
      }
    }

    public override void Reset()
    {
      if (!FixedSize)
      {
        dynamicData = new List<int>();
      }
      CurrentBufferSize = 0;
      BufferPositionBytes = 0;
    }
  }
}
