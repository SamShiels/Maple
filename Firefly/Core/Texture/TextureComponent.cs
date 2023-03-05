using OpenTK.Graphics.OpenGL4;
using Firefly.Texturing;
using System;

namespace Firefly.Core.Texture
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

      GLTexture = GL.GenTexture();
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
      if (texture.GetType().Name == "Texture")
      {
        GL.BindTexture(TextureTarget.Texture2D, GLTexture);
        if (!Uploaded)
        {
          Texturing.Texture texture = this.texture as Texturing.Texture;
          UploadTexture(texture);
        }
      }
      else if (texture.GetType().Name == "Cubemap")
      {
        GL.BindTexture(TextureTarget.TextureCubeMap, GLTexture);
        if (!Uploaded)
        {
          Cubemap cubemap = this.texture as Cubemap;
          UploadCubemap(cubemap);
        }
      }
    }

    private void UpdateSettings()
    {
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texture.WrapS);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texture.WrapT);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)texture.MinificationFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)texture.MagnificationFilter);
      DirtyId = texture.DirtyId;
    }

    private void UpdateSettingsCubemap()
    {
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)texture.WrapS);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)texture.WrapT);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)texture.WrapT);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)texture.MinificationFilter);
      GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)texture.MagnificationFilter);
      DirtyId = texture.DirtyId;
    }

    private void UploadTexture(Texturing.Texture texture)
    {
      ConvertImageToPixelArrayAndUpload(texture.Image, TextureTarget.Texture2D);

      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      UpdateSettings();
      Uploaded = true;
    }

    private void ConvertImageToPixelArrayAndUpload(Image image, TextureTarget target)
    {
      int width = image.Width;
      int height = image.Height;
      byte[] pixelArray = image.GetPixelArray();
      GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelArray);
    }

    private void ConvertImageToPixelArrayAndUploadCube(Image image, TextureTarget target)
    {
      int width = image.Width;
      int height = image.Height;
      byte[] pixelArray = image.GetPixelArray();
      GL.TexImage2D(target, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelArray);
    }

    private void UploadCubemap(Cubemap cubemap)
    {
      ConvertImageToPixelArrayAndUploadCube(cubemap.RightImage, TextureTarget.TextureCubeMapPositiveX);
      ConvertImageToPixelArrayAndUploadCube(cubemap.LeftImage, TextureTarget.TextureCubeMapNegativeX);
      ConvertImageToPixelArrayAndUploadCube(cubemap.TopImage, TextureTarget.TextureCubeMapNegativeY);
      ConvertImageToPixelArrayAndUploadCube(cubemap.BottomImage, TextureTarget.TextureCubeMapPositiveY);
      ConvertImageToPixelArrayAndUploadCube(cubemap.FrontImage, TextureTarget.TextureCubeMapPositiveZ);
      ConvertImageToPixelArrayAndUploadCube(cubemap.BackImage, TextureTarget.TextureCubeMapNegativeZ);
      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
      }

      UpdateSettingsCubemap();
      Uploaded = true;
    }

    private void UploadRenderTexture()
    {
      RenderTexture renderTexture = (RenderTexture)texture;
      int width = renderTexture.Width;
      int height = renderTexture.Height;

      UpdateSettings();
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
      Uploaded = true;
    }
  }
}
