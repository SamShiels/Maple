using Firefly.Texturing.Settings;

namespace Firefly.Texturing
{
  public class Texture
  {
    public static int NextId = 0;

    public Image Image { private set; get; }
    public int Id { private set; get; }
    public int DirtyId { private set; get; } = -1;

    private TextureWrapMode wrapS;
    public TextureWrapMode WrapS
    {
      get { return wrapS; }
      set
      {
        wrapS = value;
        DirtyId++;
      }
    }

    private TextureWrapMode wrapT;
    public TextureWrapMode WrapT
    {
      get { return wrapT; }
      set
      {
        wrapT = value;
        DirtyId++;
      }
    }

    private TextureMinFilter minificationFilter;
    public TextureMinFilter MinificationFilter
    {
      get { return minificationFilter; }
      set
      {
        minificationFilter = value;
        DirtyId++;
      }
    }
    private TextureMagFilter magnificationFilter;
    public TextureMagFilter MagnificationFilter
    {
      get { return magnificationFilter; }
      set
      {
        magnificationFilter = value;
        DirtyId++;
      }
    }
    public bool UseMipMaps { get; set; }

    public Texture(Image Image)
    {
      this.Image = Image;
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
