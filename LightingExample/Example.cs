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

namespace SpriteExample
{
  public class Example : Window
  {
    private MeshObject houseMesh;
    private Model houseModel;
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

      Material material = new Material(game.renderer.ShaderLibrary.GetShader("diffuse"), uniforms);

      houseModel = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("LightingExample.Resources.house.obj"));

      houseMesh = new MeshObject();
      houseMesh.Model = houseModel;
      houseMesh.Transform.EulerAngles = new Vector3(0f, (float)Math.PI / 2f, 0f);
      houseMesh.Transform.LocalScale = new Vector3(2f, 2f, 2f);
      houseMesh.Textures = new Texture[] { textureHouse };
      houseMesh.Material = material;

      game.scene.AddObject(houseMesh);

      light = new PointLight();
      light.Transform.Position = new Vector3(0f, 0f, 0f);
      light.Diffuse = Color4.Red;
      light.Radius = 1f;
      light.Intensity = 1f;

      game.scene.AddObject(light);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
    }

    private float Time = 0.0f;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      Time += 0.01f;
      float positionX = (float)Math.Sin(Time) * 10f;
      //houseMesh.Transform.LocalScale = new Vector3(positionX / 2, 2f, 2f);

      Quaternion pitchRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, Time / 10f);
      houseMesh.Transform.Position = new Vector3(Time, 0f, 0f);
      houseMesh.Transform.Rotation = pitchRotation * houseMesh.Transform.Rotation;

      if (Time > 5)
      {
        //game.renderer.DeleteModel(houseModel);
        //game.scene.RemoveObject(houseMesh);
      }

      base.OnRenderFrame(args);
    }
  }
}
