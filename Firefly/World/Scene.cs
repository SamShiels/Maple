using Firefly.Core;
using Firefly.World.Lighting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World
{
  public class Scene
  {
    private WorldObject rootObject;
    private List<PointLight> lights;

    public void AddObject(WorldObject worldObject)
    {
      rootObject.Transform.AddChild(worldObject.Transform);
    }

    public void RemoveObject(WorldObject worldObject)
    {
      rootObject.Transform.RemoveChild(worldObject.Transform);
    }

    public void AddLight(PointLight light)
    {
      lights.Add(light);
    }

    public void RemoveLight(PointLight light)
    {
      lights.Remove(light);
    }
  }
}
