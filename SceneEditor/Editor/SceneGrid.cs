﻿using Maple.Rendering;
using Maple.World;
using Maple.World.Mesh;
using OpenTK.Mathematics;
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
        uniform mat4 u_modelMatrix;
        uniform mat4 u_viewMatrix;
        uniform mat4 u_projectionMatrix;

        void main()
        {
          gl_Position = vec4(a_Position, 1.0) * u_modelMatrix * u_viewMatrix * u_projectionMatrix; 
        }
      ";

      string fragmentShader = @"
        void main() {
          gl_FragColor = vec4(0.75, 0.75, 0.75, 0.75);
        }
      ";

      Shader lineShader = new Shader(vertexShader, fragmentShader);
      Material lineMaterial = new Material(lineShader);

      lineMaterial.DepthFunction = DepthFunction.Lequal;
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

      float nearestMajorValue = (float)Math.Pow(10f, (float)length + 1) / 2;
      float extents = nearestMajorValue;
      int lineCount = 100;

      float displacementRoundingPoint = nearestMajorValue / (float)lineCount * 2;

      float xDisplacement = (float)Math.Floor(editorCamera.Transform.Position.X / displacementRoundingPoint) * displacementRoundingPoint;
      float zDisplacement = (float)Math.Floor(editorCamera.Transform.Position.Z / displacementRoundingPoint) * displacementRoundingPoint;

      Transform.Position = new Vector3(xDisplacement, 0.0f, zDisplacement);
      // Scale the grid depending on how far the camera is above the xz plane

      for (int x = 0; x < lineCount; x++)
      {
        float xPosition = (float)x / lineCount * extents * 2 - extents;
        lines.Add(xPosition);
        lines.Add(0f);
        lines.Add(extents);
        lines.Add(xPosition);
        lines.Add(0f);
        lines.Add(-extents);

        indices.Add(x * 2);
        indices.Add(x * 2 + 1);
      }
      for (int z = 0; z < lineCount; z++)
      {
        float zPosition = (float)z / lineCount * extents * 2 - extents;
        lines.Add(extents);
        lines.Add(0f);
        lines.Add(zPosition);
        lines.Add(-extents);
        lines.Add(0f);
        lines.Add(zPosition);

        indices.Add(lineCount * 2 + z * 2);
        indices.Add(lineCount * 2 + z * 2 + 1);
      }
      Model.Vertices = lines.ToArray();
      Model.Indices = indices.ToArray();
    }
  }
}
