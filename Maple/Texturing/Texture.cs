using Maple.Texturing.Settings;

namespace Maple.Texturing
{
  public class Texture : TextureBase
  {
    public Image Image { private set; get; }

    public Texture(Image Image) : base()
    {
      this.Image = Image;
    }
  }
}
