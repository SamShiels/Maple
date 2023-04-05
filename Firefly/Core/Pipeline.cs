using Firefly.Texturing;
using Firefly.Rendering;
using Firefly.Core.Shader;
using System.Collections.Generic;
using System;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;
using Firefly.World.Lighting;
using Firefly.Core.Lighting;
using Firefly.World.Scene;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using System.Numerics;

namespace Firefly.Core
{

  internal class Pipeline : RendererComponent
  {
    private const int BATCH_BUFFER_MAX_INDICES = 4096;
    private const int BATCH_BUFFER_MAX_INDICES_PER_OBJECT = 36;

    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private CanvasHandler canvasHandler;
    private RenderTextureManager renderTextureManager;
    private DynamicBatchHandler dynamicBatchHandler;
    private MeshBufferHandler modelBufferHandler;
    private PointLightBufferHandler pointLightBufferHandler;
    private AmbientLightBufferHandler ambientLightBufferHandler;
    private CameraHandler cameraHandler;
    private SkyboxHandler skyboxHandler;

    private int resolutionWidth;
    private int resolutionHeight;

    private Vector4D<float> clearColor;
    private Vector4D<float> ambientLight;
    private List<PointLight> lighting;

    /// <summary>
    /// Constructor
    /// </summary>
    public Pipeline(TextureManager textureManager, ShaderManager shaderManager, CanvasHandler canvasHandler, GL GLContext) : base(GLContext)
    {
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
      this.canvasHandler = canvasHandler;
      dynamicBatchHandler = new DynamicBatchHandler(BATCH_BUFFER_MAX_INDICES, BATCH_BUFFER_MAX_INDICES_PER_OBJECT, textureManager, shaderManager, GLContext);
      modelBufferHandler = new MeshBufferHandler(textureManager, shaderManager, GLContext);
      pointLightBufferHandler = new PointLightBufferHandler(0);
      ambientLightBufferHandler = new AmbientLightBufferHandler(1);

      renderTextureManager = new RenderTextureManager(textureManager);

      cameraHandler = new CameraHandler();
      skyboxHandler = new SkyboxHandler(shaderManager, textureManager, GLContext);
    }

    /// <summary>
    /// Updates the ambient light.
    /// </summary>
    /// <param name="ambientLight"></param>
    public void SetAmbientLight(Vector4D<float> ambientLight)
    {
      this.ambientLight = ambientLight;
      ambientLightBufferHandler.BufferLightData(ambientLight);
    }

    /// <summary>
    /// Updates the clear color.
    /// </summary>
    /// <param name="clearColor"></param>
    public void SetClearColor(Vector4D<float> clearColor)
    {
      this.clearColor = clearColor;
    }

    /// <summary>
    /// Push an object into the pipeline and render it, along with all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderScene(SceneObject scene, bool raw)
    {
      lighting = scene.Lights;

      //for (int i = 0; i < scene.Cameras.Count; i++)
      //{
      //  Camera camera = scene.Cameras[i];
      //  RenderTexture renderTexture = camera.RenderTexture;
      //  if (camera.RenderTexture == null)
      //  {
      //    continue;
      //  }
      //  renderTextureManager.BindRenderTexture(renderTexture);
      //  GL.Viewport(0, 0, renderTexture.Width, renderTexture.Height);
      //  GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
      //  GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      //  AssignCamera(camera);
      //  BufferObject(scene.RootObject);
      //  FlushBatchBuffers();
      //}

      if (!raw)
      {
        canvasHandler.BindFrameBuffer();
      }
      AssignCamera(scene.Camera);

      Vector4D<float> c = clearColor;
      GL.ClearColor(c.X, c.Y, c.Z, 1.0f);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      Cubemap skybox = scene.Camera.Skybox;
      if (skybox != null)
      {
        Matrix4X4<float> projectionMatrix = cameraHandler.GetProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
        skyboxHandler.DrawSkybox(skybox, scene.Camera.Transform.GetLocalRotationMatrix(), projectionMatrix.Inverted());
      }

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

            ShaderComponent shaderComponent = shaderManager.GetComponent(mesh.Material);
            shaderComponent.Use();

            // Buffer method for larger objects
            modelBufferHandler.BufferModel(mesh.Model);
            modelBufferHandler.BindModel(modelId, mesh.Textures, mesh.Material);

            Matrix4X4<float> modelMatrix = mesh.Transform.GetLocalMatrix();

            Render(mesh.Material, shaderComponent, (uint)mesh.Model.Indices.Length, modelMatrix);
            textureManager.ClearAllTextureSlots();
          }
        }
      }

      List<Transform> children = obj.Transform.GetChildren();

      // If this object has children, iterate through each of them and try to render them as well
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

        Material material = dynamicBatchHandler.GetBatchMaterial();
        ShaderComponent shaderComponent = shaderManager.GetComponent(material);
        shaderComponent.Use();

        dynamicBatchHandler.UploadSamplerPositions();
        Render(material, shaderComponent, (uint)batchSize, Matrix4X4<float>.Identity);
        dynamicBatchHandler.Reset();
        textureManager.ClearAllTextureSlots();
      }
    }

    private void Render(Material material, ShaderComponent shaderComponent, uint count, Matrix4X4<float> modelMatrix)
    {
      pointLightBufferHandler.BufferLightData(lighting);
      // Reset viewport
      GL.Viewport(new Vector2D<int>(resolutionWidth, resolutionHeight));
      // Cull back faces
      GL.Enable(EnableCap.CullFace);

      // Test vertex depth
      if (material.DepthFunction != Rendering.DepthFunction.None)
      {
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc((GLEnum)material.DepthFunction);
      }

      GL.Enable(EnableCap.Blend);
      GL.BlendEquation(GLEnum.FuncAdd);
      GL.BlendFuncSeparate(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha, GLEnum.One, GLEnum.OneMinusSrcAlpha);

      Matrix4X4<float> projectionMatrix = cameraHandler.GetProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
      Matrix4X4<float> viewMatrix = cameraHandler.GetViewMatrix();

      int screenToClipLocation = shaderComponent.GetUniformLocation("u_projectionMatrix");
      GL.UniformMatrix4(screenToClipLocation, false, ref projectionMatrix);
      int modelMatrixLocation = shaderComponent.GetUniformLocation("u_modelMatrix");
      GL.UniformMatrix4(modelMatrixLocation, false, ref modelMatrix);
      int viewMatrixLocation = shaderComponent.GetUniformLocation("u_viewMatrix");
      GL.UniformMatrix4(viewMatrixLocation, false, ref viewMatrix);

      shaderComponent.TryBindPointLightUniform(0);
      shaderComponent.TryBindAmbientLightUniform(1);

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
        GL.DrawElements(GLEnum.Triangles, count, GLEnum.UnsignedInt, 0);
      } else if (material.PrimitiveType == Rendering.PrimitiveType.Lines)
      {
        GL.DrawElements(GLEnum.Lines, count, GLEnum.UnsignedInt, 0);
      }
      else if (material.PrimitiveType == Rendering.PrimitiveType.Points)
      {
        GL.Enable(EnableCap.ProgramPointSize);
        GL.DrawElements(GLEnum.Points, count, GLEnum.UnsignedInt, 0);
      }
    }

    private bool IsMeshWithinFrustum(MeshObject mesh, Matrix4X4<float> pm)
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
