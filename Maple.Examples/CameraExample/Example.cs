﻿using OpenTK.Mathematics;
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
using Maple.Core;

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
      AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
      Model model = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.cube.obj"));
      Image kronk = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.kronk.jpg"));
      Image negx = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.Langholmen.negx.jpg"));
      Image negy = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.Langholmen.negy.jpg"));
      Image negz = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.Langholmen.negz.jpg"));
      Image posx = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.Langholmen.posx.jpg"));
      Image posy = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.Langholmen.posy.jpg"));
      Image posz = new Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("CameraExample.Resources.Langholmen.posz.jpg"));

      Cubemap cubemap = new Cubemap(posx, negx, posy, negy, posz, negz);
      game.camera.Skybox = cubemap;

      Texture texture = new Texture(kronk);
      cameraContainer = new WorldObject();
      game.scene.AddObject(cameraContainer);

      PointLight light = new PointLight();
      light.Radius = 10f;
      game.scene.AddObject(light);

      for (int i = 0; i < 5; i++)
      {
        MeshObject cube = new MeshObject();
        cube.Model = model;
        cube.Transform.Position = new Vector3(0f, 3f, -i * 4 - 5);
        cube.Textures = new Texture[] { texture };
        Material material = new Material(game.renderer.ShaderLibrary.GetShader("diffuse"), null);
        cube.Material = material;
        cameraContainer.Transform.AddChild(cube.Transform);
      }

      game.renderer.AmbientLight = new Color4(0.6f, 0.6f, 0.6f, 1.0f);
     // cameraContainer.Transform.AddChild(game.camera.Transform);
    }

    private float displacement = 0.0f;

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
      //cameraContainer.Transform.Position = new Vector3(0.0f, 0, 0.0f);
      //cameraContainer.Transform.Position = new Vector3(0.0f, 0f, -displacement * 5);
      game.camera.Transform.EulerAngles = new Vector3(-displacement / 6, 0.0f, -displacement / 4);
      displacement += 0.01f;
      //game.camera.FieldOfView = (float)(Math.Sin(displacement) + 2.0f);
      base.OnUpdateFrame(args);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
      base.OnRenderFrame(args);
    }
  }
}
