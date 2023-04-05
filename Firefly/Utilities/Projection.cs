using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Utilities
{
  public class Projection
  {

    public static Matrix4X4<float> CreatePerspectiveMatrix(float verticalFOV, float aspect, float near, float far)
    {
      float range = near - far;
      float tanHalfFOV = (float)System.Math.Tan(verticalFOV / 2.0f);

      Matrix4X4<float> pers = Matrix4X4<float>.CreatePerspectiveFieldOfView(verticalFOV, aspect, near, far);

      //Matrix4X4<float> matrix = new Matrix4(
      //  1.0f / (tanHalfFOV * aspect),
      //  0.0f,
      //  0.0f,
      //  0.0f,

      //  0.0f,
      //  1.0f / tanHalfFOV,
      //  0.0f,
      //  0.0f,

      //  0.0f,
      //  0.0f,
      // (near + far) / range,
      // -1.0f,

      //  0.0f,
      //  0.0f,
      //  2.0f * far * near / range,
      //  0.0f
      //  );

      return pers;
    }

    public static Matrix4X4<float> CreateOrthographicMatrix(float orthoSize, float aspect, float near, float far)
    {
      float width = orthoSize * aspect;
      float height = orthoSize;

      float lr = 1 / width;
      float bt = 1 / height;
      float nf = 1 / (near - far);

      Matrix4X4<float> matrix = new Matrix4X4<float>(
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
