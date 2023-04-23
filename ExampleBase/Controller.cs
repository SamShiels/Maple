using OpenTK.Mathematics;
using Firefly;
using Firefly.Core;
using Firefly.Rendering;
using System;
using Firefly.Utilities;
using Firefly.World;
using Firefly.World.Scene;
using System.Reflection;

namespace ExampleBase
{
  public class Controller
  {
    public Renderer renderer { private set; get; }
    public SceneObject scene { set; get; }

    public Camera camera;
    public ResourceLoader resourceLoader;

    private int windowWidth;
    private int windowHeight;

    private Material canvasMaterial;

    public Controller(int windowWidth, int windowHeight)
    {
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;

      resourceLoader = new ResourceLoader();
    }

    public virtual void OnLoad()
    {
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
        in vec2 texCoords;

        out vec4 FragColor;

        uniform sampler2D frameBufferTexture;

        void main()
        { 
          FragColor = vec4(vec3(texture(frameBufferTexture, texCoords)), 1.0);
        }
      ";
      //vec2 curvature = curveFunction(texCoords);

      //float warp = sin(texCoords.x * PI) * (texCoords.x - 0.5) * -0.5;
      Shader CRT = new Shader(vs, fs);

      Material material = new Material(CRT);
      canvasMaterial = material;
      renderer = new Renderer(windowWidth, windowHeight);
      renderer.AmbientLight = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
      renderer.ClearColor = new Color4(0.3f, 0.2f, 0.4f, 1.0f);
      scene = new SceneObject();
      camera = new Camera();
      camera.CanvasMaterial = material;
      camera.BackgroundColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
      camera.Transform.Position = new Vector3(0.0f, 0.0f, 0.0f);
      scene.AssignCamera(camera);
    }

    /// <summary>
    /// Game loop.
    /// </summary>
    public virtual void OnUpdate()
    {

    }

    private float test = 0.0f;
    
    /// <summary>
    /// Render loop.
    /// </summary>
    public virtual void OnRender()
    {
      renderer.Render(scene);
    }
    
    public virtual void OnUnload()
    {
      renderer.Destroy();
    }
  }
}
