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

namespace SpriteExample
{
  public class Example : Window
  {
    private MeshObject houseMesh;
    private PointLight light;

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();
      OBJLoader loader = new OBJLoader();

      Image house = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("LightingExample.Resources.house.png"));
      Texture textureHouse = new Texture(house);

      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[2] { directionalLight, shininess };

      Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);

      Model houseModel = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("LightingExample.Resources.house.obj"));

      houseMesh = new MeshObject();
      houseMesh.Model = houseModel;
      houseMesh.Transform.Rotation = new Vector3(0f, (float)Math.PI / 2f, 0f);
      houseMesh.Transform.LocalScale = new Vector3(2f, 2f, 2f);
      houseMesh.Textures = new Texture[] { textureHouse };
      houseMesh.Material = material;

      game.scene.AddObject(houseMesh);

      light = new PointLight();
      light.Transform.Position = new Vector3(0f, 0f, 8f);
      light.Diffuse = Color4.Red;
      light.Radius = 1f;

      game.scene.AddObject(light);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
    }

    private float Time = 0.0f;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      Time += 0.02f;
      float positionX = (float)Math.Sin(Time) * 20f;
      houseMesh.Transform.Position = new Vector3(positionX, 0f, 10f);

      base.OnRenderFrame(args);
    }
  }
}
