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

    private Point lastMousePosition;
    private Boolean looking = false;

    internal CameraController(Camera camera, GLWpfControl UIElement)
    {
      this.camera = camera;
      this.UIElement = UIElement;
      lastMousePosition = new Point();

      Mouse.AddMouseWheelHandler(UIElement, MouseWheelEventHandler);
      Mouse.AddMouseMoveHandler(UIElement, MouseMoveEventHandler);
      Mouse.AddMouseDownHandler(UIElement, MouseButtonEventHandler);
      Mouse.AddMouseUpHandler(UIElement, MouseButtonEventHandler);
    }

    private void MouseWheelEventHandler(object sender, MouseWheelEventArgs e)
    {
      Transform cameraTransform = camera.Transform;

      float xPos = cameraTransform.Position.X;
      float yPos = cameraTransform.Position.Y;
      float zPos = cameraTransform.Position.Z;
      cameraTransform.Position = new Vector3(xPos, yPos, zPos + e.Delta / 400f);

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

      float xRot = cameraTransform.Rotation.X;
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
