using Firefly.Rendering;
using Firefly.World.Lighting;
using Firefly.World.Mesh;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Reflection;

namespace Firefly.Test.Mesh
{
  [TestClass]
  public class MeshObject
  {
    [TestMethod]
    public void MeshDoesRender()
    {
      using (Example example = new Example(100, 100, "Test"))
      {
        example.Run();

        //example.Close();
      }

      //World.Mesh.OBJLoader loader = new World.Mesh.OBJLoader();
      //Model cube = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Firefly.Test.Resources.cube.obj"));

      //Uniform ambientLight = new Uniform("u_ambientLight", new Vector3(0.0f, 0.0f, 0.0f));
      //Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      //Uniform shininess = new Uniform("u_shininess", 0.5f);
      //Uniform[] uniforms = new Uniform[3] { ambientLight, directionalLight, shininess };

      //Material material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);

      //World.Mesh.MeshObject cubeMesh = new World.Mesh.MeshObject();
      //cubeMesh.Model = cube;
      //cubeMesh.Material = material;

      //PointLight light = new PointLight();
      //light.Transform.Position = new Vector3(-1f, -2f, -5f);
      //light.Diffuse = Color4.White;
      //light.Radius = 5f;

      //renderer.scene.AddObject(cubeMesh);
      //renderer.scene.AddObject(light);

      //renderer.Render();
      //IntPtr Pixels = IntPtr.Zero;
      //GL.ReadPixels(0, 0, 100, 100, PixelFormat.Rgba, PixelType.UnsignedByte, ref Pixels);

      //Assert.IsTrue(true);

      //renderer.Close();
    }
  }
}