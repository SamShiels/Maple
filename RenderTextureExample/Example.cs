﻿using OpenTK.Mathematics;
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

namespace RenderTextureExample
{
  public class Example : Window
  {
    private MeshObject houseMesh;
    private MeshObject cubeMesh;
    private Model houseModel;
    private Model cubeModel;
    private PointLight light;

    public Example(int width, int height, string title) : base(width, height, title) { }

    protected override void OnLoad()
    {
      base.OnLoad();
      OBJLoader loader = new OBJLoader();

      Image house = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("RenderTextureExample.Resources.house.png"));
      Texture textureHouse = new Texture(house);
      RenderTexture renderTexture = new RenderTexture();
      renderTexture.Width = 1000;
      renderTexture.Height = 1000;

      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[2] { directionalLight, shininess };

      Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);

      houseModel = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("RenderTextureExample.Resources.house.obj"));
      cubeModel = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("RenderTextureExample.Resources.cube.obj"));

      houseMesh = new MeshObject();
      houseMesh.Model = houseModel;
      houseMesh.Transform.Position = new Vector3(100f, 0f, 10f);
      houseMesh.Transform.Rotation = new Vector3(0f, (float)Math.PI / 2f, 0f);
      houseMesh.Transform.LocalScale = new Vector3(2f, 2f, 2f);
      houseMesh.Textures = new Texture[] { textureHouse };
      houseMesh.Material = material;

      Camera textureCamera = new Camera();
      textureCamera.RenderTexture = renderTexture;
      textureCamera.Transform.Position = new Vector3(100f, 0f, 0f);

      game.scene.AddObject(houseMesh);
      game.scene.AddObject(textureCamera);

      cubeMesh = new MeshObject();
      cubeMesh.Model = cubeModel;
      cubeMesh.Transform.Position = new Vector3(0f, 0f, 10f);
      cubeMesh.Transform.Rotation = new Vector3(0f, 0f, 0f);
      cubeMesh.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      cubeMesh.Textures = new Texture[] { renderTexture };
      cubeMesh.Material = material;

      game.scene.AddObject(cubeMesh);

      light = new PointLight();
      light.Transform.Position = new Vector3(0f, 0f, 10f);
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
      //houseMesh.Transform.Position = new Vector3(positionX, 0f, 10f);

      if (Time > 5)
      {
        //game.renderer.DeleteModel(houseModel);
        //game.scene.RemoveObject(houseMesh);
      }

      base.OnRenderFrame(args);
    }
  }
}