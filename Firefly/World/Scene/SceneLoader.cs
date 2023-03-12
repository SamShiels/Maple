using Firefly.Texturing;
using Firefly.World.Scene.SceneDataModels;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Firefly.World.Scene
{
  public class SceneLoader
  {
    private Assembly executingAssembly;
    private string assemblyName;

    private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
    private Dictionary<string, Texturing.Cubemap> cubemaps = new Dictionary<string, Texturing.Cubemap>();

    public SceneLoader(Assembly executingAssembly)
    {
      this.executingAssembly = executingAssembly;
      assemblyName = executingAssembly.GetName().Name;
    }

    public SceneObject CreateScene(Stream sceneStream)
    {
      using (StreamReader reader = new StreamReader(sceneStream))
      {
        string jsonContent = reader.ReadToEnd();

        SceneFile sceneFile = JsonSerializer.Deserialize<SceneFile>(jsonContent);
        SceneObject newScene = new SceneObject();

        Camera camera = new Camera();
        SceneDataModels.Camera sceneFileCamera = sceneFile.camera;
        if (sceneFileCamera.projectionType == 0)
        {
          camera.ProjectionType = Utilities.ProjectionType.Perspective;
        }
        else
        {
          camera.ProjectionType = Utilities.ProjectionType.Orthographic;
        }
        camera.FieldOfView = sceneFileCamera.fov;
        camera.FarClipPlane = sceneFileCamera.farClip;
        camera.NearClipPlane = sceneFileCamera.nearClip;

        camera.Transform.Position = new Vector3(sceneFileCamera.position[0], sceneFileCamera.position[1], sceneFileCamera.position[2]);
        camera.Transform.EulerAngles = new Vector3(sceneFileCamera.rotation[0], sceneFileCamera.rotation[1], sceneFileCamera.rotation[2]);

        camera.Skybox = GetCubemap(cubemaps, sceneFile.cubemaps, sceneFileCamera.skybox);

        newScene.AssignCamera(camera);

        return newScene;
      }
    }

    private void AddObject<T>(SceneObject scene, WorldObject<T> worldObject, WorldObject parent = null)
    {
      Type worldType = Type.GetType($"Firefly.World.{worldObject.type}");
      object newWorldObject = Activator.CreateInstance(worldType);


    }

    internal Texture GetTexture(Dictionary<string, Texture> textures, string imageName)
    {
      if (textures.TryGetValue(imageName, out Texture texture))
      {
        return texture;
      }
      Image image = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{imageName}"));
      Texture newTexture = new Texture(image);
      textures.Add(imageName, newTexture);

      return newTexture;
    }

    internal Texturing.Cubemap GetCubemap(Dictionary<string, Texturing.Cubemap> cubemaps, List<SceneDataModels.Cubemap> sceneCubemaps, string name)
    {
      if (cubemaps.TryGetValue(name, out Texturing.Cubemap cubemap))
      {
        return cubemap;
      }

      SceneDataModels.Cubemap sceneCubemap = sceneCubemaps.Find((cubemap) => cubemap.name == name);
      string posx = sceneCubemap.right;
      string negx = sceneCubemap.left;
      string posy = sceneCubemap.top;
      string negy = sceneCubemap.bottom;
      string posz = sceneCubemap.front;
      string negz = sceneCubemap.back;

      Image posxImage = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{name}.{posx}"));
      Image negxImage = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{name}.{negx}"));
      Image posyImage = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{name}.{posy}"));
      Image negyImage = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{name}.{negy}"));
      Image poszImage = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{name}.{posz}"));
      Image negzImage = new Image(executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{name}.{negz}"));
      Texturing.Cubemap newCubemap = new Texturing.Cubemap(posxImage, negxImage, posyImage, negyImage, poszImage, negzImage);
      cubemaps.Add(name, newCubemap);

      return newCubemap;
    }
  }
}
