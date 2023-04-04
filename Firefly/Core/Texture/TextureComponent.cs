using Firefly.Texturing;
using System;
using Silk.NET.OpenGL;

namespace Firefly.Core.Texture
{
  internal class TextureComponent : RendererComponent
  {
    private TextureBase texture;
    public uint GLTexture { get; private set; }
    public bool Initialized { get; private set; }
    public bool Uploaded { get; private set; }
    public uint DirtyId { private set; get; } = 0;
    public TextureUnit CurrentTextureSlot { private set; get; }

    public TextureComponent(GL GLContext) : base(GLContext)
    {
      Initialized = false;
      Uploaded = false;
    }

    public uint CreateTexture(TextureBase Texture)
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

    private unsafe void ConvertImageToPixelArrayAndUpload(Image image, GLEnum target)
    {
      uint width = image.Width;
      uint height = image.Height;
      byte[] pixelArray = image.GetPixelArray();
      GL.TexImage2D(target, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

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
      ConvertImageToPixelArrayAndUpload(texture.Image, GLEnum.Texture2D);

      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GLEnum.Texture2D);
      }
      UpdateSettings(texture);
      Uploaded = true;
    }

    private void UploadCubemap(Cubemap cubemap)
    {
      ConvertImageToPixelArrayAndUpload(cubemap.RightImage, GLEnum.TextureCubeMapPositiveX);
      ConvertImageToPixelArrayAndUpload(cubemap.LeftImage, GLEnum.TextureCubeMapNegativeX);
      ConvertImageToPixelArrayAndUpload(cubemap.TopImage, GLEnum.TextureCubeMapNegativeY);
      ConvertImageToPixelArrayAndUpload(cubemap.BottomImage, GLEnum.TextureCubeMapPositiveY);
      ConvertImageToPixelArrayAndUpload(cubemap.FrontImage, GLEnum.TextureCubeMapPositiveZ);
      ConvertImageToPixelArrayAndUpload(cubemap.BackImage, GLEnum.TextureCubeMapNegativeZ);
      if (texture.UseMipMaps)
      {
        GL.GenerateMipmap(GLEnum.TextureCubeMap);
      }

      UpdateSettings(cubemap);
      Uploaded = true;
    }

    private void UploadRenderTexture()
    {
      RenderTexture renderTexture = (RenderTexture)texture;
      uint width = renderTexture.Width;
      uint height = renderTexture.Height;

      UpdateSettings(renderTexture);
      GL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
      Uploaded = true;
    }
  }
}
