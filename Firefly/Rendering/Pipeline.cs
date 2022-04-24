using Firefly.Core;
using Firefly.Mesh;
using Firefly.Textures;
using Firefly.Materials;
using Firefly.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System;
using Firefly.Utilities;

namespace Firefly.Rendering
{

  public class Pipeline
  {
    private const int BATCH_BUFFER_MAX_INDICES = 4096;
    private const int BATCH_BUFFER_MAX_INDICES_PER_OBJECT = 36;

    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private DynamicBatchHandler dynamicBatchHandler;
    private MeshBufferHandler modelBufferHandler;

    private Matrix4 projectionMatrix;

    private int resolutionWidth;
    private int resolutionHeight;

    /// <summary>
    /// Constructor
    /// </summary>
    public Pipeline(TextureManager textureManager, ShaderManager shaderManager)
    {
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
      dynamicBatchHandler = new DynamicBatchHandler(BATCH_BUFFER_MAX_INDICES, BATCH_BUFFER_MAX_INDICES_PER_OBJECT, textureManager, shaderManager);
      modelBufferHandler = new MeshBufferHandler(textureManager, shaderManager);
    }

    /// <summary>
    /// Push an object into the pipeline and render it, along with all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderObject(WorldObject obj)
    {
      BufferObject(obj);
      FlushBatchBuffers();
    }

    /// <summary>
    /// Update the projection matrix
    /// </summary>
    /// <param name="projectionMatrix"></param>
    public void UpdateProjectionMatrix(Matrix4 projectionMatrix)
    {
      this.projectionMatrix = projectionMatrix;
    }

    /// <summary>
    /// Update the projection matrix
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void UpdateResolution(int width, int height)
    {
      resolutionWidth = width;
      resolutionHeight = height;
    }

    private void BufferObject(WorldObject obj)
    {
      if (obj.TYPE != "Container")
      {
        // this is not a container. We only use meshes at the moment, so just cast it
        MeshObject mesh = (MeshObject)obj;
        if (mesh.Model != null && mesh.Material != null && mesh.Visible)
        {
          if (dynamicBatchHandler.IsMeshBatchable(mesh))
          {
            if (!dynamicBatchHandler.IsMeshCompatible(mesh))
            {
              FlushBatchBuffers();
            }

            dynamicBatchHandler.AddToBatch(mesh);
          }
          else
          {
            // buffer method for larger objects
            modelBufferHandler.BufferMesh(mesh);
            modelBufferHandler.BindMesh(mesh);

            Matrix4 modelMatrix = mesh.Transform.GetLocalMatrix();

            Render(mesh.Material, mesh.Model.Indices.Length, modelMatrix, Matrix4.Identity);
            textureManager.ClearAllTextureSlots();
          }
        }
      }

      List<Transform> children = obj.Transform.GetChildren();

      // if this object has children, iterate through each of them and try to render them as well
      for (int i = 0; i < children.Count; i++)
      {
        WorldObject owner = children[i].Owner;
        BufferObject(owner);
      }
    }

    /// <summary>
    /// Render out all of the batched objects and reset the buffers.
    /// </summary>
    private void FlushBatchBuffers()
    {
      //modelBufferHandler.Unbind();
      int batchSize = dynamicBatchHandler.GetBatchSize();
      if (batchSize > 0) {
        dynamicBatchHandler.BindAndEnablePointers();
        dynamicBatchHandler.UploadSamplerPositions();
        Render(dynamicBatchHandler.GetBatchMaterial(), batchSize, Matrix4.Identity, Matrix4.Identity);
        dynamicBatchHandler.Reset();
        textureManager.ClearAllTextureSlots();
      }
    }

    private void Render(Material material, int count, Matrix4 modelMatrix, Matrix4 normalMatrix)
    {
      // Reset viewport
      GL.Viewport(0, 0, resolutionWidth, resolutionHeight);
      // cull back faces
      GL.Enable(EnableCap.CullFace);

      // test vertex depth
      GL.Enable(EnableCap.DepthTest);

      GL.Enable(EnableCap.Multisample);

      GL.DepthFunc(DepthFunction.Lequal);
      GL.Enable(EnableCap.Blend);
      GL.BlendEquation(BlendEquationMode.FuncAdd);
      GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

      ShaderComponent shaderComponent = shaderManager.GetComponent(material);
      shaderComponent.Use();
      int screenToClipLocation = shaderComponent.GetUniformLocation("u_projectionMatrix");
      GL.UniformMatrix4(screenToClipLocation, false, ref projectionMatrix);
      int modelMatrixLocation = shaderComponent.GetUniformLocation("u_modelMatrix");
      GL.UniformMatrix4(modelMatrixLocation, false, ref modelMatrix);

      if (material.Uniforms != null)
      {
        for (int i = 0; i < material.Uniforms.Length; i++)
        {
          Uniform uniform = material.Uniforms[i];
          if (uniform.name == "u_images" || uniform.name == "u_projectionMatrix" || uniform.name == "u_modelMatrix")
          {
            // Naughty naughty! We are trying to use builtin uniforms
            Console.WriteLine(uniform.name + " cannot be used because it conflicts with built in uniforms.");
            continue;
          }
          int location = shaderComponent.GetUniformLocation(uniform.name);
          UniformData.UploadUniformData(location, uniform.data);
        }
      }

      GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedInt, 0);
    }

    private bool IsMeshWithinFrustum(MeshObject mesh, Matrix4 pm)
    {
      float[] bounds = mesh.Component.WorldBounds;

      bool withinView = false;

      for (int bound = 0; bound < bounds.Length; bound += 3)
      {
        float x = bounds[bound];
        float y = bounds[bound + 1];
        float z = bounds[bound + 2];

        (float, float, float) pos = Utilities.Math.TransformPoint(pm, x, y, z);

        Console.WriteLine(pos.Item2);

        if ((pos.Item1 > -1 && pos.Item1 < 1) && (pos.Item2 > -1 && pos.Item2 < 1) && (pos.Item3 > -1 && pos.Item3 < 1))
        {
          withinView = true;
          break;
        }
      }

      Console.WriteLine("Culled " + !withinView);

      return withinView;
    }
  }
}
