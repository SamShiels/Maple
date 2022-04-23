using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renderer.Utilities
{
  public class Projection
  {

    public static Matrix4 CreatePerspectiveMatrix(float verticalFOV, float aspect, float near, float far)
    {
      float range = near - far;
      float tanHalfFOV = (float)Math.Tan(verticalFOV / 2.0f);

      Matrix4 matrix = new Matrix4(
        1.0f / (tanHalfFOV * aspect),
        0.0f,
        0.0f,
        0.0f,

        0.0f,
        1.0f / tanHalfFOV,
        0.0f,
        0.0f,

        0.0f,
        0.0f,
       (near + far) / range,
       -1.0f,

        0.0f,
        0.0f,
        2.0f * far * near / range,
        0.0f
        );

      return matrix;
    }

    public static Matrix4 CreateOrthographicMatrix(float orthoSize, float aspect, float near, float far)
    {
      float width = orthoSize * aspect;
      float height = orthoSize;

      float lr = 1 / width;
      float bt = 1 / height;
      float nf = 1 / (near - far);

      Matrix4 matrix = new Matrix4(
        2.0f * lr,
        0.0f,
        0.0f,
        0.0f,

        0.0f,
        2.0f * bt,
        0.0f,
        0.0f,

        0.0f,
        0.0f,
        2.0f * nf,
        0.0f,

        0.0f,
        0.0f,
       (far + near) * nf,
        1.0f
        );

      return matrix;
    }
  }
}
