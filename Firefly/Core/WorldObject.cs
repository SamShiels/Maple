using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core
{
  public class WorldObject
  {
    public virtual string TYPE { get; protected set; } = "Container";

    public Transform Transform { get; private set; }
    public bool IsContainer { get; protected set; }

    public WorldObject()
    {
      Transform = new Transform(this);
      IsContainer = true;
    }

    public virtual void Destroy()
    {
      TYPE = null;
      if (Transform != null)
      {
        //Transform.Destroy();
        Transform = null;
      }
    }
  }
}
