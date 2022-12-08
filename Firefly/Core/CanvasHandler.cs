using Firefly.Core.Buffer;
using Firefly.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using Firefly.Core.Shader;

namespace Firefly.Core
{
  internal class CanvasHandler
  {
    private int textureResolutionWidth;
    private int textureResolutionHeight;
    private int numberOfSamples;
    private int defaultFrameBufferHandle;

    private int windowWidth;
    private int windowHeight;

    private int FBOHandle;
    private int MultiSampleFBOHandle;
    private int RBOHandle;
    private int TextureHandle;
    private int MultiSampleTextureHandle;

    private bool initialised = false;

    private ShaderManager shaderManager;
    private Material currentMaterial;

    private int VAO;
    private VertexBufferObject<float> Positions { get; set; }
    private VertexBufferObject<float> TextureCoordinates { get; set; }

    public CanvasHandler(ShaderManager shaderManager, Material canvasMaterial, int textureResolutionWidth, int textureResolutionHeight, int windowWidth, int windowHeight, int numberOfSamples, int defaultFrameBufferHandle)
    {
      this.textureResolutionWidth = textureResolutionWidth;
      this.textureResolutionHeight = textureResolutionHeight;
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
      this.numberOfSamples = numberOfSamples;
      this.defaultFrameBufferHandle = defaultFrameBufferHandle;
      initialised = false;

      this.shaderManager = shaderManager;
      currentMaterial = canvasMaterial;
    }

    #region Public Methods

    /// <summary>
    /// Bind to the frame buffer used to draw our scene.
    /// </summary>
    public void BindFrameBuffer()
    {
      Initialize();
      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, MultiSampleFBOHandle);
    }

