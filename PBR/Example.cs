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

namespace PBR
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
      cube.Transform.Position = new Vector3(0f, 0f, -10f);
      cube.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      cube.Textures = new Texture[] { brick, brickHeight };
      cube.Material = material;
      game.scene.AddObject(cube);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
