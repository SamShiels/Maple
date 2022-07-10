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

namespace Firefly.Test
{
  public class Example : Window
  {
    private MeshObject cubeMesh;
    private MeshObject houseMesh;

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();
      OBJLoader loader = new OBJLoader();

      Image house = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.Resources.house.png"));
      Image kronk = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.Resources.grid.jpg"));
      Image castle = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.Resources.castle.png"));
      Texture textureKronk = new Texture(kronk);
      Texture textureCastle = new Texture(castle);
      Texture textureHouse = new Texture(house);

      Uniform ambientLight = new Uniform("u_ambientLight", new Vector3(0.0f, 0.0f, 0.0f));
      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform[] uniforms = new Uniform[2] { ambientLight, directionalLight };

      Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);
      Material material2 = new Material(ShaderLibrary.Instance.GetShader("spriteBasic"), uniforms);

      //mesh = new Sprite();
      //mesh.Transform.Position = new Vector3(2f, 0f, -10f);
      //mesh.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      //mesh.OriginX = 0.5f;
      //mesh.OriginY = 0.5f;
      //mesh.Width = 1;
      //mesh.Textures = new Texture[] { textureKronk };
      //mesh.Material = material;
      Model castleModel = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.Resources.cube.obj"));
      Model cube = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleApp1.Resources.cube.obj"));

      cubeMesh = new MeshObject();
      cubeMesh.Model = castleModel;
      cubeMesh.Transform.Position = new Vector3(0f, 0f, -5f);
      cubeMesh.Transform.LocalScale = new Vector3(4f, 4f, 2f);
      cubeMesh.Textures = new Texture[] { textureHouse };
      cubeMesh.Material = material;

      houseMesh = new MeshObject();
      houseMesh.Model = cube;
      houseMesh.Transform.Position = new Vector3(0f, 0f, 5f);
      houseMesh.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      houseMesh.Textures = new Texture[] { textureHouse };
      houseMesh.Material = material2;

      Sprite sprite = new Sprite();
      sprite.Material = material2;
      sprite.Transform.Position = new Vector3(0f, 0f, -5f);

      game.scene.AddObject(cubeMesh);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      base.OnUpdateFrame(args);
      //cubeMesh.Transform.Position = new Vector3(scale, 0, scale2);
      //mesh.Transform.LocalScale = new Vector3(3f + scale * 2, 3f, 1f);
      //mesh2.Transform.Rotation = (float)Math.PI / 4f;
    }

    private float Time = 0.0f;

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      Time += 0.01f;
      cubeMesh.Transform.Rotation = new Vector3(Time, Time, 0);
      float scale = (float)Math.Sin(Time) * 7;
      float scale2 = (float)Math.Cos(Time / 1) * 150;
      cubeMesh.Transform.Position = new Vector3(scale, 0f, -5f);

      if (Time > 2f)
      {
        //cubeMesh.Destroy();
      }
      base.OnRenderFrame(args);
    }
  }
}
