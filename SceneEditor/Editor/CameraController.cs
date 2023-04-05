using Firefly.World;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SceneEditor.Editor
{
  internal class CameraController
  {
    private Camera camera;
    private GLWpfControl UIElement;

    private SceneGrid sceneGrid;

    private Point lastMousePosition;
    private bool looking = false;

    internal CameraController(Camera camera, GLWpfControl UIElement, SceneGrid sceneGrid)
    {
      this.camera = camera;
      this.UIElement = UIElement;
      lastMousePosition = new Point();
      this.sceneGrid = sceneGrid;

      camera.Transform.Position = new Vector3(-8, 10, -8);
      camera.Transform.Rotation = new Vector3(-(float)Math.PI / 4f, (float)Math.PI / 4f, 0.0f);

      Mouse.AddMouseWheelHandler(UIElement, MouseWheelEventHandler);
      Mouse.AddMouseMoveHandler(UIElement, MouseMoveEventHandler);
      Mouse.AddMouseDownHandler(UIElement, MouseButtonEventHandler);
      Mouse.AddMouseUpHandler(UIElement, MouseButtonEventHandler);
    }

    private void MouseWheelEventHandler(object sender, MouseWheelEventArgs e)
    {
      Transform cameraTransform = camera.Transform;

      float scrollAmount = e.Delta / 200f;

      float xPos = cameraTransform.Position.X;
      float yPos = cameraTransform.Position.Y;
      float zPos = cameraTransform.Position.Z;

      float xRot = cameraTransform.Rotation.X;
      float yRot = cameraTransform.Rotation.Y;
      float zRot = cameraTransform.Rotation.Z;

      Vector3 direction = new Vector3(0.0f, 0.0f, scrollAmount);

      Matrix4X4<float> matrix = Matrix4X4<float>.Identity;
      matrix = matrix * Matrix4X4<float>.CreateRotationX(-xRot);
      matrix = matrix * Matrix4X4<float>.CreateRotationY(yRot);
      matrix = matrix * Matrix4X4<float>.CreateRotationZ(zRot);
      direction = Vector3.TransformVector(direction, matrix);

      cameraTransform.Position = new Vector3(xPos + direction.X, yPos + direction.Y, zPos + direction.Z);
      sceneGrid.UpdateGrid();
    }

    private void MouseMoveEventHandler(object sender, MouseEventArgs e)
    {
      if (!looking)
      {
        return;
      }
      Transform cameraTransform = camera.Transform;
      Point currentPosition = e.GetPosition(UIElement);

      Point delta = new Point(lastMousePosition.X - currentPosition.X, lastMousePosition.Y - currentPosition.Y);

      float xRot = Math.Min(Math.Max(cameraTransform.Rotation.X, -(float)Math.PI / 2f + 0.01f), (float)Math.PI / 2f - 0.01f);
      float yRot = cameraTransform.Rotation.Y;
      cameraTransform.Rotation = new Vector3(xRot + (float)delta.Y / 100f, yRot + (float)delta.X / 100f, 0.0f);

      lastMousePosition = new Point(currentPosition.X, currentPosition.Y);
    }

    public void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
    {
      looking = e.MiddleButton == MouseButtonState.Pressed;
      Point currentPosition = e.GetPosition(UIElement);
      lastMousePosition = new Point(currentPosition.X, currentPosition.Y);
    }
  }
}
