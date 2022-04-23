using Renderer.Core;
using Renderer.Materials;
using Renderer.Textures;

namespace Renderer.Mesh
{
  public class MeshObject : WorldObject
  {
    public override string TYPE { get; protected set; } = "Mesh";
    public Model Model { get; set; }
    public Material Material { get; set; }
    public Texture[] Textures { get; set; }
    public bool Visible { get; set; }
    public MeshComponent Component { get; private set; }

    public MeshObject() : base()
    {
      IsContainer = false;
      Visible = true;
      Component = new MeshComponent(this);
    }

    public override void Destroy()
    {
      base.Destroy();
      Model.Destroy();
      Model = null;
      Material = null;
      Textures = null;
      Component.Destroy();
      Component = null;
    }
  }
}
