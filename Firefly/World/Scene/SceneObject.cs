using Firefly.Core;
using Firefly.World.Lighting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene
{
  public class SceneObject
  {
    public WorldObject RootObject
    {
      get; private set;
    }

    public Camera Camera
    {
      get; private set;
    }

    public List<Camera> Cameras
    {
      get; private set;
    }

    public List<PointLight> Lights
    {
      get; private set;
    }

    public SceneObject()
    {
      RootObject = new WorldObject();
      Cameras = new List<Camera>();
      Lights = new List<PointLight>();
    }

    public void AddObject(WorldObject worldObject, WorldObject parent = null)
    {
      if (parent == null)
      {
        RootObject.Transform.AddChild(worldObject.Transform);
      }
      else
      {
        parent.Transform.AddChild(worldObject.Transform);
      }

      if (worldObject.GetType().Name == "PointLight")
      {
        Lights.Add((PointLight)worldObject);
      }

      if (worldObject.GetType().Name == "Camera")
      {
        Cameras.Add((Camera)worldObject);
      }
    }

    public void AssignCamera(Camera camera)
    {
      Camera = camera;
    }

    public void RemoveObject(WorldObject worldObject)
    {
      RootObject.Transform.RemoveChild(worldObject.Transform);
    }

    public void RemoveObject(PointLight pointLight)
    {
      RootObject.Transform.RemoveChild(pointLight.Transform);
      Lights.Remove(pointLight);
    }

    public void RemoveObject(Camera camera)
    {
      RootObject.Transform.RemoveChild(camera.Transform);
      Cameras.Remove(camera);
    }
  }
}
