﻿using Firefly.Core;
using Firefly.Texturing;
using Firefly.Rendering;
using Firefly.Core.Shader;
using System.Collections.Generic;
using System;
using Firefly.Utilities;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;
using Firefly.World.Lighting;
using Firefly.Core.Lighting;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using System.Numerics;

namespace Firefly.Core
{

  internal class Pipeline
  {
    private const int BATCH_BUFFER_MAX_INDICES = 4096;
    private const int BATCH_BUFFER_MAX_INDICES_PER_OBJECT = 36;

    private GL GLContext;

    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private CanvasHandler canvasHandler;
    private RenderTextureManager renderTextureManager;
    private DynamicBatchHandler dynamicBatchHandler;
    private MeshBufferHandler modelBufferHandler;
    private PointLightBufferHandler pointLightBufferHandler;
    private AmbientLightBufferHandler ambientLightBufferHandler;
    private CameraHandler cameraHandler;

    private int resolutionWidth;
    private int resolutionHeight;

    private Color clearColor;
    private Color ambientLight;
    private List<PointLight> lighting;

    /// <summary>
    /// Constructor
    /// </summary>
    public Pipeline(GL GLContext, TextureManager textureManager, ShaderManager shaderManager, CanvasHandler canvasHandler)
    {
      this.GLContext = GLContext;
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
      this.canvasHandler = canvasHandler;
      dynamicBatchHandler = new DynamicBatchHandler(GLContext, BATCH_BUFFER_MAX_INDICES, BATCH_BUFFER_MAX_INDICES_PER_OBJECT, textureManager, shaderManager);
      modelBufferHandler = new MeshBufferHandler(GLContext, textureManager, shaderManager);
      pointLightBufferHandler = new PointLightBufferHandler(GLContext, 0);
      ambientLightBufferHandler = new AmbientLightBufferHandler(GLContext, 1);

      renderTextureManager = new RenderTextureManager(textureManager);

      cameraHandler = new CameraHandler();
    }

    /// <summary>
    /// Updates the ambient light.
    /// </summary>
    /// <param name="ambientLight"></param>
    public void SetAmbientLight(Color ambientLight)
    {
      this.ambientLight = ambientLight;
      ambientLightBufferHandler.BufferLightData(ambientLight);
    }

    /// <summary>
    /// Updates the clear color.
    /// </summary>
    /// <param name="clearColor"></param>
    public void SetClearColor(Color clearColor)
    {
      this.clearColor = clearColor;
    }

    /// <summary>
    /// Push an object into the pipeline and render it, along with all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderScene(Scene scene, bool raw)
    {
      lighting = scene.Lights;

      for (int i = 0; i < scene.Cameras.Count; i++)
      {
        Camera camera = scene.Cameras[i];
        RenderTexture renderTexture = camera.RenderTexture;
        if (camera.RenderTexture == null)
        {
          continue;
        }
        renderTextureManager.BindRenderTexture(renderTexture);
        GLContext.Viewport(new System.Drawing.Size(renderTexture.Width, renderTexture.Height));
        GLContext.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        GLContext.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        AssignCamera(camera);
        BufferObject(scene.RootObject);
        FlushBatchBuffers();
      }

      if (!raw)
      {
        canvasHandler.BindFrameBuffer();
      }
      Color c = clearColor;
      GLContext.ClearColor(c.R, c.G, c.B, 1.0f);
      GLContext.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      AssignCamera(scene.Camera);
      BufferObject(scene.RootObject);
      FlushBatchBuffers();
      if (!raw)
      {
        canvasHandler.DrawCanvas();
      }
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
    /// Pre-upload a model to the GPU.
    /// </summary>
    /// <param name="model"></param>
    public void UploadModel(Model model)
    {
      modelBufferHandler.BufferModel(model);
    }

    /// <summary>
    /// Delete model from video memory.
    /// </summary>
    /// <param name="model"></param>
    public void DeleteModel(Model model)
    {
      modelBufferHandler.BufferModel(model);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Clear()
    {

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
        MeshObject mesh = (MeshObject)obj;
        if (mesh.Model != null && mesh.Material != null && mesh.Visible)
        {
          if (dynamicBatchHandler.IsMeshBatchable(mesh))
          {
            // The mesh can be batched
            if (!dynamicBatchHandler.IsMeshCompatible(mesh))
            {
              // The mesh can be uploaded to the current buffer
              FlushBatchBuffers();
            }

            dynamicBatchHandler.AddToBatch(mesh);
          }
          else
          {
            uint modelId = mesh.Model.Id;

            // buffer method for larger objects
            modelBufferHandler.BufferModel(mesh.Model);
            modelBufferHandler.BindModel(modelId, mesh.Textures, mesh.Material);

            Matrix4x4 modelMatrix = mesh.Transform.GetLocalMatrix();

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
        Render(dynamicBatchHandler.GetBatchMaterial(), batchSize, Matrix4x4.Identity);
        dynamicBatchHandler.Reset();
        textureManager.ClearAllTextureSlots();
      }
    }

    private void Render(Material material, int count, Matrix4x4 modelMatrix)
    {
      pointLightBufferHandler.BufferLightData(lighting);
      // Reset viewport
      GLContext.Viewport(new System.Drawing.Size(resolutionWidth, resolutionHeight));
      // cull back faces
      GLContext.Enable(EnableCap.CullFace);

      // test vertex depth
      if (material.DepthFunction != Rendering.DepthFunction.None)
      {
        GLContext.Enable(EnableCap.DepthTest);
        GLContext.DepthFunc((Silk.NET.OpenGL.DepthFunction)material.DepthFunction);
      }

      GLContext.Enable(EnableCap.Blend);
      GLContext.BlendEquation(BlendEquationMode.FuncAdd);
      GLContext.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

      ShaderComponent shaderComponent = shaderManager.GetComponent(material);
      shaderComponent.Use();

      Matrix4x4 projectionMatrix = cameraHandler.GetProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
      Matrix4x4 viewMatrix = cameraHandler.GetViewMatrix();

      int screenToClipLocation = shaderComponent.GetUniformLocation("u_projectionMatrix");
      GLContext.UniformMatrix4(screenToClipLocation, true, ref projectionMatrix);
      int modelMatrixLocation = shaderComponent.GetUniformLocation("u_modelMatrix");
      GLContext.UniformMatrix4(modelMatrixLocation, true, ref modelMatrix);
      int viewMatrixLocation = shaderComponent.GetUniformLocation("u_viewMatrix");
      GLContext.UniformMatrix4(viewMatrixLocation, true, ref viewMatrix);

      shaderComponent.BindUniformBlock("PointLightBlock", pointLightBufferHandler.GetBlockIndex());
      shaderComponent.BindUniformBlock("AmbientLightBlock", ambientLightBufferHandler.GetBlockIndex());

      if (material.Uniforms != null)
      {
        for (int i = 0; i < material.Uniforms.Length; i++)
        {
          Uniform uniform = material.Uniforms[i];
          if (uniform.name == "u_images" || uniform.name == "u_projectionMatrix" || uniform.name == "u_modelMatrix" || uniform.name == "u_viewMatrix")
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
        GLContext.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedInt, 0);
      } else if (material.PrimitiveType == Rendering.PrimitiveType.Lines)
      {
        GLContext.DrawElements(BeginMode.Lines, count, DrawElementsType.UnsignedInt, 0);
      }
    }

    private bool IsMeshWithinFrustum(MeshObject mesh, Matrix4x4 pm)
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
