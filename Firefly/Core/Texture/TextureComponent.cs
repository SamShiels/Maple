using Firefly.Texturing;
using System;

namespace Firefly.Core.Texture
{
  internal class TextureComponent
  {
    private Texturing.Texture texture;
    public int GLTexture { get; private set; }
    public bool Initialized { get; private set; }
    public bool Uploaded { get; private set; }
    public int DirtyId { private set; get; } = -1;
    public TextureUnit CurrentTextureSlot { private set; get; }

    public TextureComponent()
    {
      Initialized = false;
      Uploaded = false;
    }

    public void CreateTexture(Texturing.Texture Texture)
    {
      texture = Texture;

      GLTexture = Gl.GenTexture();
      Initialized = true;
    }

    public void SetUnit(TextureUnit ActiveTextureSlot)
    {
      CurrentTextureSlot = ActiveTextureSlot;
      Gl.ActiveTexture(ActiveTextureSlot);
      Gl.BindTexture(TextureTarget.Texture2D, GLTexture);

      if (DirtyId != texture.DirtyId)
      {
        //UpdateSettings();
      }

      if (!Uploaded)
      {
        if (texture.Image != null)
        {
          Upload();
        } else if (texture.GetType().Name == "RenderTexture")
        {
          AllocateRenderTexture();
        }
      }
    }

    private void UpdateSettings()
    {
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texture.WrapS);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texture.WrapT);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)texture.MinificationFilter);
      Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)texture.MagnificationFilter);
      DirtyId = texture.DirtyId;
    }

    private void Upload()
    {
      int width = texture.Image.Width;
      int height = texture.Image.Height;
      byte[] pixelArray = texture.Image.GetPixelArray();

      Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelArray);
      if (texture.UseMipMaps)
      {
        Gl.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      UpdateSettings();
      Uploaded = true;
    }

    private void AllocateRenderTexture()
    {
      RenderTexture renderTexture = (RenderTexture)texture;
      int width = renderTexture.Width;
      int height = renderTexture.Height;

      UpdateSettings();
      Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
      Uploaded = true;
    }
  }
}
