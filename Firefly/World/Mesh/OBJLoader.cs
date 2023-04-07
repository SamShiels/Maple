﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firefly.World.Mesh
{
  public class OBJLoader
  {
    public Model Load(Stream stream)
    {
      List<float> positions = new List<float>();
      List<float> texcoords = new List<float>();
      List<float> normals = new List<float>();
      List<int> indices = new List<int>();

      List<float> positionCache = new List<float>();
      List<float> texcoordCache = new List<float>();
      List<float> normalCache = new List<float>();

      using (StreamReader reader = new StreamReader(stream))
      {
        string contents = reader.ReadToEnd();
        string[] lines = contents.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
          string line = lines[i];
          string[] parts = line.Split(' ');

          if (parts[0] == "v")
          {
            positionCache.Add(float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat));
            positionCache.Add(float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat));
            positionCache.Add(float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat));
          }
          else if (parts[0] == "vt")
          {
            texcoordCache.Add(float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat));
            texcoordCache.Add(float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat));
          }
          else if (parts[0] == "vn")
          {
            normalCache.Add(float.Parse(parts[1], CultureInfo.InvariantCulture.NumberFormat));
            normalCache.Add(float.Parse(parts[2], CultureInfo.InvariantCulture.NumberFormat));
            normalCache.Add(float.Parse(parts[3], CultureInfo.InvariantCulture.NumberFormat));
          }
          else if (parts[0] == "f")
          {
            int triangles = parts.Length - 3;

            for (int tri = 0; tri < triangles; tri++)
            {
              ExtractVertex(parts[1], positions, positionCache, texcoords, texcoordCache, normals, normalCache, indices);
              ExtractVertex(parts[tri + 2], positions, positionCache, texcoords, texcoordCache, normals, normalCache, indices);
              ExtractVertex(parts[tri + 3], positions, positionCache, texcoords, texcoordCache, normals, normalCache, indices);
            }
          }
        }
      }

      Model model = new Model();
      model.Vertices = positions.ToArray();
      model.Texcoords = texcoords.ToArray();
      model.Normals = normals.ToArray();
      model.Indices = indices.ToArray();

      return model;
    }

    private void ExtractVertex(string faceStringLine, List<float> modelPositions, List<float> positionCache, List<float> modelTexcoords, List<float> texcoordCache, List<float> modelNormals, List<float> normalCache, List<int> modelIndices)
    {
      string[] faceIndices = faceStringLine.Split('/');

      string positionIndexString = faceIndices[0];
      int positionIndex = (int.Parse(positionIndexString, CultureInfo.InvariantCulture.NumberFormat) - 1) * 3;
      modelPositions.AddRange(new float[] { positionCache[positionIndex], positionCache[positionIndex + 1], positionCache[positionIndex + 2] });

      string textureCoordinateIndexString = faceIndices[1];

      if (!string.IsNullOrEmpty(textureCoordinateIndexString))
      {
        int texcoordIndex = (int.Parse(textureCoordinateIndexString, CultureInfo.InvariantCulture.NumberFormat) - 1) * 2;
        modelTexcoords.AddRange(new float[] { texcoordCache[texcoordIndex], texcoordCache[texcoordIndex + 1] });
      }

      string normalIndexString = faceIndices[2];

      if (!string.IsNullOrEmpty(normalIndexString))
      {
        int normalIndex = (int.Parse(normalIndexString, CultureInfo.InvariantCulture.NumberFormat) - 1) * 3;
        modelNormals.AddRange(new float[] { normalCache[normalIndex], normalCache[normalIndex + 1], normalCache[normalIndex + 2] });
      }

      // Extract the vertex data from the caches and add it to our model lists
      modelIndices.Add(modelIndices.Count);
    }
  }
}
