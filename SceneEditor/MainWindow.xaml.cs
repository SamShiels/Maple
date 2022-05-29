using Firefly;
using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.Utilities;
using Firefly.World;
using Firefly.World.Mesh;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Wpf;
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
    public Renderer renderer { private set; get; }
    public Scene scene { private set; get; }

    public MainWindow()
    {
      InitializeComponent();

      GLWpfControlSettings settings = new GLWpfControlSettings {
        MajorVersion = 4,
        MinorVersion = 6,
      };

      OpenTkControl.Start(settings);

      string vs = @"
        #version 450 core
        layout (location = 0) in vec3 a_Position;
        layout (location = 1) in vec2 a_TexCoords;

        out vec2 texCoords;

        void main()
        {
          texCoords = a_TexCoords;
          gl_Position = vec4(a_Position, 1.0); 
        }
      ";

      string fs = @"
        #version 450 core
        #define PI 3.1415926538

        in vec2 texCoords;

        out vec4 FragColor;

        uniform vec2 curvature;
        uniform sampler2D frameBufferTexture;

        vec2 curveFunction(vec2 texUV)
        {
          texUV = texUV * 2.0 - 1.0;
          vec2 offset = abs(texUV) / curvature;
          texUV = texUV + texUV * offset * offset;
          texUV = texUV * 0.5 + 0.5;
          return texUV;
        }

        void main()
        { 
          vec4 canvasColor = vec4(texture(frameBufferTexture, texCoords));
          FragColor = canvasColor;
        }
      ";
      //vec2 curvature = curveFunction(texCoords);

      //float warp = sin(texCoords.x * PI) * (texCoords.x - 0.5) * -0.5;
      Shader CRT = new Shader(vs, fs);

      Material material = new Material(CRT);
      renderer = new Renderer(800, 450, 800, 450, 0, material, false);
      renderer.ProjectionType = ProjectionType.Perspective;
      renderer.VerticalFieldOfView = 90;
      scene = new Scene();
      renderer.UpdateBackgroundColor(new Color4(0.3f, 0.2f, 0.4f, 1.0f));
      Model cube = OBJLoader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.cube.obj"));

      Uniform ambientLight = new Uniform("u_ambientLight", new Vector3(0.0f, 0.0f, 0.0f));
      Uniform directionalLight = new Uniform("u_lightDirection", new Vector3(0.1f, 0.5f, 1.0f));
      Uniform shininess = new Uniform("u_shininess", 0.5f);
      Uniform[] uniforms = new Uniform[3] { ambientLight, directionalLight, shininess };

      Material material2 = new Material(ShaderLibrary.Instance.GetShader("diffuse"), uniforms);
      Firefly.Texturing.Image house = new Firefly.Texturing.Image(Assembly.GetExecutingAssembly().GetManifestResourceStream("SceneEditor.Resources.house.png"));
      Texture textureHouse = new Texture(house);

      MeshObject cubeMesh = new MeshObject();
      cubeMesh.Model = cube;
      cubeMesh.Transform.Position = new Vector3(0f, 0f, -5f);
      cubeMesh.Transform.LocalScale = new Vector3(1f, 1f, 1f);
      cubeMesh.Textures = new Texture[] { textureHouse };
      cubeMesh.Material = material2;

      scene.AddObject(cubeMesh);
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
      renderer.RenderRaw(scene);
    }
  }
}
