using Firefly.World.Scene.SceneDataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Scene.FactoryMethods
{
  internal interface ISceneObjectFactory
  {
    WorldObject Instantiate(Assets sceneAssets, SceneDataModels.WorldObject parent);
  }
}
