using Firefly;
using Firefly.Rendering;
using Firefly.Utilities;
using Firefly.World;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using System;
using System.Windows;

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

      var settings = new GLWpfControlSettings
      {
        MajorVersion = 4,
        MinorVersion = 5
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

      Uniform curvature = new Uniform("curvature", new Vector2(2, 2));
      Material material = new Material(CRT, null);

      renderer = new Renderer(2560, 1440, (int)OpenTkControl.Width, (int)OpenTkControl.Height, material, true);
      renderer.ProjectionType = ProjectionType.Perspective;
      renderer.VerticalFieldOfView = 90;
      scene = new Scene();
      renderer.UpdateBackgroundColor(new Color4(0.3f, 0.2f, 0.4f, 1.0f));
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
      //ExampleScene.Render();
      renderer.UpdateBackgroundColor(new Color4(0.3f, 0.2f, 0.4f, 1.0f));
      renderer.Render(scene);
    }
  }
}
