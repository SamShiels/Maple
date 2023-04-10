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

      SceneLoader sceneLoader = new SceneLoader(Assembly.GetExecutingAssembly());
      Stream sceneJson = Assembly.GetExecutingAssembly().GetManifestResourceStream("CubeExample.Scenes.scene.json");

      SceneObject scene = sceneLoader.CreateScene(sceneJson, game.scene);
      OBJLoader loader = new OBJLoader();

      Model model = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("CubeExample.Resources.cube.obj"));
      Image kronk = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CubeExample.Resources.kronk.jpg"));
      Texture texture = new Texture(kronk);

      for (int x = 0; x < 5; x++)
      {
        for (int y = 0; y < 5; y++)
        {
          for (int z = 0; z < 5; z++)
          {
            MeshObject cube = new MeshObject();
            cube.Model = model;
            cube.Transform.Position = new Vector3(x * 5 - 10f, y * 5 - 10f, z * 5 - 10f);
            cube.Textures = new Texture[] { texture };
            Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), null);
            cube.Material = material;
            scene.AddObject(cube);
          }
        }
      }
    }

    private float time = 0;

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
