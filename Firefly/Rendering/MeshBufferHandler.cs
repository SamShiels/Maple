using OpenTK.Graphics.OpenGL4;
using Firefly.Buffer;
using Firefly.Materials;
using Firefly.Mesh;
using Firefly.Shaders;
using Firefly.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Rendering
{
  public class MeshBufferHandler
  {
    private Dictionary<uint, VertexArrayObject> VAOList;
    private TextureManager textureManager;
    private ShaderManager shaderManager;

    private VertexArrayObject lastVao;

    public MeshBufferHandler(TextureManager textureManager, ShaderManager shaderManager)
    {
      VAOList = new Dictionary<uint, VertexArrayObject>();
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
    }

    public void BufferMesh(MeshObject mesh)
    {
      if (mesh.Model == null || VAOList.ContainsKey(mesh.Model.Id))
      {
        return;
      }

      Model model = mesh.Model;

      VertexArrayObject newVao = new VertexArrayObject(DrawType.Dynamic, false);
      newVao.AddPositions(model.Vertices);
      newVao.AddTextureCoordinates(model.Texcoords);
      newVao.AddNormals(model.Normals);
      newVao.AddIndices(model.Indices);

      if (mesh.Textures != null)
      {
        float[] textureUnits = new float[mesh.Model.Texcoords.Length / 2];

        for (int i = 0; i < textureUnits.Length; i++)
        {
          textureUnits[i] = 0;
        }

        newVao.AddTextureUnits(textureUnits);
      }

      newVao.EnablePointers();
      newVao.UploadData();

      VAOList.Add(model.Id, newVao);
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
          Texture texture = mesh.Textures[i];
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
