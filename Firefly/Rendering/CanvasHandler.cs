using OpenTK.Graphics.OpenGL4;
using Firefly.Buffer;
using Firefly.Materials;
using Firefly.Shaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Rendering
{
  public class CanvasHandler
  {
    private int textureResolutionWidth;
    private int textureResolutionHeight;
    private int windowWidth;
    private int windowHeight;

    private int FBOHandler;
    private int RBOHandler;
    private int TextureHandler;

    private bool initialised = false;

    private ShaderManager shaderManager;
    private Material currentMaterial;

    private int VAO;
    private VertexBufferObject<float> Positions { get; set; }
    private VertexBufferObject<float> TextureCoordinates { get; set; }

    public CanvasHandler(ShaderManager shaderManager, Material canvasMaterial, int textureResolutionWidth, int textureResolutionHeight, int windowWidth, int windowHeight)
    {
      this.textureResolutionWidth = textureResolutionWidth;
      this.textureResolutionHeight = textureResolutionHeight;
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
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
      if (!initialised)
      {
        CreateVBOs();
        CreateFrameBuffer();
        CreateTexture();
        CreateRenderBuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
        {
          Console.WriteLine("Frame buffer is not complete");
        }
        initialised = true;
      }

      GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandler);
    }

    /// <summary>
    /// Draw the frame buffer data onto the screen.
    /// </summary>
    public void DrawCanvas()
    {
      ShaderComponent shader = shaderManager.GetComponent(currentMaterial);
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
      GL.Clear(ClearBufferMask.ColorBufferBit);
      GL.BindVertexArray(VAO);
      shader.Use();
      Material material = currentMaterial;

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
          int location = shader.GetUniformLocation(uniform.name);
          UniformData.UploadUniformData(location, uniform.data);
        }
      }

      GL.Viewport(0, 0, windowWidth, windowHeight);
      //Console.WriteLine("Canvas - Viewport set to " + windowWidth + ", " + windowWidth);

      GL.BindTexture(TextureTarget.Texture2D, TextureHandler);
      GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
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
      AllocateTextureMemory();
      AllocateRenderBufferMemory();
    }

    public void UpdateMaterial(Material material)
    {
      currentMaterial = material;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Create the frame buffer handle.
    /// </summary>
    private void CreateFrameBuffer()
    {
      FBOHandler = GL.GenFramebuffer();
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandler);
    }

    /// <summary>
    /// Create the texture used for rendering.
    /// </summary>
    private void CreateTexture()
    {
      TextureHandler = GL.GenTexture();
      AllocateTextureMemory();

      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandler, 0);
    }

    /// <summary>
    /// Create the render buffer handle.
    /// </summary>
    private void CreateRenderBuffer()
    {
      RBOHandler = GL.GenRenderbuffer();
      AllocateRenderBufferMemory();
      GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RBOHandler);
    }

    /// <summary>
    /// Allocate texture memory with the current texture settings and resolution.
    /// </summary>
    private void AllocateTextureMemory()
    {
      GL.BindTexture(TextureTarget.Texture2D, TextureHandler);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, textureResolutionWidth, textureResolutionHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
    }

    /// <summary>
    /// Allocate render buffer memory with a depth buffer.
    /// </summary>
    private void AllocateRenderBufferMemory()
    {
      GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOHandler);
      GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, textureResolutionWidth, textureResolutionHeight);
      GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
    }

    private void CreateVBOs()
    {
      VAO = GL.GenVertexArray();
      GL.BindVertexArray(VAO);
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
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
      TextureCoordinates.Bind();
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

      Positions.BufferData();
      TextureCoordinates.BufferData();
    }

    #endregion
  }
}
