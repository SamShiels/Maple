using System;
using System.Collections.Generic;
using System.Text;
using Firefly.World.Lighting;
using Silk.NET.OpenGL;

namespace Firefly.Core.Lighting
{
  internal abstract class LightBufferHandler
  {
    private GL GLContext;

    private uint blockIndex;
    private uint bufferHandle;

    private uint totalByteAllocation;

    /// <summary>
    /// Create a new instance of LightBufferHandler
    /// </summary>
    /// <param name="blockIndex">The block index to bind to.</param>
    /// <param name="totalByteAllocation">The total amount of bytes allocated for this light buffer.</param>
    public LightBufferHandler(GL GLContext, uint blockIndex)
    {
      this.GLContext = GLContext;
      this.blockIndex = blockIndex;
    }

    protected void AllocateMemory(uint totalByteAllocation)
    {
      bufferHandle = GLContext.GenBuffer();
      GLContext.BindBuffer(GLEnum.UniformBuffer, bufferHandle);
      GLContext.BufferData(GLEnum.UniformBuffer, totalByteAllocation, IntPtr.Zero, GLEnum.DynamicDraw);
      GLContext.BindBufferBase(GLEnum.UniformBuffer, blockIndex, bufferHandle);
      GLContext.BindBuffer(GLEnum.UniformBuffer, 0);
      this.totalByteAllocation = totalByteAllocation;
    }

    protected void BufferData(float[] lightData)
    {
      ReadOnlySpan<float> buffer = new ReadOnlySpan<float>(lightData);
      GLContext.BindBuffer(GLEnum.UniformBuffer, bufferHandle);
      GLContext.BufferSubData(GLEnum.UniformBuffer, 0, buffer);
      GLContext.BindBuffer(GLEnum.UniformBuffer, 0);
    }

    public uint GetBlockIndex()
    {
      return blockIndex;
    }
  }
}
