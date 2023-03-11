using Firefly.World.Scene.SceneDataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Firefly.World.Scene
{
  internal class SceneLoader
  {

    internal SceneLoader()
    {
    }

    internal SceneObject CreateScene(string jsonString)
    {
      SceneFile scenefile = JsonSerializer.Deserialize<SceneFile>(jsonString);
      SceneObject newScene = new SceneObject();

      Camera camera = new Camera();
      //camera.FieldOfView = scenefile.camera.fov;

      return newScene;
    }
  }
}
