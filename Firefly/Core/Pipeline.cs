using Firefly.Texturing;
using Firefly.Rendering;
using Firefly.Core.Shader;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System;
using Firefly.World;
using Firefly.Core.Texture;
using Firefly.World.Mesh;
using Firefly.World.Lighting;
using Firefly.Core.Lighting;
using Firefly.World.Scene;

namespace Firefly.Core
{

  internal class Pipeline
  {
    private const int BATCH_BUFFER_MAX_INDICES = 4096;
    private const int BATCH_BUFFER_MAX_INDICES_PER_OBJECT = 36;

    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private CanvasHandler canvasHandler;
    private DynamicBatchHandler dynamicBatchHandler;
    private MeshBufferHandler modelBufferHandler;
    private PointLightBufferHandler pointLightBufferHandler;
    private AmbientLightBufferHandler ambientLightBufferHandler;
    private DirectionalLightBufferHandler directionalLightBufferHandler;
    private DirectionalShadowHandler directionalShadowHandler;
    private CameraHandler cameraHandler;
    private SkyboxHandler skyboxHandler;

    private int resolutionWidth;
    private int resolutionHeight;

    private List<PointLight> pointLights;
    private List<DirectionalLight> directionalLights;

    /// <summary>
    /// Constructor
    /// </summary>
    public Pipeline(TextureManager textureManager, ShaderManager shaderManager, CanvasHandler canvasHandler)
    {
      this.textureManager = textureManager;
      this.shaderManager = shaderManager;
      this.canvasHandler = canvasHandler;
      dynamicBatchHandler = new DynamicBatchHandler(BATCH_BUFFER_MAX_INDICES, BATCH_BUFFER_MAX_INDICES_PER_OBJECT, textureManager, shaderManager);
      modelBufferHandler = new MeshBufferHandler(textureManager, shaderManager);
      pointLightBufferHandler = new PointLightBufferHandler(0);
      ambientLightBufferHandler = new AmbientLightBufferHandler(1);
      directionalLightBufferHandler = new DirectionalLightBufferHandler(2);

      directionalShadowHandler = new DirectionalShadowHandler(textureManager.GetMaxTextureUnitCount(), 4);

      cameraHandler = new CameraHandler();
      skyboxHandler = new SkyboxHandler(shaderManager, textureManager);
    }

    /// <summary>
    /// Updates the ambient light.
    /// </summary>
    /// <param name="ambientLight"></param>
    public void SetAmbientLight(Color4 ambientLight)
    {
      ambientLightBufferHandler.BufferLightData(ambientLight);
    }

    /// <summary>
    /// Updates the clear color.
    /// </summary>
    /// <param name="clearColor"></param>
    public void SetClearColor(Color4 clearColor)
    {
    }

    /// <summary>
    /// Push an object into the pipeline and render it, along with all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderScene(SceneObject scene, bool raw)
    {
      pointLights = scene.Lights;

      RenderShadowMaps(scene);

      for (int i = 0; i < scene.Cameras.Count; i++)
      {
        Camera camera = scene.Cameras[i];
        RenderTexture renderTexture = camera.RenderTexture;
        if (camera.RenderTexture == null)
        {
          continue;
        }
        textureManager.BindRenderTextureFrameBuffer(renderTexture);
        RenderCameraView(camera, scene.RootObject);
      }

      if (!raw)
      {
        canvasHandler.BindFrameBuffer();
      }
      RenderCameraView(scene.Camera, scene.RootObject);
      if (!raw)
      {
        canvasHandler.DrawCanvas();
      }
    }

