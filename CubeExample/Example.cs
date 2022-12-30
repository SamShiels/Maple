using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Firefly;
using ExampleBase;
using System;
using Firefly.Texturing;
using Firefly.Texturing.Settings;
using System.Reflection;
using Firefly.World;
using Firefly.Rendering;
using Firefly.World.Mesh;
using Firefly.World.Lighting;

namespace CubeExample
{
  public class Example : Window
  {
    private MeshObject cubeMesh;
    private MeshObject houseMesh;

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();
      OBJLoader loader = new OBJLoader();

      Model model = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("CubeExample.Resources.cube.obj"));
      Image kronk = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CubeExample.Resources.kronk.jpg"));
      Texture textureKronk = new Texture(kronk);
      MeshObject starDust = new MeshObject();
      starDust.Model = model;
      starDust.Transform.Position = new Vector3(0f, 0f, 5f);
      starDust.Textures = new Texture[] { textureKronk };

      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[2] { directionalLight, shininess };

      Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);

      starDust.Material = material;

      game.scene.AddObject(starDust);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      //cubeMesh.Transform.Position = new Vector3(scale, 0, scale2);
      //mesh.Transform.LocalScale = new Vector3(3f + scale * 2, 3f, 1f);
      //mesh2.Transform.Rotation = (float)Math.PI / 4f;
    }

    private float Time = 0.0f;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
