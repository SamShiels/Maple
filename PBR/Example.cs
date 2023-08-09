using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Maple;
using ExampleBase;
using System;
using Maple.Texturing;
using Maple.Texturing.Settings;
using System.Reflection;
using Maple.World;
using Maple.Rendering;
using Maple.World.Mesh;
using Maple.World.Lighting;
using Maple.World.Scene;
using System.Text.Json;
using System.IO;

namespace PBR
{
  public class Example : Window
  {

    private MeshObject cube;

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();

      OBJLoader loader = new OBJLoader();
      Model model = loader.Load(game.resourceLoader.GetResourceStream("cube.obj"));
      Texture brick = new Texture(new Image(game.resourceLoader.GetResourceStream("brickwall.jpg")));
      Texture brickNormal = new Texture(new Image(game.resourceLoader.GetResourceStream("brickwall_normal.jpg")));
      Material material = new Material(game.renderer.ShaderLibrary.GetShader("diffuse"), null);

      cube = new MeshObject();
      cube.Model = model;
      cube.Transform.Position = new Vector3(0f, 0f, -3f);
      cube.Transform.EulerAngles = new Vector3(0f, 15f, 0f);
      cube.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      cube.Textures = new Texture[] { brick, brickNormal };
      cube.Material = material;
      game.scene.AddObject(cube);

      PointLight light = new PointLight();
      light.Transform.Position = new Vector3(3f, 0f, 0f);
      light.Radius = 10f;
      game.scene.AddObject(light);

      DirectionalLight directionalLight = new DirectionalLight();
      directionalLight.Transform.EulerAngles = new Vector3(-30f, 0f, 30f);
      //game.scene.AddObject(directionalLight);
    }

    private float time = 0.0f;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      cube.Transform.EulerAngles = new Vector3(0.0f, time, 0.0f);
      time += 0.04f;
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