    /// <summary>
    /// Draw the frame buffer data onto the screen.
    /// </summary>
    public void DrawCanvas()
    {
      ShaderComponent shader = shaderManager.GetComponent(currentMaterial);
      Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, MultiSampleFBOHandle);
      Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FBOHandle);
      Gl.BlitFramebuffer(0, 0, textureResolutionWidth, textureResolutionHeight, 0, 0, textureResolutionWidth, textureResolutionHeight, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, defaultFrameBufferHandle);
      Gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
      Gl.Clear(ClearBufferMask.ColorBufferBit);
      Gl.BindVertexArray(VAO);
      shader.Use();
      Material material = currentMaterial;

      if (material.Uniforms != null)
      {
        for (int i = 0; i < material.Uniforms.Length; i++)
        {
          Rendering.Uniform uniform = material.Uniforms[i];
          if (uniform.name == "u_images" || uniform.name == "u_projectionMatrix" || uniform.name == "u_modelMatrix")
          {
            // Tisk tisk! We are trying to use builtin uniforms
            Console.WriteLine(uniform.name + " cannot be used because it conflicts with built in uniforms.");
            continue;
          }
          int location = shader.GetUniformLocation(uniform.name);
          UniformDataHelper.UploadUniformData(location, uniform.data);
        }
      }

      Gl.Viewport(0, 0, windowWidth, windowHeight);
      //Console.WriteLine("Canvas - Viewport set to " + windowWidth + ", " + windowWidth);

      Gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
      Gl.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 6);
    }

    /// <summary>
    /// Update the stored dimensions of the window.
    /// </summary>
    /// <param name="windowWidth"></param>
    /// <param name="windowHeight"></param>
    public void UpdateWindowDimensions(int windowWidth, int windowHeight)
    {
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
    }

    /// <summary>
    /// Update the resolution settings and re-allocate texture and render buffer memory.
    /// </summary>
    /// <param name="textureResolutionWidth">The texture resolution width.</param>
    /// <param name="textureResolutionHeight">The texture resolution height.</param>
    public void UpdateResolution(int textureResolutionWidth, int textureResolutionHeight)
    {
      this.textureResolutionWidth = textureResolutionWidth;
      this.textureResolutionHeight = textureResolutionHeight;
      UpdateResolution();
    }

    /// <summary>
    /// Update the resolution settings and re-allocate texture and render buffer memory.
    /// </summary>
    /// <param name="numberOfSamples">The number of samples</param>
    public void UpdateMSAA(int numberOfSamples)
    {
      this.numberOfSamples = numberOfSamples;
      UpdateResolution();
    }

    public void UpdateMaterial(Material material)
    {
      currentMaterial = material;
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
      if (!initialised)
      {
        CreateVBOs();

        // MSAA stuff
        MultiSampleFBOHandle = CreateFrameBuffer();
        CreateMultiSampleTexture();
        CreateRenderBuffer();
        if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
          Console.WriteLine("MSAA Frame buffer is not complete");
        }
        //

        FBOHandle = CreateFrameBuffer();
        CreateTexture();
        Gl.BindFramebuffer(FramebufferTarget.Framebuffer, defaultFrameBufferHandle);

        if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
          Console.WriteLine("Frame buffer is not complete");
        }
        initialised = true;
      }
    }

    private void UpdateResolution()
    {
      AllocateTextureMemory();
      Gl.BindTexture(TextureTarget.Texture2DMultisample, MultiSampleTextureHandle);
      Gl.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, numberOfSamples, PixelInternalFormat.Rgb, textureResolutionWidth, textureResolutionHeight, true);
      Gl.BindTexture(TextureTarget.Texture2DMultisample, 0);
      AllocateRenderBufferMemory();
    }

    /// <summary>
    /// Create a frame buffer handle.
    /// </summary>
    private int CreateFrameBuffer()
    {
      int handle = Gl.GenFramebuffer();
      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

      return handle;
    }

    /// <summary>
    /// Create a texture used for rendering.
    /// </summary>
    private void CreateMultiSampleTexture()
    {
      MultiSampleTextureHandle = Gl.GenTexture();
      Gl.BindTexture(TextureTarget.Texture2DMultisample, MultiSampleTextureHandle);
      Gl.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, numberOfSamples, PixelInternalFormat.Rgb, textureResolutionWidth, textureResolutionHeight, true);
      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, MultiSampleTextureHandle, 0);
      Gl.BindTexture(TextureTarget.Texture2DMultisample, 0);
    }

    /// <summary>
    /// Create a texture used for rendering.
    /// </summary>
    private void CreateTexture()
    {
      TextureHandle = Gl.GenTexture();
      AllocateTextureMemory();

      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);
    }

    /// <summary>
    /// Create the render buffer handle.
    /// </summary>
    private void CreateRenderBuffer()
    {
      RBOHandle = Gl.GenRenderbuffer();
      AllocateRenderBufferMemory();
      Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RBOHandle);
    }

    private void AllocateRenderBufferMemory()
    {
      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOHandle);
      Gl.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, numberOfSamples, RenderbufferStorage.DepthComponent24, textureResolutionWidth, textureResolutionHeight);
      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
    }

    /// <summary>
    /// Allocate texture memory with the current texture settings and resolution.
    /// </summary>
    private void AllocateTextureMemory()
    {
      Gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

      Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, textureResolutionWidth, textureResolutionHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
    }

    private void CreateVBOs()
    {
      VAO = Gl.GenVertexArray();
      Gl.BindVertexArray(VAO);
      Positions = new VertexBufferObject<float>(DrawType.Static, false);
      TextureCoordinates = new VertexBufferObject<float>(DrawType.Static, false);

      Positions.PushData(new float[] { -1.0f, -1.0f, -1.0f,
                                        1.0f, -1.0f, -1.0f,
                                        1.0f,  1.0f, -1.0f,
                                       -1.0f, -1.0f, -1.0f,
                                        1.0f,  1.0f, -1.0f,
                                       -1.0f,  1.0f, -1.0f });

      TextureCoordinates.PushData(new float[] { 0.0f, 0.0f,
                                                1.0f, 0.0f,
                                                1.0f, 1.0f,
                                                0.0f, 0.0f,
                                                1.0f, 1.0f,
                                                0.0f, 1.0f });

      // bind the positions buffer and enable the positions attribute pointer
      Positions.Bind();
      Gl.EnableVertexAttribArray(0);
      Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
      TextureCoordinates.Bind();
      Gl.EnableVertexAttribArray(1);
      Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

      Positions.BufferData();
      TextureCoordinates.BufferData();
    }

    #endregion
  }
}
