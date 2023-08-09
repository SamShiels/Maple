using Maple.Core;
using Maple.World.Lighting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.World.Scene
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

    public List<DirectionalLight> DirectionalLights
    {
      get; private set;
    }

    public SceneObject()
    {
      RootObject = new WorldObject();
      Cameras = new List<Camera>();
      Lights = new List<PointLight>();
      DirectionalLights = new List<DirectionalLight>();
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

      if (worldObject.GetType() == typeof(PointLight))
      {
        Lights.Add((PointLight)worldObject);
      }

      if (worldObject.GetType() == typeof(DirectionalLight))
      {
        DirectionalLights.Add((DirectionalLight)worldObject);
      }

      if (worldObject.GetType() == typeof(Camera))
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
