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

      Uniform ambientLight = new Uniform("u_ambientLight", new Vector3(0.0f, 0.0f, 0.0f));
      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[3] { ambientLight, directionalLight, shininess };

      Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);

      Model houseModel = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("LightingExample.Resources.house.obj"));

      houseMesh = new MeshObject();
      houseMesh.Model = houseModel;
      //houseMesh.Transform.Position = new Vector3(5f, 0f, 0f);
      houseMesh.Transform.Rotation = new Vector3(0f, 0.0f, 0f);
      houseMesh.Transform.LocalScale = new Vector3(2f, 2f, 2f);
      houseMesh.Textures = new Texture[] { textureHouse };
      houseMesh.Material = material;

      light = new PointLight();
      light.Transform.Position = new Vector3(-1f, -2f, -5f);
      light.Diffuse = Color4.White;
      light.Radius = 5f;

      game.scene.AddObject(houseMesh);
      game.scene.AddObject(light);

      PointLight light2 = new PointLight();
      light2.Transform.Position = new Vector3(0f, 2f, -5f);
      light2.Diffuse = Color4.White;
      light2.Radius = 15f;

      game.scene.AddObject(light2);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
    }

    private float Time = 0.0f;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      Time += 0.02f;
      float positionX = (float)Math.Sin(Time) * 1f;
      float positionZ = (float)Math.Cos(Time) * 4f;
      game.camera.Transform.Position = new Vector3(positionX, 0f, 0.0f);
      game.camera.Transform.Rotation = new Vector3(0.0f, Time, 0.0f);
      houseMesh.Transform.Position = new Vector3(0.0f, 0f, 0f);
      //houseMesh.Transform.Rotation = new Vector3(0.0f, positionZ, 0.0f);

      base.OnRenderFrame(args);
    }
  }
}
