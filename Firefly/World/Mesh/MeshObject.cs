using Firefly.Core;
using Firefly.Core.Mesh;
using Firefly.Rendering;
using Firefly.Texturing;

namespace Firefly.World.Mesh
{
  public class MeshObject : WorldObject
  {
    public override string TYPE { get; protected set; } = "Mesh";
    public Model Model { get; set; }
    public Material Material { get; set; }
    public Texture[] Textures { get; set; }
    public bool Visible { get; set; }
    public bool EnableFrustumCulling { get; set; }
    public bool IsRendered { get; internal set; }
    internal MeshComponent Component { get; private set; }

    public MeshObject() : base()
    {
      IsContainer = false;
      Visible = true;
      EnableFrustumCulling = true;
      Component = new MeshComponent(this);
    }

    public override void Destroy()
    {
      base.Destroy();
      Model = null;
      Material = null;
      Textures = null;
      Component.Destroy();
      Component = null;
    }
  }
}
