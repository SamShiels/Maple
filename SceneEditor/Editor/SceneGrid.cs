using Firefly.Rendering;
using Firefly.World;
using Firefly.World.Mesh;
using System;
using System.Collections.Generic;
using System.Text;

namespace SceneEditor.Editor
{
  public class SceneGrid : MeshObject
  {
    private Camera editorCamera; 

    public SceneGrid(Camera editorCamera)
    {
      Model = new Model();
      this.editorCamera = editorCamera;

      string vertexShader = @"
        #version 450 core
        layout (location = 0) in vec3 a_Position;
        uniform mat4 u_viewMatrix;
        uniform mat4 u_projectionMatrix;

        void main()
        {
          gl_Position = vec4(a_Position, 1.0) * u_viewMatrix * u_projectionMatrix; 
        }
      ";

      string fragmentShader = @"
        void main() {
          gl_FragColor = vec4(0.75, 0.75, 0.75, 0.75);
        }
      ";

      Shader lineShader = new Shader(vertexShader, fragmentShader);
      Material lineMaterial = new Material(lineShader);

      lineMaterial.PrimitiveType = PrimitiveType.Lines;
      Material = lineMaterial;
      UpdateGrid();
    }

    public void UpdateGrid()
    {
      List<float> lines = new List<float>();
      List<int> indices = new List<int>();

      string cameraHeightString = Math.Abs((float)Math.Round(editorCamera.Transform.Position.Y)).ToString();
      int length = cameraHeightString.Length;

      float nearestMajorValue = (float)Math.Pow(10f, (float)length + 1);
      float extents = nearestMajorValue;
      for (int x = 0; x < 20; x++)
      {
        float xPosition = (float)x / 20 * extents * 2 - extents;
        lines.Add(xPosition);
        lines.Add(0f);
        lines.Add(extents);
        lines.Add(xPosition);
        lines.Add(0f);
        lines.Add(-extents);

        indices.Add(x * 2);
        indices.Add(x * 2 + 1);
      }
      for (int x = 0; x < 20; x++)
      {
        float zPosition = (float)x / 20 * extents * 2 - extents;
        lines.Add(extents);
        lines.Add(0f);
        lines.Add(zPosition);
        lines.Add(-extents);
        lines.Add(0f);
        lines.Add(zPosition);

        indices.Add(40 + (int)x * 2);
        indices.Add(40 + (int)x * 2 + 1);
      }
      Model.Vertices = lines.ToArray();
      Model.Indices = indices.ToArray();
    }
  }
}