    private void RenderShadowMaps(SceneObject scene)
    {
      directionalLights = scene.DirectionalLights;

      for (int i = 0; i < directionalLights.Count; i++)
      {
        DirectionalLight light = directionalLights[i];
        if (!light.CastShadows)
        {
          continue;
        }

        directionalShadowHandler.BindFrameBuffer(light);

        Matrix4 lightProjection = directionalShadowHandler.GetLightProjectionMatrix(directionalLights[i]);
        Matrix4 lightView = directionalShadowHandler.GetLightViewMatrix(directionalLights[i]);
        Matrix4 lightMatrix = Matrix4.Mult(lightView, lightProjection);

        Material lightDepthMaterial = directionalShadowHandler.GetDepthMaterial();

        GL.Clear(ClearBufferMask.DepthBufferBit);
        BufferObject(scene.RootObject, lightProjection, lightView, lightMatrix, lightDepthMaterial);
        FlushBatchBuffers(lightProjection, lightView, lightMatrix, lightDepthMaterial);
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

    private void RenderCameraView(Camera camera, WorldObject rootObject)
    {
      AssignCamera(camera);

      Color4 c = camera.BackgroundColor;
      GL.ClearColor(c.R, c.G, c.B, 1.0f);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      // Reset viewport
      GL.Viewport(0, 0, resolutionWidth, resolutionHeight);

      Matrix4 projectionMatrix = cameraHandler.GetProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
      Matrix4 viewMatrix = cameraHandler.GetViewMatrix();
      Cubemap skybox = camera.Skybox;
      if (skybox != null)
      {
        skyboxHandler.DrawSkybox(skybox, camera.Transform.GetLocalRotationMatrix(), projectionMatrix.Inverted());
      }

      directionalShadowHandler.BindDepthMap();

      Matrix4 lightMatrix = Matrix4.Identity;

      if (directionalLights.Count > 0)
      {
        Matrix4 lightProjection = directionalShadowHandler.GetLightProjectionMatrix(directionalLights[0]);
        Matrix4 lightView = directionalShadowHandler.GetLightViewMatrix(directionalLights[0]);
        lightMatrix = Matrix4.Mult(lightView, lightProjection);
      }

      BufferObject(rootObject, projectionMatrix, viewMatrix, lightMatrix);
      FlushBatchBuffers(projectionMatrix, viewMatrix, lightMatrix);
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
    private void BufferObject(WorldObject obj, Matrix4 projectionMatrix, Matrix4 viewMatrix, Matrix4 lightMatrix, Material forcedMaterial = null)
    {
      if (obj is MeshObject)
      {
        MeshObject mesh = (MeshObject)obj;
        if (mesh.Model != null && mesh.Material != null && mesh.Visible)
        {
          if (IsMeshWithinFrustum(mesh, projectionMatrix, viewMatrix))
          {
            if (dynamicBatchHandler.IsMeshBatchable(mesh))
            {
              // The mesh can be batched
              if (!dynamicBatchHandler.IsMeshCompatible(mesh))
              {
                // The mesh cannot be uploaded to the current buffer. Flush this batch (render it) and begin a new one.
                FlushBatchBuffers(projectionMatrix, viewMatrix, lightMatrix, forcedMaterial);
              }

              dynamicBatchHandler.AddToBatch(mesh);
            }
            else
            {
              uint modelId = mesh.Model.Id;
              Material usedMaterial = forcedMaterial ?? mesh.Material;

              ShaderComponent shaderComponent = shaderManager.GetComponent(usedMaterial);
              shaderComponent.Use();

              // Buffer method for larger objects
              modelBufferHandler.BufferModel(mesh.Model);
              modelBufferHandler.BindModel(modelId, mesh.Textures, usedMaterial);

              Matrix4 modelMatrix = mesh.Transform.GetLocalMatrix();

              Render(usedMaterial, shaderComponent, mesh.Model.Indices.Length, modelMatrix, projectionMatrix, viewMatrix, lightMatrix);
              textureManager.ClearAllTextureSlots();
            }
          }
        }
      }

      List<Transform> children = obj.Transform.GetChildren();

      // If this object has children, iterate through each of them and try to render them as well
      for (int i = 0; i < children.Count; i++)
      {
        WorldObject owner = children[i].Owner;
        BufferObject(owner, projectionMatrix, viewMatrix, lightMatrix, forcedMaterial);
      }
    }

    /// <summary>
    /// Render out all of the batched objects and reset the buffers.
    /// </summary>
    private void FlushBatchBuffers(Matrix4 projectionMatrix, Matrix4 viewMatrix, Matrix4 lightMatrix, Material forcedMaterial = null)
    {
      int batchSize = dynamicBatchHandler.GetBatchSize();
      if (batchSize > 0) {
        dynamicBatchHandler.BindAndEnablePointers();

        if (forcedMaterial == null)
        {
          forcedMaterial = dynamicBatchHandler.GetBatchMaterial();
        }
        ShaderComponent shaderComponent = shaderManager.GetComponent(forcedMaterial);
        shaderComponent.Use();

        dynamicBatchHandler.UploadSamplerPositions();
        Render(forcedMaterial, shaderComponent, batchSize, Matrix4.Identity, projectionMatrix, viewMatrix, lightMatrix);
        dynamicBatchHandler.Reset();
        textureManager.ClearAllTextureSlots();
      }
    }

    private void Render(Material material, ShaderComponent shaderComponent, int count, Matrix4 modelMatrix, Matrix4 projectionMatrix, Matrix4 viewMatrix, Matrix4 lightMatrix)
    {
      pointLightBufferHandler.BufferLightData(pointLights);
      directionalLightBufferHandler.BufferLightData(directionalLights);
      // Cull back faces
      GL.Enable(EnableCap.CullFace);

      // Test vertex depth
      if (material.DepthFunction != Rendering.DepthFunction.None)
      {
        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc((OpenTK.Graphics.OpenGL4.DepthFunction)material.DepthFunction);
      }

      GL.Enable(EnableCap.Blend);
      GL.BlendEquation(BlendEquationMode.FuncAdd);
      GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

      int modelMatrixLocation = shaderComponent.GetUniformLocation("u_modelMatrix");
      GL.UniformMatrix4(modelMatrixLocation, false, ref modelMatrix);
      int screenToClipLocation = shaderComponent.GetUniformLocation("u_projectionMatrix");
      GL.UniformMatrix4(screenToClipLocation, false, ref projectionMatrix);
      int viewMatrixLocation = shaderComponent.GetUniformLocation("u_viewMatrix");
      GL.UniformMatrix4(viewMatrixLocation, false, ref viewMatrix);
      int lightMatrixLocation = shaderComponent.GetUniformLocation("u_lightSpaceMatrix");
      GL.UniformMatrix4(lightMatrixLocation, false, ref lightMatrix);

      directionalShadowHandler.UploadSamplerPositions(shaderComponent);

      shaderComponent.TryBindPointLightUniform(0);
      shaderComponent.TryBindAmbientLightUniform(1);
      shaderComponent.TryBindDirectionalLightUniform(2);

      if (material.Uniforms != null)
      {
        for (int i = 0; i < material.Uniforms.Length; i++)
        {
          Uniform uniform = material.Uniforms[i];
          if (uniform.name == "u_images" || uniform.name == "u_projectionMatrix" || uniform.name == "u_modelMatrix" || uniform.name == "u_viewMatrix" || uniform.name == "u_lightMatrix")
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
      else if (material.PrimitiveType == Rendering.PrimitiveType.Points)
      {
        GL.Enable(EnableCap.ProgramPointSize);
        GL.DrawElements(BeginMode.Points, count, DrawElementsType.UnsignedInt, 0);
      }
    }

    private bool IsMeshWithinFrustum(MeshObject mesh, Matrix4 projectionMatrix, Matrix4 viewMatrix)
    {
      if (!mesh.EnableFrustumCulling)
      {
        mesh.IsRendered = true;
        return true;
      }
      return true;

      float[] bounds = mesh.Component.WorldBounds;
      bool withinView = true;

      // Maybe cache this?
      Matrix4 viewProjectionMatrix = Matrix4.Mult(viewMatrix, projectionMatrix);

      for (int bound = 0; bound < bounds.Length; bound += 3)
      { 
        float x = bounds[bound    ];
        float y = bounds[bound + 1];
        float z = bounds[bound + 2];

        Vector4 boundPoint = new Vector4(x, y, z, 1f);
        Vector4 point = Vector4.TransformRow(boundPoint, viewProjectionMatrix);

        point.X /= point.W;
        point.Y /= point.W;
        point.Z /= point.W;

        if ((point.X > -1.25 && point.X < 1.25) && (point.Y > -1.25 && point.Y < 1.25) && (point.Z > -1.25 && point.Z < 1.25))
        {
          withinView = true;
          break;
        }
      }

      mesh.IsRendered = withinView;

      return withinView;
    }
  }
}
