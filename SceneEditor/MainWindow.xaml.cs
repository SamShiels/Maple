using Firefly;
using Firefly.Core;
using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.Utilities;
using Firefly.World;
using Firefly.World.Lighting;
using Firefly.World.Mesh;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SceneEditor.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SceneEditor
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private RendererManager rendererManager;
    private SceneManager sceneManager;
    private CameraController cameraController;
    private SceneGrid sceneGrid;
    private WorldObject selectedObject;

    private Model cube;
    private Material material;
    private Texture texture;

    public MainWindow()
    {
      InitializeComponent();

      GLWpfControlSettings settings = new GLWpfControlSettings {
        MajorVersion = 4,
        MinorVersion = 6,
      };

      OpenTkControl.Start(settings);

      OBJLoader loader = new OBJLoader();
      rendererManager = new RendererManager(OpenTkControl.Framebuffer);
      sceneManager = new SceneManager(treeView);

      Camera camera = new Camera();
      SceneGrid sceneGrid = new SceneGrid(camera);

      sceneManager.AddEditorObject(sceneGrid);

      sceneManager.Scene.AddObject(camera);
      sceneManager.AssignCamera(camera);

      cameraController = new CameraController(camera, OpenTkControl, sceneGrid);

      cube = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.house.obj"));

      Uniform ambientLight = new Uniform("u_ambientLight", new Vector3(0.2f, 0.2f, 0.2f));
      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[3] { ambientLight, directionalLight, shininess };

      material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);
      Firefly.Texturing.Image house = new Firefly.Texturing.Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.emu_face.jpg"));
      texture = new Texture(house);

      MeshObject cubeMesh = sceneManager.CreateObject<MeshObject>();
      cubeMesh.Model = cube;
      cubeMesh.Transform.Position = new Vector3(0f, 0f, 0f);
      cubeMesh.Transform.LocalScale = new Vector3(2f, 2f, 2f);
      cubeMesh.Textures = new Texture[] { texture };
      cubeMesh.Material = material;

      PointLight light = sceneManager.CreateObject<PointLight>();
      light.Transform.Position = new Vector3(0f, 3f, 2f);
      light.Diffuse = Color4.White;
      light.Radius = 10f;
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
      rendererManager.Render(sceneManager.Scene);
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      rendererManager.Resize(OpenTkControl.FrameBufferWidth, OpenTkControl.FrameBufferHeight);
    }

    private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      TreeViewItem item = (TreeViewItem)e.NewValue;
      sceneManager.NewItemSelected(item);
      selectedObject = (WorldObject)item.DataContext;

      xPos.DataContext = selectedObject.Transform.Position;
      yPos.DataContext = selectedObject.Transform.Position;
      zPos.DataContext = selectedObject.Transform.Position;

      xRot.DataContext = selectedObject.Transform.Rotation;
      yRot.DataContext = selectedObject.Transform.Rotation;
      zRot.DataContext = selectedObject.Transform.Rotation;

      xScale.DataContext = selectedObject.Transform.LocalScale;
      yScale.DataContext = selectedObject.Transform.LocalScale;
      zScale.DataContext = selectedObject.Transform.LocalScale;
    }

    #region Transform Event Handlers
    private void xPosChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float yPos = selectedObject.Transform.Position.Y;
      float zPos = selectedObject.Transform.Position.Z;
      selectedObject.Transform.Position = new Vector3(newValue, yPos, zPos);
    }

    private void yPosChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float xPos = selectedObject.Transform.Position.X;
      float zPos = selectedObject.Transform.Position.Z;
      selectedObject.Transform.Position = new Vector3(xPos, newValue, zPos);
    }

    private void zPosChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float xPos = selectedObject.Transform.Position.X;
      float yPos = selectedObject.Transform.Position.Y;
      selectedObject.Transform.Position = new Vector3(xPos, yPos, newValue);
    }

    private void xRotChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float yRot = selectedObject.Transform.Rotation.Y;
      float zRot = selectedObject.Transform.Rotation.Z;
      selectedObject.Transform.Rotation = new Vector3(newValue, yRot, zRot);
    }

    private void yRotChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float xRot = selectedObject.Transform.Rotation.X;
      float zRot = selectedObject.Transform.Rotation.Z;
      selectedObject.Transform.Rotation = new Vector3(xRot, newValue, zRot);
    }

    private void zRotChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float xRot = selectedObject.Transform.Rotation.X;
      float yRot = selectedObject.Transform.Rotation.Y;
      selectedObject.Transform.Rotation = new Vector3(xRot, yRot, newValue);
    }

    private void xScaleChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float yScale = selectedObject.Transform.LocalScale.Y;
      float zScale = selectedObject.Transform.LocalScale.Z;
      selectedObject.Transform.LocalScale = new Vector3(newValue, yScale, zScale);
    }

    private void yScaleChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float xScale = selectedObject.Transform.LocalScale.X;
      float zScale = selectedObject.Transform.LocalScale.Z;
      selectedObject.Transform.LocalScale = new Vector3(xScale, newValue, zScale);
    }

    private void zScaleChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      TextBox textBox = (TextBox)sender;
      float newValue = float.Parse(textBox.Text.Length > 0 ? textBox.Text : "0");

      float xScale = selectedObject.Transform.LocalScale.X;
      float yScale = selectedObject.Transform.LocalScale.Y;
      selectedObject.Transform.LocalScale = new Vector3(xScale, yScale, newValue);
    }

    #endregion

    private void Create_Container(object sender, RoutedEventArgs e)
    {

    }

    private void Create_Mesh(object sender, RoutedEventArgs e)
    {
      MeshObject newMesh = sceneManager.CreateObject<MeshObject>();
      newMesh.Model = cube;
      newMesh.Transform.Position = new Vector3(2f, 0f, -5f);
      newMesh.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      newMesh.Textures = new Texture[] { texture };
      newMesh.Material = material;
    }
  }
}
