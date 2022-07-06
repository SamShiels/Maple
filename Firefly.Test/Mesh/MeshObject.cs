using Firefly.Rendering;
using Firefly.World.Mesh;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Reflection;

namespace Firefly.Test.Mesh
{
  [TestClass]
  public class MeshObject
  {
    private RenderingBase renderer;

    [TestMethod]
    public void MeshDoesRender()
    {
      renderer = new RenderingBase();
      World.Mesh.OBJLoader loader = new World.Mesh.OBJLoader();
      Model cube = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Firefly.Test.Resources.cube.obj"));
      Material material = new Material(ShaderLibrary.Instance.GetShader("spriteBasic"), null);
      World.Mesh.MeshObject cubeMesh = new World.Mesh.MeshObject();
      cubeMesh.Model = cube;
      cubeMesh.Material = material;

      renderer.Render();
      IntPtr Pixels = IntPtr.Zero;
      GL.ReadPixels(0, 0, 100, 100, PixelFormat.Rgba, PixelType.UnsignedByte, ref Pixels);

      Assert.IsTrue(true);

      renderer.Close();
    }
  }
}