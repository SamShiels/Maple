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

namespace CubeExample
{
  public class Example : Window
  {

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();

      SceneLoader sceneLoader = new SceneLoader(Assembly.GetExecutingAssembly());
      Stream sceneJson = Assembly.GetExecutingAssembly().GetManifestResourceStream("Scene.Scenes.scene.json");

      SceneObject scene = sceneLoader.CreateScene(sceneJson, game.scene);
      OBJLoader loader = new OBJLoader();

      Model model = loader.Load(game.resourceLoader.GetResourceStream("cube.obj"));
      Image brick = new Image(game.resourceLoader.GetResourceStream("brick.png"));
      Texture texture = new Texture(brick);
      Material material = new Material(game.renderer.ShaderLibrary.GetShader("diffuse"), null);

      MeshObject floor = new MeshObject();
      floor.Model = model;
      floor.Transform.Position = new Vector3(0f, -13, 0f);
      floor.Transform.LocalScale = new Vector3(100f, 1f, 100f);
      floor.Textures = new Texture[] { texture };
      floor.Material = material;
      game.scene.AddObject(floor);
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
