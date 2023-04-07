using OpenTK.Graphics.OpenGL4;
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

    public RenderTextureComponent(TextureManager textureManager, RenderTexture renderTexture)
    {
      this.textureManager = textureManager;
      BoundTexture = renderTexture;
    }

    #region Public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="RenderTexture"></param>
    public void BindFrameBuffer()
    {
      int width = BoundTexture.Width;
      int height = BoundTexture.Height;

      Initialize();
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
      //AllocateRenderBufferMemory(width, height);
      //AllocateTextureMemory(width, height);
    }

    public void BindRenderTexture()
    {
      GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
      if (!initialised)
      {
        CreateFrameBuffer();
        BindTextureToFrameBuffer();
        //CreateRenderBuffer();

        FramebufferErrorCode error = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
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
      FBOHandle = GL.GenFramebuffer();
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
    }

    /// <summary>
    /// Create the render buffer handle.
    /// </summary>
    private void CreateRenderBuffer()
    {
      RBOHandle = GL.GenRenderbuffer();
      int width = BoundTexture.Width;
      int height = BoundTexture.Height;
      AllocateRenderBufferMemory(width, height);
      GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RBOHandle);
    }

    /// <summary>
    /// Create a texture used for rendering.
    /// </summary>
    private void BindTextureToFrameBuffer()
    {
      int width = BoundTexture.Width;
      int height = BoundTexture.Height;
      TextureHandle = GL.GenTexture();
      AllocateTextureMemory(width, height);

      //DepthTextureHandle = GL.GenTexture();
      //GL.BindTexture(TextureTarget.Texture2D, DepthTextureHandle);
      //GL.TexImage2D(TextureTarget.Texture2D, 1, PixelInternalFormat.DepthComponent32f, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);

      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);
      //GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthTextureHandle, 0);

      //GL.DrawBuffers(1, new DrawBuffersEnum[1] { DrawBuffersEnum.ColorAttachment0 });
    }

    private void AllocateRenderBufferMemory(int width, int height)
    {
      GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOHandle);
      GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
      GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
    }

    /// <summary>
    /// Allocate texture memory with the current texture settings and resolution.
    /// </summary>
    private void AllocateTextureMemory(int width, int height)
    {
      GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
    }

    #endregion
  }
}
