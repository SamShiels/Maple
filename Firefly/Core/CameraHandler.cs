using Firefly.Utilities;
using Firefly.World;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Firefly.Core
{
  internal class CameraHandler
  {
    private uint lastCameraId;
    private uint lastCameraDirtyId;
    private float lastAspectRatio;

    private float lastPositionX;
    private float lastPositionY;
    private float lastPositionZ;

    private float lastRotationX;
    private float lastRotationY;
    private float lastRotationZ;

    private Matrix4X4<float> projectionMatrix;
    private Matrix4X4<float> viewMatrix;

    private bool initialized;

    public Camera camera;

    public CameraHandler()
    {
      initialized = false;
    }

    public void AssignCamera(Camera camera)
    {
      this.camera = camera;
    }

    public Matrix4X4<float> GetProjectionMatrix(float aspectRatio)
    {
      if (camera == null)
      {
        Console.WriteLine("No camera has been assigned!");
        return Matrix4X4<float>.Identity;
      }

      if (lastCameraId != camera.Id || lastCameraDirtyId != camera.DirtyId || lastAspectRatio != aspectRatio)
      {
        ProjectionType projectionType = camera.ProjectionType;
        if (projectionType == ProjectionType.Perspective)
        {
          projectionMatrix = Projection.CreatePerspectiveMatrix(camera.FieldOfView, aspectRatio, camera.NearClipPlane, camera.FarClipPlane);
        }
        else if (projectionType == ProjectionType.Orthographic)
        {
          projectionMatrix = Projection.CreateOrthographicMatrix(camera.OrthographicSize, aspectRatio, camera.NearClipPlane, camera.FarClipPlane);
        }

        lastCameraId = camera.Id;
        lastCameraDirtyId = camera.DirtyId;
        lastAspectRatio = aspectRatio;
      }

      return projectionMatrix;
    }

    public Matrix4X4<float> GetViewMatrix()
    {
      if (camera == null)
      {
        Console.WriteLine("No camera has been assigned!");
        return Matrix4X4<float>.Identity;
      }

      float xPosition = camera.Transform.Position.X;
      float yPosition = camera.Transform.Position.Y;
      float zPosition = camera.Transform.Position.Z;

      float xRotation = camera.Transform.Rotation.X;
      float yRotation = camera.Transform.Rotation.Y;
      float zRotation = camera.Transform.Rotation.Z;

      if (!initialized || (xPosition != lastPositionX || yPosition != lastPositionY || zPosition != lastPositionZ || xRotation != lastRotationX || yRotation != lastRotationY || zRotation != lastRotationZ))
      {

        Vector3D<float> front = new Vector3D<float>(
          (float)System.Math.Cos(xRotation) * (float)System.Math.Sin(yRotation),
          (float)System.Math.Sin(xRotation),
          (float)System.Math.Cos(xRotation) * (float)System.Math.Cos(yRotation)
          );

        front = Vector3D.Normalize(front);
        Vector3D<float> rightAxis = Vector3D.Normalize(Vector3D.Cross(front, new Vector3D<float>(0, 1f, 0)));
        Vector3D<float> upAxis = Vector3D.Normalize(Vector3D.Cross(rightAxis, front));

        //viewMatrix = Matrix4X4<float>.LookAt(camera.Transform.Position, camera.Transform.Position + front, upAxis);
        //viewMatrix = Matrix4X4<float>.CreateRotationZ(-zRotation) * viewMatrix;
        viewMatrix = camera.Transform.GetLocalMatrix().Inverted();
        //viewMatrix = Matrix4X4<float>.Mult(viewMatrix, camera.Transform.GetLocalMatrix());

        lastPositionX = xPosition;
        lastPositionY = yPosition;
        lastPositionZ = zPosition;

        lastRotationX = xRotation;
        lastRotationY = yRotation;
        lastRotationZ = zRotation;
      }

      //Matrix4X4<float> viewMatrix = new Matrix4X4<float>(
      //    rightAxis.X,        rightAxis.Y,        rightAxis.Z,        0f,
      //    upAxis.X,           upAxis.Y,           upAxis.Z,           0f,
      //    front.X,            front.Y,            front.Z,            0f,
      //    0f,                 0f,                 0f,                 1f
      //  );

      //Matrix4X4<float> translationMatrix = new Matrix4X4<float>(
      //    1f, 0f, 0f, xPosition,
      //    0f, 1f, 0f, yPosition,
      //    0f, 0f, 1f, zPosition,
      //    0f, 0f, 0f, 1f
      //  );

      //return Matrix4X4<float>.Mult(viewMatrix, translationMatrix);
      return viewMatrix;
    }
  }
}
