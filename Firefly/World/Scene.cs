using Firefly.Core;
using Firefly.World.Lighting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World
{
  public class Scene
  {

    public WorldObject RootObject
    {
      get; private set;
    }

    public List<PointLight> Lights
    {
      get; private set;
    }

    public Scene()
    {
      RootObject = new WorldObject();
      Lights = new List<PointLight>();
    }

    public void AddObject(WorldObject worldObject)
    {
      RootObject.Transform.AddChild(worldObject.Transform);
    }

    public void AddObject(PointLight pointLight)
    {
      RootObject.Transform.AddChild(pointLight.Transform);
      Lights.Add(pointLight);
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
  }
}
