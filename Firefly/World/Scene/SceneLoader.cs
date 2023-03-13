using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.World.Mesh;
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

		private OBJLoader objLoader;

		private Dictionary<string, Model> models = new Dictionary<string, Model>();
		private Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
		private Dictionary<string, Rendering.Material> materials = new Dictionary<string, Rendering.Material>();
		private Dictionary<string, Texturing.Cubemap> cubemaps = new Dictionary<string, Texturing.Cubemap>();

		public SceneLoader(Assembly executingAssembly)
		{
			objLoader = new OBJLoader();
			this.executingAssembly = executingAssembly;
			assemblyName = executingAssembly.GetName().Name;
		}

		public SceneObject CreateScene(Stream sceneStream)
		{
			using (StreamReader reader = new StreamReader(sceneStream))
			{
				string jsonContent = reader.ReadToEnd();

				// Parse the json scene object
				SceneFile sceneFile = JsonSerializer.Deserialize<SceneFile>(jsonContent);
				// Declare a new scene
				SceneObject newScene = new SceneObject();

				// Set up the camera
				Camera camera = new Camera();
				SceneDataModels.Camera sceneFileCamera = sceneFile.camera;
				// 0 = perspective. 1 = orthographic.
				if (sceneFileCamera.projectionType == 0)
				{
					camera.ProjectionType = Utilities.ProjectionType.Perspective;
				}
				else
				{
					camera.ProjectionType = Utilities.ProjectionType.Orthographic;
				}

				// Assign FOV and clip plane distances
				camera.FieldOfView = sceneFileCamera.fov;
				camera.FarClipPlane = sceneFileCamera.farClip;
				camera.NearClipPlane = sceneFileCamera.nearClip;

				// Position and rotation
				camera.Transform.Position = new Vector3(sceneFileCamera.position[0], sceneFileCamera.position[1], sceneFileCamera.position[2]);
				camera.Transform.EulerAngles = new Vector3(sceneFileCamera.rotation[0], sceneFileCamera.rotation[1], sceneFileCamera.rotation[2]);

				// Create the skybox cubemap. If it exists
				if (sceneFileCamera.skybox != null && sceneFileCamera.skybox.Length > 0)
				{
					camera.Skybox = GetCubemap(cubemaps, sceneFile.cubemaps, sceneFileCamera.skybox);
				}

				newScene.AssignCamera(camera);

				// Insntantiate scene object using the hierarchy given in the scene file
				if (sceneFile.worldObjects != null)
				{
					for (int i = 0; i < sceneFile.worldObjects.Count; i++)
					{
						SceneDataModels.WorldObject worldObject = sceneFile.worldObjects[i];
						AddObject(newScene, sceneFile.materials, worldObject, null);
					}
				}

				return newScene;
			}
		}

		private void AddObject(SceneObject scene, List<SceneDataModels.Material> sceneMaterials, SceneDataModels.WorldObject worldObject, WorldObject parent = null)
		{
			WorldObject newObject;
			// Each type of world object has different properties
			if (worldObject.type == "MeshObject")
			{
				newObject = InstantiateMeshObject(sceneMaterials, worldObject);
			} else
      {
				newObject = InstantiateMeshObject(sceneMaterials, worldObject);
			}

			newObject.Transform.Position = new Vector3(worldObject.position[0], worldObject.position[1], worldObject.position[2]);
			newObject.Transform.EulerAngles = new Vector3(worldObject.rotation[0], worldObject.rotation[1], worldObject.rotation[2]);
			newObject.Transform.LocalScale = new Vector3(worldObject.localScale[0], worldObject.localScale[1], worldObject.localScale[2]);

			// Add the new object to our scene
			scene.AddObject(newObject, parent);
			for (int i = 0; i < worldObject.children.Count; i++)
      {
				// Recursively add each child to the scene
				SceneDataModels.WorldObject child = worldObject.children[i];
				AddObject(scene, sceneMaterials, child, newObject);
			}
		}

		private MeshObject InstantiateMeshObject(List<SceneDataModels.Material> sceneMaterials, SceneDataModels.WorldObject worldObject)
    {
			MeshObject meshObject = new MeshObject();

			JsonElement meshObjectProperties = worldObject.properties;
			string modelName = meshObjectProperties.GetProperty("modelName").ToString();
			JsonElement textureElement = meshObjectProperties.GetProperty("textures");
			int textureCount = textureElement.GetArrayLength();

			List<string> textureNames = new List<string>();
			for (int i = 0; i < textureCount; i++)
      {
				string textureName = textureElement[i].ToString();
				textureNames.Add(textureName);
			}

			List<Texture> meshTextures = new List<Texture>();
			for (int i = 0; i < textureNames.Count; i++)
			{
				// Get the textures by name
				Texture texture = GetTexture(textures, textureNames[i]);
				meshTextures.Add(texture);
			}

			// Assign textures to the object
			meshObject.Textures = meshTextures.ToArray();

			string materialName = meshObjectProperties.GetProperty("material").ToString();

			// Assign the model to the new world object
			Model model = GetModel(models, modelName);
			meshObject.Model = model;

			// Get the material and assign it
			Rendering.Material material = GetMaterial(materials, sceneMaterials, materialName);
			meshObject.Material = material;

			return meshObject;
		}

		internal Model GetModel(Dictionary<string, Model> models, string modelName)
		{
			if (models.TryGetValue(modelName, out Model model))
			{
				return model;
			}
			Stream modelStream = executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{modelName}");
			// Create a new model by this name, and cache it
			Model newModel = objLoader.Load(modelStream);
			models.Add(modelName, newModel);

			return newModel;
		}

		internal Texture GetTexture(Dictionary<string, Texture> textures, string imageName)
		{
			if (textures.TryGetValue(imageName, out Texture texture))
			{
				return texture;
			}
			// Create a new texture by this image name, and cache it
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

			// Grab the images from the scene data using the cubemap name
			SceneDataModels.Cubemap sceneCubemap = sceneCubemaps.Find((cubemap) => cubemap.name == name);
			string posx = sceneCubemap.right;
			string negx = sceneCubemap.left;
			string posy = sceneCubemap.top;
			string negy = sceneCubemap.bottom;
			string posz = sceneCubemap.front;
			string negz = sceneCubemap.back;

			// Load up the images
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

		internal Rendering.Material GetMaterial(Dictionary<string, Rendering.Material> materials, List<SceneDataModels.Material> sceneMaterials, string name)
		{
			if (materials.TryGetValue(name, out Rendering.Material material))
			{
				return material;
			}

			// Use the name of the material to obtain the material from the scene data
			SceneDataModels.Material sceneMaterial = sceneMaterials.Find((cubemap) => cubemap.name == name);
			string vertexShaderName = $"{sceneMaterial.shaderName}.vert";
			string fragmentShaderName = $"{sceneMaterial.shaderName}.frag";

			// Grab the shader files from the assembly
			Stream vertexShaderStream = executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{vertexShaderName}");
			Stream fragmentShaderStream = executingAssembly.GetManifestResourceStream($"{assemblyName}.Resources.{fragmentShaderName}");

			string vertexShaderSource = "";
			string fragmentShaderSource = "";

			// Stream the vertex and fragment shaders into memory
			using (StreamReader reader = new StreamReader(vertexShaderStream))
			{
				vertexShaderSource = reader.ReadToEnd();
			}

			using (StreamReader reader = new StreamReader(fragmentShaderStream))
			{
				fragmentShaderSource = reader.ReadToEnd();
			}

			Shader shader = new Shader(vertexShaderSource, fragmentShaderSource);

			// Map the uniform data into an array fit for the new material object
			Uniform[] uniforms = null;
			if (sceneMaterial.uniforms.Count > 0)
      {
				uniforms = new Uniform[materials.Count];
				for (int i = 0; i < sceneMaterial.uniforms.Count; i++)
        {
					MaterialUniform sceneUniform = sceneMaterial.uniforms[i];
					uniforms[i] = new Uniform(sceneUniform.name, sceneUniform.value);
				}
			}

			// Create the material and cache it for later
			Rendering.Material newMaterial = new Rendering.Material(shader, uniforms);
			materials.Add(name, newMaterial);

			return newMaterial;
		}
	}
}
