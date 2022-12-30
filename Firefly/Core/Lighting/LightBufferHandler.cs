﻿using System;
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

    /// <summary>
    /// Create a new instance of LightBufferHandler
    /// </summary>
    /// <param name="blockIndex">The block index to bind to.</param>
    /// <param name="totalByteAllocation">The total amount of bytes allocated for this light buffer.</param>
    internal LightBufferHandler(int blockIndex)
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
    }

    protected void BufferData(float[] lightData)
    {
      GL.BindBuffer(BufferTarget.UniformBuffer, bufferHandle);
      GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, lightData.Length * 4, lightData);
      GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    internal int GetBlockIndex()
    {
      return blockIndex;
    }
  }
}