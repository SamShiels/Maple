using Firefly.Core.Buffer;
using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.Core.Shader;
using System;
using System.Collections.Generic;
using System.Text;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;
using Silk.NET.OpenGL;

namespace Firefly.Core
{
  internal class DynamicBatchHandler
  {
    private GL GLContext;

    private int maxBatchIndices;
    private int maxObjectIndices;

    private TextureManager textureManager;
    private ShaderManager shaderManager;

    private int currentIndexCount;
    private int currentBatchIndexBase;
    private Material currentMaterial;
    private int currentTextureCount;

    private VertexArrayObject buffers;

    public DynamicBatchHandler(GL GLContext, int maxBatchIndices, int maxObjectIndices, TextureManager textureManager, ShaderManager shaderManager)
    {
      this.GLContext = GLContext;
      this.maxBatchIndices = maxBatchIndices;
      this.maxObjectIndices = maxObjectIndices;

      this.textureManager = textureManager;
      this.shaderManager = shaderManager;

      currentIndexCount = 0;
      currentTextureCount = 0;
      currentBatchIndexBase = 0;

      buffers = new VertexArrayObject(DrawType.Dynamic, true);
    }


    /// <summary>
    /// Checks if a mesh is small enough to batch and has a shader which meets requirements.
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public bool IsMeshBatchable(MeshObject mesh)
    {
      int indexCount = mesh.Model.Indices.Length;

      ShaderComponent shaderComponent = shaderManager.GetComponent(mesh.Material);
      int textureCount = mesh.Textures != null ? mesh.Textures.Length : 0;
      bool textureCompatible = (textureCount > 0 && shaderComponent.UsesTextureUnits()) || textureCount == 0;

      return indexCount <= maxObjectIndices && textureCompatible;
    }

    /// <summary>
    /// Check if a mesh is compatible with the batch in its current state. i.e. has the same material and will not cause the batch index count to overflow.
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public bool IsMeshCompatible(MeshObject mesh)
    {
      int indexCount = mesh.Model.Indices.Length;

      bool hasEnoughSpace = currentIndexCount + indexCount <= maxBatchIndices;
      bool materialsEqual = currentMaterial == null || (currentMaterial != null && currentMaterial.Id == mesh.Material.Id);
      bool freeTextureUnits = textureManager.GetMaxTextureUnitCount() - currentTextureCount > 0;

      return hasEnoughSpace && materialsEqual && freeTextureUnits;
    }

    /// <summary>
    /// Add a mesh to the current batch.
    /// </summary>
    /// <param name="mesh"></param>
    public void AddToBatch(MeshObject mesh)
    {
      int[] mappedIndices = new int[mesh.Model.Indices.Length];
      int highestIndex = 0;
      for (int i = 0; i < mappedIndices.Length; i++)
      {
        mappedIndices[i] = currentBatchIndexBase + mesh.Model.Indices[i];
        if (mappedIndices[i] > highestIndex)
        {
          highestIndex = mappedIndices[i];
        }
      }

      currentBatchIndexBase = highestIndex + 1;

      int currentTextureSlot = -1;

      if (mesh.Textures != null && mesh.Textures.Length > 0 && mesh.Model.Texcoords != null && mesh.Model.TexcoordCount > 0)
      {
        currentTextureSlot = textureManager.UseTexture(mesh.Textures[0]);
        float[] textureUnits = new float[mesh.Model.TexcoordCount];

        for (int i = 0; i < textureUnits.Length; i++)
        {
          textureUnits[i] = currentTextureSlot;
        }

        buffers.AddTextureUnits(textureUnits);
      }
      currentTextureCount++;

      currentMaterial = mesh.Material;
      buffers.AddPositions(mesh.Component.WorldVertices);
      buffers.AddTextureCoordinates(mesh.Model.Texcoords);
      buffers.AddNormals(mesh.Model.Normals);
      buffers.AddColors(mesh.Model.Colors);

      buffers.AddIndices(mappedIndices);

      currentIndexCount += mesh.Model.VertexCount;
    }

    /// <summary>
    /// Upload the array of samplers to 'u_images'.
    /// </summary>
    public void UploadSamplerPositions()
    {
      Material batchMaterial = currentMaterial;
      ShaderComponent shaderComponent = shaderManager.GetComponent(batchMaterial);

      int batchSamplerLocation = shaderComponent.GetUniformLocation("u_images");
      if (batchSamplerLocation > -1)
      {
        int[] samplers = new int[textureManager.GetMaxTextureUnitCount()];
        for (int i = 0; i < samplers.Length; i++)
        {
          samplers[i] = i;
        }

        var span = new ReadOnlySpan<int>(samplers);
        GLContext.Uniform1(batchSamplerLocation, span);
      }
    }

    /// <summary>
    /// Bind the VAO and enable the attribute pointers.
    /// </summary>
    public void BindAndEnablePointers()
    {
      buffers.EnablePointers();
      buffers.UploadData();
    }

    /// <summary>
    /// Get the material used in the current batch.
    /// </summary>
    /// <returns>Material</returns>
    public Material GetBatchMaterial()
    {
      return currentMaterial;
    }

    /// <summary>
    /// Get the size of the current batch.
    /// </summary>
    /// <returns></returns>
    public int GetBatchSize()
    {
      return buffers.GetIndexCount();
    }

    /// <summary>
    /// Reset the batch.
    /// </summary>
    public void Reset()
    {
      buffers.Reset();
      currentIndexCount = 0;
      currentTextureCount = 0;
      currentBatchIndexBase = 0;
    }

    public void Unbind()
    {
      buffers.Unbind();
    }
  }
}
