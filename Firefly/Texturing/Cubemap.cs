﻿using Firefly.Texturing.Settings;

namespace Firefly.Texturing
{
  public class Cubemap : TextureBase
  {
    public Image RightImage { private set; get; }
    public Image LeftImage { private set; get; }
    public Image TopImage { private set; get; }
    public Image BottomImage { private set; get; }
    public Image FrontImage { private set; get; }
    public Image BackImage { private set; get; }

    public Cubemap(Image RightImage, Image LeftImage, Image TopImage, Image BottomImage, Image FrontImage, Image BackImage)
    {
      this.RightImage = RightImage;
      this.LeftImage = LeftImage;
      this.TopImage = TopImage;
      this.BottomImage = BottomImage;
      this.FrontImage = FrontImage;
      this.BackImage = BackImage;

      DirtyId = 0;

      WrapS = TextureWrapMode.Repeat;
      WrapT = TextureWrapMode.Repeat;
      MinificationFilter = TextureMinFilter.LinearMipmapLinear;
      MagnificationFilter = TextureMagFilter.Linear;
      UseMipMaps = true;
      Id = NextId;
      NextId++;
    }
  }
}
