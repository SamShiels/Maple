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
      cubeMesh = new MeshObject();
      cubeMesh.Model = model;
      cubeMesh.Transform.Position = new Vector3(0f, 0f, 5f);
      cubeMesh.Textures = new Texture[] { textureKronk };

      Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), null);

      cubeMesh.Material = material;

      game.scene.AddObject(cubeMesh);
    }

    private float scale = 0.0f;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      scale += 0.01f;
      cubeMesh.Transform.LocalScale = new Vector3(3f + scale * 2, 3f, 1f);
      //cubeMesh.Transform.EulerAngles = new Vector3(scale, 0f, 0f);
    }

    private float Time = 0.0f;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
