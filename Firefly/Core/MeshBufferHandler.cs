using OpenTK.Graphics.OpenGL4;
using Firefly.Core.Buffer;
using Firefly.Rendering;
using Firefly.Core.Shader;
using System.Collections.Generic;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;

namespace Firefly.Core
{
  internal class MeshBufferHandler
  {
    private Dictionary<uint, VertexArrayObject> VAOList;
    private Dictionary<uint, uint> DirtyIDs;
    private TextureManager textureManager;
    private ShaderManager shaderManager;

    private VertexArrayObject lastVao;

    public MeshBufferHandler(TextureManager textureManager, ShaderManager shaderManager)
    {
      VAOList = new Dictionary<uint, VertexArrayObject>();
      DirtyIDs = new Dictionary<uint, uint>();
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
    }

    public void BufferMesh(MeshObject mesh)
    {
      if (mesh.Model == null)
      {
        return;
      }
      Model model = mesh.Model;

      VertexArrayObject vertexArrayObject;
      bool vaoExists = VAOList.TryGetValue(mesh.Model.Id, out vertexArrayObject);

      uint dirtyId;
      bool dirtyIdExists = DirtyIDs.TryGetValue(mesh.Model.Id, out dirtyId);

      if (vaoExists)
      {
        // Check if the model has changed in any way. If so, reset the VAO and re-buffer.
        if (dirtyId != mesh.Model.DirtyId)
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

      if (mesh.Textures != null)
      {
        float[] textureUnits = new float[mesh.Model.TexcoordCount];

        for (int i = 0; i < textureUnits.Length; i++)
        {
          textureUnits[i] = 0;
        }

        vertexArrayObject.AddTextureUnits(textureUnits);
      }

      vertexArrayObject.EnablePointers();
      vertexArrayObject.UploadData();
    }

    public void BindMesh(MeshObject mesh)
    {
      Model model = mesh.Model;

      VertexArrayObject vao;
      VAOList.TryGetValue(model.Id, out vao);

      if (vao != null)
      {
        vao.Bind();
        lastVao = vao;
      }

      List<int> usedTextureSlots = new List<int>();

      if (mesh.Textures != null && mesh.Textures.Length > 0)
      {
        for (int i = 0; i < mesh.Textures.Length; i++)
        {
          Texturing.Texture texture = mesh.Textures[i];
          if (texture == null)
          {
            continue;
          }
          usedTextureSlots.Add(textureManager.UseTexture(mesh.Textures[i]));
        }
      }

      UploadSamplerPositions(mesh, usedTextureSlots);
    }

    public void Unbind()
    {
      if (lastVao != null)
      {
        lastVao.Unbind();
      }
    }

    private void UploadSamplerPositions(MeshObject mesh, List<int> texturePositions)
    {
      Material batchMaterial = mesh.Material;
      ShaderComponent shaderComponent = shaderManager.GetComponent(batchMaterial);

      int samplerLocation = shaderComponent.GetUniformLocation("u_images");
      if (samplerLocation > -1)
      {
        int[] samplers = new int[texturePositions.Count];
        for (int i = 0; i < texturePositions.Count; i++)
        {
          samplers[i] = texturePositions[i];
        }
        GL.Uniform1(samplerLocation, samplers.Length, samplers);
      }
    }
  }
}
