using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Maple.Core.Buffer
{
  internal class VertexBufferObject<T> where T: struct
  {
    protected const int MaximumSize = 1048576;

    private float[] fixedData;
    private List<float> dynamicData;

    protected BufferUsageHint DrawType;
    protected bool FixedSize = true;
    protected int CurrentBufferSize;
    protected int BufferPositionBytes;

    protected int GLBuffer = -1;

    public VertexBufferObject(DrawType DrawType, bool FixedSize)
    {
      this.FixedSize = FixedSize;
      CurrentBufferSize = 0;
      BufferPositionBytes = 0;
      if (DrawType == DrawType.Dynamic)
      {
        this.DrawType = BufferUsageHint.DynamicDraw;
      }
      else if (DrawType == DrawType.Static)
      {
        this.DrawType = BufferUsageHint.StaticDraw;
      }

      if (FixedSize)
      {
        fixedData = new float[MaximumSize];
      } else
      {
        dynamicData = new List<float>();
      }

      InitBuffer();
    }

    /// <summary>
    /// Initialise the buffer.
    /// </summary>
    protected virtual void InitBuffer()
    {
      GLBuffer = GL.GenBuffer();
    }

    /// <summary>
    /// Bind OpenGL to this buffer.
    /// </summary>
    public virtual void Bind() {
      GL.BindBuffer(BufferTarget.ArrayBuffer, GLBuffer);
		}

    /// <summary>
    /// Push data to the buffer list.
    /// </summary>
    /// <param name="data"></param>
    public void PushData(float[] data)
    {
      if (data == null)// || data.Length == 0)
      {
        return;
      }

      if (FixedSize)
      {
        data.CopyTo(fixedData, CurrentBufferSize);
      }
      else
      {
        dynamicData.AddRange(data);
      }

      CurrentBufferSize += data.Length;
      BufferPositionBytes += data.Length * sizeof(float);
    }

    /// <summary>
    /// Upload list to the GPU.
    /// </summary>
    public virtual void BufferData()
    {
      Bind();

      if (FixedSize)
      {
        GL.BufferData(BufferTarget.ArrayBuffer, fixedData.Length, fixedData, DrawType);
      }
      else
      {
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * dynamicData.Count, dynamicData.ToArray(), DrawType);
      }
    }

    /// <summary>
    /// Get the current position of the buffer (in bytes).
    /// </summary>
    /// <returns></returns>
    public virtual int GetBufferSizeBytes()
    {
      return BufferPositionBytes;
    }

    /// <summary>
    /// Get the current position of the buffer.
    /// </summary>
    /// <returns></returns>
    public virtual int GetBufferSize()
    {
      return CurrentBufferSize;
    }

    /// <summary>
    /// Clear the buffer and reset its position back to 0.
    /// </summary>
    public virtual void Reset()
    {
      if (!FixedSize)
      {
        dynamicData = new List<float>();
      }
      CurrentBufferSize = 0;
      BufferPositionBytes = 0;
    }

    /// <summary>
    /// Destroy the buffer.
    /// </summary>
    public virtual void DestroyBuffer()
    {
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.DeleteBuffer(GLBuffer);
    }
  }
}
