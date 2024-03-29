﻿using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Utilities
{
  public class Math
  {

    public static Vector3 TransformPoint(Matrix4 matrix, Vector3 point)
    {
      float x = point[0];
      float y = point[1];
      float z = point[2];

      float w = matrix[3, 0] * x + matrix[3, 1] * y + matrix[3, 2] * z + matrix[3, 3];

      Vector3 pos = new Vector3(
        (matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] * z + matrix[0, 3] * 1) / w,
        (matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z + matrix[1, 3] * 1) / w,
        (matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2] * z + matrix[2, 3] * 1) / w);

      return pos;
    }

    public static (float, float, float) TransformPoint(Matrix4 matrix, float x, float y, float z)
    {
      float w = matrix[3, 0] * x + matrix[3, 1] * y + matrix[3, 2] * z + matrix[3, 3];

      float newX = (matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] * z + matrix[0, 3] * 1) / w;
      float newY = (matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z + matrix[1, 3] * 1) / w;
      float newZ = (matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2] * z + matrix[2, 3] * 1) / w;

      return (newX, newY, newZ);
    }

    public static (float, float, float) TransformPointTransposed(Matrix4 matrix, float x, float y, float z)
    {
      float w = matrix[0, 3] * x + matrix[1, 3] * y + matrix[2, 3] * z + matrix[3, 3];

      float newX = (matrix[0, 0] * x + matrix[1, 0] * y + matrix[2, 0] * z + matrix[3, 0] * 1) / w;
      float newY = (matrix[0, 0] * x + matrix[1, 1] * y + matrix[2, 1] * z + matrix[3, 1] * 1) / w;
      float newZ = (matrix[0, 0] * x + matrix[1, 2] * y + matrix[2, 2] * z + matrix[3, 2] * 1) / w;

      return (newX, newY, newZ);
    }
  }
}
