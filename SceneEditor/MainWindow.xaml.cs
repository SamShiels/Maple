using Firefly;
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

    private Model cube;
    private Material material;
    private Texture texture;

    private bool rendererInitialized;

    public MainWindow()
    {
      InitializeComponent();

      GLWpfControlSettings settings = new GLWpfControlSettings {
        MajorVersion = 4,
        MinorVersion = 6,
      };

      OpenTkControl.Start(settings);
      rendererInitialized = false;
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
      if (rendererInitialized == false)
      {
        rendererManager = new RendererManager(OpenTkControl.Framebuffer);
        sceneManager = new SceneManager(treeView);
        cube = OBJLoader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.cube.obj"));

        Uniform ambientLight = new Uniform("u_ambientLight", new Vector3(0.2f, 0.2f, 0.2f));
        Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
        Uniform[] uniforms = new Uniform[2] { ambientLight, directionalLight };

        material = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);
        Firefly.Texturing.Image house = new Firefly.Texturing.Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.emu_face.jpg"));
        texture = new Texture(house);

        MeshObject cubeMesh = sceneManager.CreateObject<MeshObject>();
        cubeMesh.Model = cube;
        cubeMesh.Transform.Position = new Vector3(-2f, 0f, -5f);
        cubeMesh.Transform.LocalScale = new Vector3(1f, 1f, 1f);
        cubeMesh.Textures = new Texture[] { texture };
        cubeMesh.Material = material;

        PointLight light = sceneManager.CreateObject<PointLight>();
        light.Transform.Position = new Vector3(0f, 3f, -2f);
        light.Diffuse = Color4.White;
        light.Radius = 10f;

        rendererInitialized = true;
      }

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
    }

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
