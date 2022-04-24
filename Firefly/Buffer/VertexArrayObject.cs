using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Buffer
{
  public class VertexArrayObject
  {
    private VertexBufferObject<float> Positions { get; set; }
    private VertexBufferObject<float> TextureCoordinates { get; set; }
    private VertexBufferObject<float> Normals { get; set; }
    private VertexBufferObject<float> TextureUnits { get; set; }
    private IndexBufferObject Indices { get; set; }

    private int vao = -1;

    public VertexArrayObject(DrawType Usage, bool FixedSize)
    {
      vao = GL.GenVertexArray();
      Positions = new VertexBufferObject<float>(Usage, FixedSize);
      TextureCoordinates = new VertexBufferObject<float>(Usage, FixedSize);
      Normals = new VertexBufferObject<float>(Usage, FixedSize);
      TextureUnits = new VertexBufferObject<float>(Usage, FixedSize);
      Indices = new IndexBufferObject(Usage, FixedSize);
    }

    /// <summary>
    /// Push an array of positions to the buffers.
    /// </summary>
    /// <param name="positions"></param>
    public void AddPositions(float[] positions)
    {
      Positions.PushData(positions);
    }

    /// <summary>
    /// Push an array of texture coordinates to the buffers.
    /// </summary>
    /// <param name="textureCoordinates"></param>
    public void AddTextureCoordinates(float[] textureCoordinates)
    {
      TextureCoordinates.PushData(textureCoordinates);
    }

    /// <summary>
    /// Push an array of normals to the buffers.
    /// </summary>
    /// <param name="normals"></param>
    public void AddNormals(float[] normals)
    {
      Normals.PushData(normals);
    }

    /// <summary>
    /// Push an array of texture units to the buffers.
    /// </summary>
    /// <param name="textureUnits"></param>
    public void AddTextureUnits(float[] textureUnits)
    {
      TextureUnits.PushData(textureUnits);
    }

    /// <summary>
    /// Push an array of indices to the buffers.
    /// </summary>
    /// <param name="indices"></param>
    public void AddIndices(int[] indices)
    {
      Indices.PushData(indices);
    }

    /// <summary>
    /// Upload vertex data to the GPU.
    /// </summary>
    public void UploadData()
    {
      Bind();
      Positions.BufferData();
      if (TextureCoordinates.GetBufferSizeBytes() >= 0)
      {
        TextureCoordinates.BufferData();
      }
      if (Normals.GetBufferSizeBytes() >= 0)
      {
        Normals.BufferData();
      }
      if (TextureUnits.GetBufferSizeBytes() >= 0)
      {
        TextureUnits.BufferData();
      }
      if (Indices.GetBufferSizeBytes() >= 0)
      {
        Indices.BufferData();
      }
    }

    /// <summary>
    /// Bind the buffers and enable attribute pointers.
    /// </summary>
    public void EnablePointers()
    {
      Bind();

      // bind the positions buffer and enable the positions attribute pointer
      Positions.Bind();
      EnableAttribute(0, 3, VertexAttribPointerType.Float, 3 * sizeof(float));

      // bind the texture coordinates buffer and enable the attribute pointer
      if (TextureCoordinates.GetBufferSizeBytes() >= 0)
      {
        TextureCoordinates.Bind();
        EnableAttribute(1, 2, VertexAttribPointerType.Float, 2 * sizeof(float));
      }

      // bind the normals buffer and enable the attribute pointer
      if (Normals.GetBufferSizeBytes() >= 0)
      {
        Normals.Bind();
        EnableAttribute(2, 3, VertexAttribPointerType.Float, 3 * sizeof(float));
      }

      // bind the texture units buffer and enable the attribute pointer
      if (TextureUnits.GetBufferSizeBytes() >= 0)
      {
        TextureUnits.Bind();
        EnableAttribute(3, 1, VertexAttribPointerType.Float, 1 * sizeof(float));
      }


      if (Indices.GetBufferSizeBytes() >= 0)
      {
        // we are using elements because it's better
        Indices.Bind();
      }
    }

    /// <summary>
    /// Bind to this vao.
    /// </summary>
    public void Bind()
    {
      GL.BindVertexArray(vao);
    }

    /// <summary>
    /// Unbind this vao.
    /// </summary>
    public void Unbind()
    {
      GL.BindVertexArray(0);
    }

    /// <summary>
    /// Get the amount of indices sitting in the buffer.
    /// </summary>
    /// <returns></returns>
    public int GetIndexCount()
    {
      return Indices.GetBufferSize();
    }

    /// <summary>
    /// Resets the buffers back to zero.
    /// </summary>
    public void Reset()
    {
      Positions.Reset();
      TextureCoordinates.Reset();
      TextureUnits.Reset();
      Normals.Reset();
      Indices.Reset();
    }

    /// <summary>
    /// Enable a vertex attribute.
    /// </summary>
    /// <param name="location">The attribute location.</param>
    /// <param name="dimensions">The amount of dimensions we are using.</param>
    /// <param name="type">The data type contained within the bound buffer.</param>
    private void EnableAttribute(int location, int dimensions, VertexAttribPointerType type, int stride)
    {
      GL.EnableVertexAttribArray(location);
      GL.VertexAttribPointer(location, dimensions, type, false, stride, 0);
    }
  }
}
