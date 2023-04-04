using Firefly.Texturing;
using System;
using Silk.NET.OpenGL;

namespace Firefly.Core.Texture
{
  internal class RenderTextureComponent : RendererComponent
  {
    private uint FBOHandle;
    private uint RBOHandle;
    private uint TextureHandle;
    private uint DepthTextureHandle;
    private TextureManager textureManager;

    private bool initialised = false;

    private RenderTexture BoundTexture;

    public RenderTextureComponent(TextureManager textureManager, GL GLContext) : base(GLContext)
    {
      this.textureManager = textureManager;
    }

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="RenderTexture"></param>
    public void BindRenderTexture(RenderTexture RenderTexture)
    {
      uint width = RenderTexture.Width;
      uint height = RenderTexture.Height;
      BoundTexture = RenderTexture;

      Initialize();
      GL.BindFramebuffer(GLEnum.Framebuffer, FBOHandle);
      //AllocateRenderBufferMemory(width, height);
      //AllocateTextureMemory(width, height);
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
      if (!initialised)
      {
        CreateFrameBuffer();
        BindTexture();
        //CreateRenderBuffer();

        GLEnum error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (error != GLEnum.FramebufferComplete)
        {
          Console.WriteLine("Frame buffer is not complete: " + error);
        }
        initialised = true;
      }
    }

    /// <summary>
    /// Create a frame buffer handle.
    /// </summary>
    private void CreateFrameBuffer()
    {
      FBOHandle = GL.GenFramebuffer();
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
    }

    /// <summary>
    /// Create the render buffer handle.
    /// </summary>
    private void CreateRenderBuffer()
    {
      RBOHandle = GL.GenRenderbuffer();
      uint width = BoundTexture.Width;
      uint height = BoundTexture.Height;
      AllocateRenderBufferMemory(width, height);
      GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RBOHandle);
    }

    /// <summary>
    /// Create a texture used for rendering.
    /// </summary>
    private void BindTexture()
    {
      uint width = BoundTexture.Width;
      uint height = BoundTexture.Height;
      TextureHandle = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
      GL.TexImage2D(TextureTarget.Texture2D, 1, InternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, GLEnum.UnsignedByte, IntPtr.Zero);

      DepthTextureHandle = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, DepthTextureHandle);
      GL.TexImage2D(TextureTarget.Texture2D, 1, InternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);

      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTextureHandle, 0);

      GL.DrawBuffers(1, new GLEnum[1] { GLEnum.ColorAttachment0 });
    }

    private void AllocateRenderBufferMemory(uint width, uint height)
    {
      GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOHandle);
      GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, GLEnum.DepthComponent24, width, height);
      GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
    }

    /// <summary>
    /// Allocate texture memory with the current texture settings and resolution.
    /// </summary>
    private void AllocateTextureMemory(uint width, uint height)
    {
      GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

      GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
    }

    #endregion
  }
}
