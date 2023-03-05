using Firefly.Texturing.Settings;

namespace Firefly.Texturing
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
