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
using Firefly.World.Scene;
using System.Text.Json;
using System.IO;

namespace CubeExample
{
  public class Example : Window
  {

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();

      OBJLoader loader = new OBJLoader();
      Model model = loader.Load(game.resourceLoader.GetResourceStream("cube.obj"));
      Texture brick = new Texture(new Image(game.resourceLoader.GetResourceStream("brick.png")));
      Texture brickHeight = new Texture(new Image(game.resourceLoader.GetResourceStream("brick-height.png")));
      Texture brickNormal = new Texture(new Image(game.resourceLoader.GetResourceStream("brick-normal.png")));
      Material material = new Material(game.renderer.ShaderLibrary.GetShader("diffuse"), null);

      MeshObject cube = new MeshObject();
      cube.Model = model;
      cube.Transform.Position = new Vector3(0f, 0f, 0f);
      cube.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      cube.Textures = new Texture[] { brick, brickHeight, brickNormal };
      cube.Material = material;
      game.scene.AddObject(cube);
    }

    private float time = 0;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      game.scene.DirectionalLights[0].Transform.EulerAngles = new Vector3(time / 100f, 0.4f, 0f);
      //game.scene.Camera.Transform.Position = new Vector3(0f, 0f, (float)Math.Sin(time / 150f) * 50f);
      time++;
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
