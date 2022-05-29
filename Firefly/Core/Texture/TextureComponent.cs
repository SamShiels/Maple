using OpenTK.Graphics.OpenGL4;
using Firefly.Texturing;
using System;

namespace Firefly.Core.Texture
{
  internal class TextureComponent
  {
    private Texturing.Texture texture;
    private Image image;
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

      GLTexture = GL.GenTexture();
      Initialized = true;
    }

    public void SetUnit(TextureUnit ActiveTextureSlot)
    {
      CurrentTextureSlot = ActiveTextureSlot;
      GL.ActiveTexture(ActiveTextureSlot);
      GL.BindTexture(TextureTarget.Texture2D, GLTexture);

      if (DirtyId != texture.DirtyId)
      {
        Upload();
      }
    }

    private void Upload()
    {
      image = texture.Image;
      int width = image.Width;
      int height = image.Height;
      byte[] pixelArray = image.GetPixelArray();

      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelArray);
      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texture.WrapS);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texture.WrapT);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)texture.MinificationFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)texture.MagnificationFilter);
      DirtyId = texture.DirtyId;
      Uploaded = true;
    }
  }
}
