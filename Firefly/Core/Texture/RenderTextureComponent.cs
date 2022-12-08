using Firefly.Texturing;
using System;

namespace Firefly.Core.Texture
{
  internal class RenderTextureComponent
  {
    private int FBOHandle;
    private int RBOHandle;
    private int TextureHandle;
    private int DepthTextureHandle;
    private TextureManager textureManager;

    private bool initialised = false;

    private RenderTexture BoundTexture;

    public RenderTextureComponent(TextureManager textureManager)
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
      int width = RenderTexture.Width;
      int height = RenderTexture.Height;
      BoundTexture = RenderTexture;

      Initialize();
      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
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

        FramebufferErrorCode error = Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (error != FramebufferErrorCode.FramebufferComplete)
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
      FBOHandle = Gl.GenFramebuffer();
      Gl.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
    }

    /// <summary>
    /// Create the render buffer handle.
    /// </summary>
    private void CreateRenderBuffer()
    {
      RBOHandle = Gl.GenRenderbuffer();
      int width = BoundTexture.Width;
      int height = BoundTexture.Height;
      AllocateRenderBufferMemory(width, height);
      Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RBOHandle);
    }

    /// <summary>
    /// Create a texture used for rendering.
    /// </summary>
    private void BindTexture()
    {
      int width = BoundTexture.Width;
      int height = BoundTexture.Height;
      TextureHandle = Gl.GenTexture();
      Gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
      Gl.TexImage2D(TextureTarget.Texture2D, 1, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

      DepthTextureHandle = Gl.GenTexture();
      Gl.BindTexture(TextureTarget.Texture2D, DepthTextureHandle);
      Gl.TexImage2D(TextureTarget.Texture2D, 1, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);

      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);
      Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTextureHandle, 0);

      Gl.DrawBuffers(1, new DrawBuffersEnum[1] { DrawBuffersEnum.ColorAttachment0 });

    }

    private void AllocateRenderBufferMemory(int width, int height)
    {
      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOHandle);
      Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
      Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
    }

    /// <summary>
    /// Allocate texture memory with the current texture settings and resolution.
    /// </summary>
    private void AllocateTextureMemory(int width, int height)
    {
      Gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

      Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
    }

    #endregion
  }
}
