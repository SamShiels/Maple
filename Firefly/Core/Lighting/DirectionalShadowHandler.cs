using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Lighting
{
  internal class DirectionalShadowHandler
  {
    private int depthTextureHandle;
    private int FBOHandle;

    private const int width = 1024;
    private const int height = 1024;

    public DirectionalShadowHandler()
    {
      
    }

    public void Initialize()
    {
      depthTextureHandle = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, depthTextureHandle);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

      FBOHandle = GL.GenFramebuffer();
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTextureHandle, 0);
      GL.DrawBuffer(DrawBufferMode.None);
      GL.ReadBuffer(ReadBufferMode.None);
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
  }
}
