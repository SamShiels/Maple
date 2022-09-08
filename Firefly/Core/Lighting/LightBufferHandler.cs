using System;
using System.Collections.Generic;
using System.Text;
using Firefly.World.Lighting;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Firefly.Core.Lighting
{
  internal abstract class LightBufferHandler
  {
    private int blockIndex;
    private int bufferHandle;

    private int totalByteAllocation;

    private bool initialized = false;

    /// <summary>
    /// Create a new instance of LightBufferHandler
    /// </summary>
    /// <param name="blockIndex">The block index to bind to.</param>
    /// <param name="totalByteAllocation">The total amount of bytes allocated for this light buffer.</param>
    public LightBufferHandler(int blockIndex)
    {
      this.blockIndex = blockIndex;
    }

    protected void AllocateMemory(int totalByteAllocation)
    {
      bufferHandle = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.UniformBuffer, bufferHandle);
      GL.BufferData(BufferTarget.UniformBuffer, totalByteAllocation, IntPtr.Zero, BufferUsageHint.DynamicDraw);
      GL.BindBufferBase(BufferRangeTarget.UniformBuffer, blockIndex, bufferHandle);
      GL.BindBuffer(BufferTarget.UniformBuffer, 0);
      this.totalByteAllocation = totalByteAllocation;

      initialized = true;
    }

    protected void BufferData(float[] lightData)
    {
      if (initialized)
      {
        GL.BindBuffer(BufferTarget.UniformBuffer, bufferHandle);
        GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, lightData.Length * 4, lightData);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
      }
    }

    public int GetBlockIndex()
    {
      return blockIndex;
    }
  }
}
