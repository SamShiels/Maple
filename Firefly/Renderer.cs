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

namespace Firefly
{
  public class Renderer
  {
    private Pipeline pipeline;
    private CanvasHandler canvasHandler;
    private TextureManager textureManager;
    private ShaderManager shaderManager;
    private Color4 ambientLight;
    private Color4 clearColor;

    /// <summary>
    /// Window width.
    /// </summary>
    private int resolutionWidth;
    /// <summary>
    /// Window height.
    /// </summary>
    private int resolutionHeight;

    /// <summary>
    /// World units on the x-axis.
    /// </summary>
    private int windowWidth;
    /// <summary>
    /// World units on the y-axis.
    /// </summary>
    private int windowHeight;
    private Matrix4 projectionMatrix;

    private ProjectionType projectionType = ProjectionType.Perspective;

    public ProjectionType ProjectionType
    {
      get
      {
        return projectionType;
      }
      set
      {
        projectionType = value;
        CalculateProjectionMatrix((float)windowWidth / (float)windowHeight);
      }
    }


    public float orthographicSize = 18.0f;

    public float OrthographicSize
    {
      get
      {
        return orthographicSize;
      }
      set
      {
        orthographicSize = value;
        CalculateProjectionMatrix((float)windowWidth / (float)windowHeight);
      }
    }

    private float verticalFieldOfView = (float)System.Math.PI / 2.5f;
    public float VerticalFieldOfView {
      get
      {
        return verticalFieldOfView;
      }
      set
      {
        verticalFieldOfView = value;
        CalculateProjectionMatrix((float)windowWidth / (float)windowHeight);
      }
    }

    public Renderer(int windowWidth, int windowHeight)
    {
      textureManager = new TextureManager();
      shaderManager = new ShaderManager(textureManager.GetFreeTextureUnitCount());
      pipeline = new Pipeline(textureManager, shaderManager);

      Material canvasMaterial = new Material(ShaderLibrary.Instance.GetShader("canvas"));
      canvasHandler = new CanvasHandler(shaderManager, canvasMaterial, windowWidth, windowHeight, windowWidth, windowHeight, 0, 0);
      clearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
      UpdateWindowDimensions(windowWidth, windowHeight);
      UpdateResolution(windowWidth, windowHeight);

      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    public Renderer(int resolutionWidth, int resolutionHeight, int windowWidth, int windowHeight, Material canvasMaterial, int numberOfSamples, int defaultFrameBufferHandle)
    {
      textureManager = new TextureManager();
      shaderManager = new ShaderManager(textureManager.GetFreeTextureUnitCount());
      pipeline = new Pipeline(textureManager, shaderManager);
      canvasHandler = new CanvasHandler(shaderManager, canvasMaterial, resolutionWidth, resolutionHeight, windowWidth, windowHeight, numberOfSamples, defaultFrameBufferHandle);

      clearColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
      UpdateWindowDimensions(windowWidth, windowHeight);
      UpdateResolution(resolutionWidth, resolutionHeight);

      GL.Enable(EnableCap.DebugOutput);
      GL.Enable(EnableCap.DebugOutputSynchronous);
    }

    /// <summary>
    /// Render an object and all of its children.
    /// </summary>
    /// <param name="obj"></param>
    public void Render(Scene scene)
    {
      canvasHandler.BindFrameBuffer();
      Clear();
      pipeline.RenderScene(scene);
      canvasHandler.DrawCanvas();
    }

    /// <summary>
    /// Render an object and all of its children directly to the window.
    /// </summary>
    /// <param name="obj"></param>
    public void RenderRaw(Scene scene)
    {
      Clear();
      pipeline.RenderScene(scene);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public void UpdateBackgroundColor(Color4 color)
    {
      clearColor = color;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public void UpdateAmbientLight(Color4 color)
    {
      ambientLight = color;
    }

    /// <summary>
    /// Update the GL viewport.
    /// </summary>
    /// <param name="windowWidth">Window width.</param>
    /// <param name="windowHeight">Window height.</param>
    public void UpdateWindowDimensions(int windowWidth, int windowHeight)
    {
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
      canvasHandler.UpdateWindowDimensions(windowWidth, windowHeight);
    }

    /// <summary>
    /// Update the canvas resolution.
    /// </summary>
    /// <param name="resolutionWidth">Resolution width.</param>
    /// <param name="resolutionWidth">Resolution height.</param>
    public void UpdateResolution(int resolutionWidth, int resolutionHeight)
    {
      this.resolutionWidth = resolutionWidth;
      this.resolutionHeight = resolutionHeight;
      canvasHandler.UpdateResolution(resolutionWidth, resolutionHeight);
      pipeline.UpdateResolution(resolutionWidth, resolutionHeight);
      CalculateProjectionMatrix((float)resolutionWidth / (float)resolutionHeight);
    }

    /// <summary>
    /// Update the MSAA sample count.
    /// </summary>
    /// <param name="numberOfSamples">The number of samples for each pixel.</param>
    public void UpdateMSAA(int numberOfSamples)
    {
      canvasHandler.UpdateMSAA(numberOfSamples);
    }

    /// <summary>
    /// Clear the screen and set OpenGL states to prepare for rendering.
    /// </summary>
    public void Clear()
    {
      Color4 c = clearColor;
      GL.ClearColor(c.R, c.G, c.B, 1.0f);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    /// <summary>
    /// Calculate the screen to clip-space matrix.
    /// </summary>
    private void CalculateProjectionMatrix(float aspect)
    {
      if (projectionType == ProjectionType.Perspective)
      {
        projectionMatrix = Projection.CreatePerspectiveMatrix(verticalFieldOfView, aspect, 0.001f, 1000f);
      } else if (projectionType == ProjectionType.Orthographic)
      {
        projectionMatrix = Projection.CreateOrthographicMatrix(orthographicSize, aspect, 0.001f, 1000f);
      }

      pipeline.UpdateProjectionMatrix(projectionMatrix);
    }

    private void DebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
    {
      string messageString = Marshal.PtrToStringAnsi(message, length);
      Console.WriteLine($"{severity} {type} | {messageString}");

      if (type == DebugType.DebugTypeError)
      {
        throw new Exception(messageString);
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
