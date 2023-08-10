using OpenTK.Graphics.OpenGL4;
using Maple.Texturing;
using System;

namespace Maple.Core.Texture
{
  internal class TextureComponent
  {
    private TextureBase texture;
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

    public int CreateTexture(TextureBase Texture)
    {
      texture = Texture;

      if (texture.GetType().Name != "RenderTexture")
      {
        GLTexture = GL.GenTexture();
      }
      Initialized = true;

      return GLTexture;
    }

    public void SetUnit(TextureUnit ActiveTextureSlot)
    {
      CurrentTextureSlot = ActiveTextureSlot;
      GL.ActiveTexture(ActiveTextureSlot);

      if (DirtyId != texture.DirtyId)
      {
        //UpdateSettings();
      }
      if (texture.GetType() == typeof(Texturing.Texture))
      {
        GL.BindTexture(TextureTarget.Texture2D, GLTexture);
        if (!Uploaded)
        {
          Texturing.Texture texture = this.texture as Texturing.Texture;
          UploadTexture(texture);
        }
      }
      else if (texture.GetType() == typeof(Cubemap))
      {
        GL.BindTexture(TextureTarget.TextureCubeMap, GLTexture);
        if (!Uploaded)
        {
          Cubemap cubemap = this.texture as Cubemap;
          UploadCubemap(cubemap);
        }
      }
    }

    private void ConvertImageToPixelArrayAndUpload(Image image, TextureTarget target)
    {
      int width = image.Width;
      int height = image.Height;
      byte[] pixelArray = image.GetPixelArray();
      GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelArray);
    }

    private void UpdateSettings(Texturing.Texture texture)
    {
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texture.WrapS);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texture.WrapT);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)texture.MinificationFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)texture.MagnificationFilter);
      DirtyId = texture.DirtyId;
    }

    private void UpdateSettings(Cubemap cubemap)
    {
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)cubemap.WrapS);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)cubemap.WrapT);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)cubemap.WrapR);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)cubemap.MinificationFilter);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)cubemap.MagnificationFilter);
      DirtyId = cubemap.DirtyId;
    }

    private void UploadTexture(Texturing.Texture texture)
    {
      ConvertImageToPixelArrayAndUpload(texture.Image, TextureTarget.Texture2D);

      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      UpdateSettings(texture);
      Uploaded = true;
    }

    private void UploadCubemap(Cubemap cubemap)
    {
      ConvertImageToPixelArrayAndUpload(cubemap.RightImage, TextureTarget.TextureCubeMapPositiveX);
      ConvertImageToPixelArrayAndUpload(cubemap.LeftImage, TextureTarget.TextureCubeMapNegativeX);
      ConvertImageToPixelArrayAndUpload(cubemap.TopImage, TextureTarget.TextureCubeMapNegativeY);
      ConvertImageToPixelArrayAndUpload(cubemap.BottomImage, TextureTarget.TextureCubeMapPositiveY);
      ConvertImageToPixelArrayAndUpload(cubemap.FrontImage, TextureTarget.TextureCubeMapPositiveZ);
      ConvertImageToPixelArrayAndUpload(cubemap.BackImage, TextureTarget.TextureCubeMapNegativeZ);
      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
      }

      UpdateSettings(cubemap);
      Uploaded = true;
    }
  }
}
