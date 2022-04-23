using OpenTK.Mathematics;
using Renderer;
using Renderer.Core;
using Renderer.Materials;
using Renderer.Shaders;
using System;

namespace ExampleBase
{
  public class Controller
  {
    public Firefly graphics { private set; get; }
    public WorldObject stage { private set; get; }

    private int windowWidth;
    private int windowHeight;

    private Material canvasMaterial;

    public Controller(int windowWidth, int windowHeight)
    {
      this.windowWidth = windowWidth;
      this.windowHeight = windowHeight;
    }

    public virtual void OnLoad()
    {
      Uniform invertColours = new Uniform("u_invert", true);

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
      Material material = new Material(CRT, new Uniform[] { curvature });
      canvasMaterial = material;
      graphics = new Firefly(2560, 1440, windowWidth, windowHeight, material, true);
      graphics.ProjectionType = Renderer.Utilities.ProjectionType.Perspective;
      graphics.VerticalFieldOfView = 90;
      stage = new WorldObject();
      graphics.UpdateBackgroundColor(new OpenTK.Mathematics.Color4(0.3f, 0.2f, 0.4f, 1.0f));
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
      //if (test == true) return;
      //graphics.Clear();
      test += 0.03f;
      float resScale = ((float)Math.Sin(test) + 1) / 2;

      graphics.UpdateResolution((int)Math.Round(2560.0f * resScale), (int)Math.Round(1440.0f * resScale));
      graphics.Render(stage);
    }
    
    public virtual void OnUnload()
    {
      graphics.Destroy();
    }
  }
}
