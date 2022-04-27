using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Shader
{

  public static class UniformDataHelper
  {
    public static void UploadUniformData(int location, object data)
    {
      Type dataType = data.GetType();

      if (dataType.Equals(typeof(int)))
      {
        GL.Uniform1(location, (int)Convert.ChangeType(data, typeof(int)));
      } 
      else if(dataType.Equals(typeof(float)))
      {
        GL.Uniform1(location, (float)Convert.ChangeType(data, typeof(float)));
      }
      else if(dataType.Equals(typeof(Vector2)))
      {
        GL.Uniform2(location, (Vector2)Convert.ChangeType(data, typeof(Vector2)));
      }
      else if(dataType.Equals(typeof(Vector3)))
      {
        GL.Uniform3(location, (Vector3)Convert.ChangeType(data, typeof(Vector3)));
      }
      else if(dataType.Equals(typeof(Vector4)))
      {
        GL.Uniform4(location, (Vector4)Convert.ChangeType(data, typeof(Vector4)));
      }
      else if(dataType.Equals(typeof(Matrix2)))
      {
        Matrix2 mat2 = (Matrix2)Convert.ChangeType(data, typeof(Matrix2));
        GL.UniformMatrix2(location, false, ref mat2);
      }
      else if(dataType.Equals(typeof(Matrix3)))
      {
        Matrix3 mat3 = (Matrix3)Convert.ChangeType(data, typeof(Matrix3));
        GL.UniformMatrix3(location, false, ref mat3);
      }
      else if (dataType.Equals(typeof(Matrix4)))
      {
        Matrix4 mat4 = (Matrix4)Convert.ChangeType(data, typeof(Matrix4));
        GL.UniformMatrix4(location, false, ref mat4);
      }
      else if (dataType.Equals(typeof(bool)))
      {
        if ((bool)data == true)
        {
          GL.Uniform1(location, 1);
        } else
        {
          GL.Uniform1(location, 0);
        }
      }
    }
  }
}
