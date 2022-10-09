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
using SceneEditor.Editor.Controls;
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

    private Vector3Controls positionControls;
    private Vector3Controls rotationControls;
    private Vector3Controls localScaleControls;

    private SingleControl radiusControl;
    private SingleControl intensityControl;

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
      positionControls = new Vector3Controls(xPos, yPos, zPos);
      rotationControls = new Vector3Controls(xRot, yRot, zRot);
      localScaleControls = new Vector3Controls(xScale, yScale, zScale);
      radiusControl = new SingleControl(radius);
      intensityControl = new SingleControl(intensity);
      Settings.Visibility = Visibility.Hidden;
      PointLight.Visibility = Visibility.Hidden;

      cube = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.house.obj"));

      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[2] { directionalLight, shininess };

      material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);
      Firefly.Texturing.Image house = new Firefly.Texturing.Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.house.png"));
      texture = new Texture(house);

      MeshObject cubeMesh = sceneManager.CreateObject<MeshObject>();
      cubeMesh.Model = cube;
      cubeMesh.Transform.Position = new Vector3(0f, 0f, 0f);
      cubeMesh.Transform.LocalScale = new Vector3(2f, 2f, 2f);
      cubeMesh.Textures = new Texture[] { texture };
      cubeMesh.Material = material;

      PointLight light = sceneManager.CreateObject<PointLight>();
      light.Transform.Position = new Vector3(0f, 3f, 2f);
      light.Diffuse = Color4.Red;
      light.Radius = 2f;
      light.Intensity = 2f;
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
      Settings.Visibility = Visibility.Visible;
      TreeViewItem item = (TreeViewItem)e.NewValue;
      sceneManager.NewItemSelected(item);
      selectedObject = (WorldObject)item.DataContext;

      Vector3 Position = selectedObject.Transform.Position;
      positionControls.AttachData(Position);

      Vector3 Rotation = selectedObject.Transform.Rotation;
      rotationControls.AttachData(Rotation);

      Vector3 LocalScale = selectedObject.Transform.LocalScale;
      localScaleControls.AttachData(LocalScale);

      #region Point Light
      if (selectedObject.GetType() == typeof(PointLight))
      {
        PointLight pointLight = (PointLight)selectedObject;
        radiusControl.AttachData(new FloatBinding { value = pointLight.Radius });
        radiusControl.SetEnabled(true);
        intensityControl.AttachData(new FloatBinding { value = pointLight.Intensity });
        intensityControl.SetEnabled(true);
        PointLight.Visibility = Visibility.Visible;
      }
      else
      {
        radiusControl.SetEnabled(false);
        intensityControl.SetEnabled(false);
        PointLight.Visibility = Visibility.Hidden;
      }
      #endregion

      positionControls.SetEnabled(true);
      rotationControls.SetEnabled(true);
      localScaleControls.SetEnabled(true);
    }

    #region Transform Event Handlers
    private void xPosChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.Position = positionControls.xChangedEventHandler(sender, args);
    }

    private void yPosChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.Position = positionControls.yChangedEventHandler(sender, args);
    }

    private void zPosChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.Position = positionControls.zChangedEventHandler(sender, args);
    }

    private void xRotChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.Rotation = rotationControls.xChangedEventHandler(sender, args);
    }

    private void yRotChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.Rotation = rotationControls.yChangedEventHandler(sender, args);
    }

    private void zRotChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.Rotation = rotationControls.zChangedEventHandler(sender, args);
    }

    private void xScaleChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.LocalScale = localScaleControls.xChangedEventHandler(sender, args);
    }

    private void yScaleChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.LocalScale = localScaleControls.yChangedEventHandler(sender, args);
    }

    private void zScaleChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      selectedObject.Transform.LocalScale = localScaleControls.zChangedEventHandler(sender, args);
    }

    private void radiusChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      ((PointLight)selectedObject).Radius = radiusControl.ChangedEventHandler(sender, args).value;
    }

    private void intensityChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      ((PointLight)selectedObject).Intensity = intensityControl.ChangedEventHandler(sender, args).value;
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
