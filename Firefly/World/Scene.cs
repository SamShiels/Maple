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

    public void RemoveObject(WorldObject worldObject)
    {
      RootObject.Transform.RemoveChild(worldObject.Transform);
    }

    public void AddLight(PointLight light)
    {
      Lights.Add(light);
    }

    public void RemoveLight(PointLight light)
    {
      Lights.Remove(light);
    }
  }
}
