using Maple.Utilities;
using Maple.World;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
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

    private Matrix4 projectionMatrix;
    private Matrix4 viewMatrix;

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

    public Matrix4 GetProjectionMatrix(float aspectRatio)
    {
      if (camera == null)
      {
        Console.WriteLine("No camera has been assigned!");
        return Matrix4.Identity;
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

    public Matrix4 GetViewMatrix()
    {
      if (camera == null)
      {
        Console.WriteLine("No camera has been assigned!");
        return Matrix4.Identity;
      }

      float xPosition = camera.Transform.Position.X;
      float yPosition = camera.Transform.Position.Y;
      float zPosition = camera.Transform.Position.Z;

      float xRotation = camera.Transform.Rotation.X;
      float yRotation = camera.Transform.Rotation.Y;
      float zRotation = camera.Transform.Rotation.Z;

      if (!initialized || (xPosition != lastPositionX || yPosition != lastPositionY || zPosition != lastPositionZ || xRotation != lastRotationX || yRotation != lastRotationY || zRotation != lastRotationZ))
      {

        Vector3 front = new Vector3(
          (float)System.Math.Cos(xRotation) * (float)System.Math.Sin(yRotation),
          (float)System.Math.Sin(xRotation),
          (float)System.Math.Cos(xRotation) * (float)System.Math.Cos(yRotation)
          );

        front.Normalize();
        Vector3 rightAxis = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
        Vector3 upAxis = Vector3.Normalize(Vector3.Cross(rightAxis, front));

        //viewMatrix = Matrix4.LookAt(camera.Transform.Position, camera.Transform.Position + front, upAxis);
        //viewMatrix = Matrix4.CreateRotationZ(-zRotation) * viewMatrix;
        viewMatrix = camera.Transform.GetLocalMatrix().Inverted();
        //viewMatrix = Matrix4.Mult(viewMatrix, camera.Transform.GetLocalMatrix());

        lastPositionX = xPosition;
        lastPositionY = yPosition;
        lastPositionZ = zPosition;

        lastRotationX = xRotation;
        lastRotationY = yRotation;
        lastRotationZ = zRotation;
      }

      //Matrix4 viewMatrix = new Matrix4(
      //    rightAxis.X,        rightAxis.Y,        rightAxis.Z,        0f,
      //    upAxis.X,           upAxis.Y,           upAxis.Z,           0f,
      //    front.X,            front.Y,            front.Z,            0f,
      //    0f,                 0f,                 0f,                 1f
      //  );

      //Matrix4 translationMatrix = new Matrix4(
      //    1f, 0f, 0f, xPosition,
      //    0f, 1f, 0f, yPosition,
      //    0f, 0f, 1f, zPosition,
      //    0f, 0f, 0f, 1f
      //  );

      //return Matrix4.Mult(viewMatrix, translationMatrix);
      return viewMatrix;
    }
  }
}
