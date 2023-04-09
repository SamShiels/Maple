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
    }

    private float time = 0;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      WorldObject cube = game.scene.RootObject.Transform.GetChildren()[1].Owner;
      cube.Transform.Position = new Vector3((float)Math.Sin(time / 100) * 30, 0.0f, -10);
      game.camera.Transform.Position = new Vector3((float)Math.Cos(time / 100) * 30, 0.0f, 0f);
      //time++;
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
