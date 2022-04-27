using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace Firefly.Core.Buffer
{
  public class IndexBufferObject : VertexBufferObject<int>
  {
    private int[] fixedData;
    private List<int> dynamicData;

    public IndexBufferObject(DrawType DrawType, bool FixedSize) : base(DrawType, FixedSize) {


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
    public override void Bind() {
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLBuffer);
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
    public override void BufferData()
    {
      Bind();
      if (FixedSize)
      {
        GL.BufferData(BufferTarget.ElementArrayBuffer, fixedData.Length, fixedData, DrawType);
      }
      else
      {
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * dynamicData.Count, dynamicData.ToArray(), DrawType);
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
