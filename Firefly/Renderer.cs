using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Firefly.Core;
using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.Utilities;
using Firefly.Core.Shader;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Firefly.Core.Texture;
using Firefly.World;
using Firefly.World.Mesh;

namespace Firefly
{
  public class Renderer
  {
    private Pipeline pipeline;
    private CanvasHandler canvasHandler;
    private TextureManager textureManager;
    private ShaderManager shaderManager;

    private static DebugProc _debugProcCallback = DebugMessage;
    private static GCHandle _debugProcCallbackHandle;

    private Color4 ambientLight;

    public Color4 AmbientLight
    {
      get
      {
        return ambientLight;
      }
      set
      {
        ambientLight = value;
        pipeline.SetAmbientLight(value);
      }
    }

    private Color4 clearColor;

    public Color4 ClearColor
    {
      get
      {
        return clearColor;
      }
      set
      {
        clearColor = value;
        pipeline.SetClearColor(value);
      }
    }

    /// <summary>
    /// Window width.
    /// </summary>
    private int resolutionWidth;

    public int ResolutionWidth
    {
      get
      {
        return resolutionWidth;
      }
      set
      {
        resolutionWidth = value;
        ResolutionUpdated();
      }
    }
    /// <summary>
    /// Window height.
    /// </summary>
    private int resolutionHeight;

    public int ResolutionHeight
    {
      get
      {
        return resolutionHeight;
      }
      set
      {
        resolutionHeight = value;
        ResolutionUpdated();
      }
    }

    /// <summary>
    /// Current amount of MSAA samples.
    /// </summary>
    private int msaaSamples = 4;

    public int MSAASamples
    {
      get
      {
        return MSAASamples;
      }
      set
      {
        msaaSamples = value;
        canvasHandler.UpdateMSAA(msaaSamples);
      }
    }

    /// <summary>
    /// The current width of the window.
    /// </summary>
    private int windowWidth;
    /// <summary>
    /// The current height of the window.
    /// </summary>
    private int windowHeight;

    public Renderer(int windowWidth, int windowHeight)
    {
      textureManager = new TextureManager();
      shaderManager = new ShaderManager(textureManager.GetFreeTextureUnitCount());

      Material canvasMaterial = new Material(ShaderLibrary.Instance.GetShader("canvas"));
      resolutionWidth = windowWidth;
      resolutionHeight = windowHeight;
      canvasHandler = new CanvasHandler(shaderManager, canvasMaterial, resolutionWidth, resolutionHeight, windowWidth, windowHeight, msaaSamples, 0);

      pipeline = new Pipeline(textureManager, shaderManager, canvasHandler);

      ClearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
      AmbientLight = new Color4(1.0f, 1.0f, 1.0f, 1.0f);

      UpdateGLViewport(windowWidth, windowHeight);
      ResolutionUpdated();

      _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

      GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    /// <summary>
    /// Render an object and all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void Render(Scene scene)
    {
      pipeline.RenderScene(scene, false);
    }

    /// <summary>
    /// Render an object and all of its children directly to the window.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderRaw(Scene scene)
    {
      pipeline.RenderScene(scene, true);
    }

    /// <summary>
    /// Update the GL viewport. Call this when the window resizes.
    /// </summary>
    /// <param name="windowWidth">Window width.</param>
    /// <param name="windowHeight">Window height.</param>
    public void UpdateGLViewport(int windowWidth, int windowHeight)
    {
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
      canvasHandler.UpdateWindowDimensions(windowWidth, windowHeight);
    }

    /// <summary>
    /// Upload a model to the GPU.
    /// </summary>
    /// <param name="model"></param>
    public void UploadModel(Model model)
    {
      pipeline.UploadModel(model);
    }

    /// <summary>
    /// Delete a model on the GPU.
    /// </summary>
    /// <param name="model"></param>
    public void DeleteModel(Model model)
    {
      pipeline.DeleteModel(model);
    }

    private void ResolutionUpdated()
    {
      canvasHandler.UpdateResolution(resolutionWidth, resolutionHeight);
      pipeline.UpdateResolution(resolutionWidth, resolutionHeight);
    }

    private static void DebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
      string messageString = Marshal.PtrToStringAnsi(message, length);
      Console.WriteLine($"{severity} {type} | {messageString}");

      if (type == DebugType.DebugTypeError)
      {
        //throw new Exception(messageString);
      }
    }

    /// <summary>
    /// Destroy the batch buffers.
    /// </summary>
    public void Destroy()
    {

    }
  }
}
