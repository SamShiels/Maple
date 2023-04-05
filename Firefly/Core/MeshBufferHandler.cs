using Firefly.Core.Buffer;
using Firefly.Rendering;
using Firefly.Core.Shader;
using System.Collections.Generic;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;
using Silk.NET.OpenGL;

namespace Firefly.Core
{
  internal class MeshBufferHandler : RendererComponent
  {
    private Dictionary<uint, VertexArrayObject> VAOList;
    private Dictionary<uint, uint> DirtyIDs;
    private TextureManager textureManager;
    private ShaderManager shaderManager;

    private VertexArrayObject lastVao;

    public MeshBufferHandler(TextureManager textureManager, ShaderManager shaderManager, GL GLContext) : base(GLContext)
    {
      VAOList = new Dictionary<uint, VertexArrayObject>();
      DirtyIDs = new Dictionary<uint, uint>();
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
    }

    public void BufferModel(Model model)
    {
      VertexArrayObject vertexArrayObject;
      bool vaoExists = VAOList.TryGetValue(model.Id, out vertexArrayObject);

      uint dirtyId;
      bool dirtyIdExists = DirtyIDs.TryGetValue(model.Id, out dirtyId);

      if (vaoExists)
      {
        // Check if the model has changed in any way. If so, reset the VAO and re-buffer.
        if (dirtyId != model.DirtyId)
        {
          vertexArrayObject.Reset();
          if (dirtyIdExists)
          {
            DirtyIDs.Remove(model.Id);
          }
          DirtyIDs.Add(model.Id, model.DirtyId);
        }
        else
        {
          return;
        }
      } else
      {
        vertexArrayObject = new VertexArrayObject(DrawType.Dynamic, false);
        VAOList.Add(model.Id, vertexArrayObject);
        DirtyIDs.Add(model.Id, model.DirtyId);
      }

      vertexArrayObject.AddPositions(model.Vertices);
      vertexArrayObject.AddTextureCoordinates(model.Texcoords);
      vertexArrayObject.AddNormals(model.Normals);
      vertexArrayObject.AddColors(model.Colors);
      vertexArrayObject.AddIndices(model.Indices);

      float[] textureUnits = new float[model.TexcoordCount];
      for (int i = 0; i < textureUnits.Length; i++)
      {
        textureUnits[i] = 0;
      }

      vertexArrayObject.AddTextureUnits(textureUnits);

      vertexArrayObject.EnablePointers();
      vertexArrayObject.UploadData();
    }

    public void BindModel(uint modelId, Texturing.Texture[] textures, Material material)
    {
      VertexArrayObject vao;
      VAOList.TryGetValue(modelId, out vao);

      if (vao != null)
      {
        vao.Bind();
        lastVao = vao;
      }

      List<int> usedTextureSlots = new List<int>();

      if (textures != null && textures.Length > 0)
      {
        for (int i = 0; i < textures.Length; i++)
        {
          Texturing.Texture texture = textures[i];
          if (texture == null)
          {
            continue;
          }
          usedTextureSlots.Add(textureManager.UseTexture(textures[i]));
        }
      }

      UploadSamplerPositions(material, usedTextureSlots);
    }

    public void DeleteModel(uint modelId)
    {
      VertexArrayObject vertexArrayObject;
      bool vaoExists = VAOList.TryGetValue(modelId, out vertexArrayObject);

      if (vaoExists)
      {
        vertexArrayObject.Reset();
        vertexArrayObject.Dispose();

        VAOList.Remove(modelId);
      }
    }

    public void Unbind()
    {
      if (lastVao != null)
      {
        lastVao.Unbind();
      }
    }

    private void UploadSamplerPositions(Material material, List<int> texturePositions)
    {
      ShaderComponent shaderComponent = shaderManager.GetComponent(material);

      int samplerLocation = shaderComponent.GetUniformLocation("u_images");
      if (samplerLocation > -1)
      {
        int[] samplers = new int[texturePositions.Count];
        for (int i = 0; i < texturePositions.Count; i++)
        {
          samplers[i] = texturePositions[i];
        }
        GL.Uniform1(samplerLocation, samplers);
      }
    }
  }
}
