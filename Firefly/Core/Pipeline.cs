using Firefly.Core;
using Firefly.Texturing;
using Firefly.Rendering;
using Firefly.Core.Shader;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System;
using Firefly.Utilities;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;
using Firefly.World.Lighting;

namespace Firefly.Core
{

  internal class Pipeline
  {
    private const int BATCH_BUFFER_MAX_INDICES = 4096;
    private const int BATCH_BUFFER_MAX_INDICES_PER_OBJECT = 36;

    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private DynamicBatchHandler dynamicBatchHandler;
    private MeshBufferHandler modelBufferHandler;
    private CameraHandler cameraHandler;

    private int resolutionWidth;
    private int resolutionHeight;

    private List<PointLight> lighting;

    /// <summary>
    /// Constructor
    /// </summary>
    public Pipeline(TextureManager textureManager, ShaderManager shaderManager)
    {
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
      dynamicBatchHandler = new DynamicBatchHandler(BATCH_BUFFER_MAX_INDICES, BATCH_BUFFER_MAX_INDICES_PER_OBJECT, textureManager, shaderManager);
      modelBufferHandler = new MeshBufferHandler(textureManager, shaderManager);
      cameraHandler = new CameraHandler();
    }

    /// <summary>
    /// Push an object into the pipeline and render it, along with all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderScene(Scene scene)
    {
      lighting = scene.Lights;
      AssignCamera(scene.Camera);
      BufferObject(scene.RootObject);
      FlushBatchBuffers();
    }

    /// <summary>
    /// Update the projection matrix.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void UpdateResolution(int width, int height)
    {
      resolutionWidth = width;
      resolutionHeight = height;
    }

    /// <summary>
    /// Assign the view matrix from the camera's local matrix.
    /// </summary>
    /// <param name="camera"></param>
    private void AssignCamera(Camera camera)
    {
      cameraHandler.AssignCamera(camera);
    }

    /// <summary>
    /// Evaluate an object and add it to the appropriate buffer.
    /// </summary>
    /// <param name="obj">The object to buffer</param>
    private void BufferObject(WorldObject obj)
    {
      if (obj is MeshObject)
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

            Render(mesh.Material, mesh.Model.Indices.Length, modelMatrix);
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
      int batchSize = dynamicBatchHandler.GetBatchSize();
      if (batchSize > 0) {
        dynamicBatchHandler.BindAndEnablePointers();
        dynamicBatchHandler.UploadSamplerPositions();
        Render(dynamicBatchHandler.GetBatchMaterial(), batchSize, Matrix4.Identity);
        dynamicBatchHandler.Reset();
        textureManager.ClearAllTextureSlots();
      }
    }

    private void Render(Material material, int count, Matrix4 modelMatrix)
    {
      // Reset viewport
      GL.Viewport(0, 0, resolutionWidth, resolutionHeight);
      // cull back faces
      GL.Enable(EnableCap.CullFace);

      // test vertex depth
      GL.Enable(EnableCap.DepthTest);

      GL.DepthFunc(DepthFunction.Lequal);
      GL.Enable(EnableCap.Blend);
      GL.BlendEquation(BlendEquationMode.FuncAdd);
      GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

      ShaderComponent shaderComponent = shaderManager.GetComponent(material);
      shaderComponent.Use();

      Matrix4 projectionMatrix = cameraHandler.GetProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
      Matrix4 viewMatrix = cameraHandler.GetViewMatrix();

      int screenToClipLocation = shaderComponent.GetUniformLocation("u_projectionMatrix");
      GL.UniformMatrix4(screenToClipLocation, true, ref projectionMatrix);
      int modelMatrixLocation = shaderComponent.GetUniformLocation("u_modelMatrix");
      GL.UniformMatrix4(modelMatrixLocation, true, ref modelMatrix);
      int viewMatrixLocation = shaderComponent.GetUniformLocation("u_viewMatrix");
      GL.UniformMatrix4(viewMatrixLocation, true, ref viewMatrix);

      //Matrix4 mvp = Matrix4.Mult(Matrix4.Mult(projectionMatrix, viewMatrix), modelMatrix);
      //int mvpLocation = shaderComponent.GetUniformLocation("u_mvp");
      //GL.UniformMatrix4(mvpLocation, false, ref mvp);

      int pointLightCount = lighting.Count;
      for (int i = 0; i < System.Math.Min(pointLightCount, 16); i++)
      {
        PointLight light = lighting[i];

        int usedLocation = shaderComponent.GetUniformLocation(string.Format("u_pointLights.used{0}", i.ToString()));
        GL.Uniform1(usedLocation, 1);
        int positionLocation = shaderComponent.GetUniformLocation(string.Format("u_pointLights.position{0}", i.ToString()));
        GL.Uniform3(positionLocation, light.Transform.Position);
        int rangeLocation = shaderComponent.GetUniformLocation(string.Format("u_pointLights.range{0}", i.ToString()));
        GL.Uniform1(rangeLocation, light.Radius);
      }

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
          UniformDataHelper.UploadUniformData(location, uniform.data);
        }
      }

      if (material.PrimitiveType == Rendering.PrimitiveType.Triangles)
      {
        GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedInt, 0);
      } else if (material.PrimitiveType == Rendering.PrimitiveType.Lines)
      {
        GL.DrawElements(BeginMode.Lines, count, DrawElementsType.UnsignedInt, 0);
      }
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
