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
using Firefly.Core;

namespace CameraExample
{
  public class Example : Window
  {
    private WorldObject cameraContainer;

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();
      OBJLoader loader = new OBJLoader();

      Model model = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.cube.obj"));
      Image kronk = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.kronk.jpg"));
      Texture texture = new Texture(kronk);

      for (int i = 0; i < 5; i++)
      {
        MeshObject cube = new MeshObject();
        cube.Model = model;
        cube.Transform.Position = new Vector3(0f, 0f, -i * 4 - 5);
        cube.Textures = new Texture[] { texture };
        Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), null);
        cube.Material = material;
        game.scene.AddObject(cube);
      }

      cameraContainer = new WorldObject();
      cameraContainer.Transform.AddChild(game.camera.Transform);
    }

    private float displacement = 0.0f;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      //cameraContainer.Transform.Position = new Vector3(0.0f, displacement, 0.0f);
      game.scene.Camera.Transform.Position = new Vector3(0.0f, 0f, displacement);
      game.scene.Camera.Transform.Rotation = new Vector3(displacement, 0.0f, 0.0f);
      displacement += 0.01f;
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
