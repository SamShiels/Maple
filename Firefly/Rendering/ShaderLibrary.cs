using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Firefly.Rendering
{
  public class ShaderLibrary
  {
    private Dictionary<string, Shader> library;
    private Assembly executingAssembly;

    private static ShaderLibrary instance = null;
    public static ShaderLibrary Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new ShaderLibrary();
        }
        return instance;
      }
    }

    public ShaderLibrary()
    {
      library = new Dictionary<string, Shader>();
      executingAssembly = Assembly.GetExecutingAssembly();

      Sprite();
      Diffuse();
      Canvas();
      CanvasInverted();
    }

    public Shader GetShader(string shaderName)
    {
      Shader shader;
      bool found = library.TryGetValue(shaderName, out shader);
      if (!found)
      {
        throw new DirectoryNotFoundException($"Cannot find shader {shaderName}");
      }

      return shader;
    }

    private void SetShader(string shaderName, Shader shader)
    {
      library.Add(shaderName, shader);
    }

    private void Canvas()
    {
      Shader shader = CreateShader("canvas");
      SetShader("canvas", shader);
    }

    private void CanvasInverted()
    {
      string vertex = @"
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

      string fragment = @"
        #version 450 core
        in vec2 texCoords;

        out vec4 FragColor;

        uniform bool u_invert;

        uniform sampler2D frameBufferTexture;

        void main()
        {   
          if (u_invert == true)
          {
            FragColor = vec4(vec3(1.0 - texture(frameBufferTexture, texCoords)), 1.0);
          } else
          {
            FragColor = vec4(vec3(texture(frameBufferTexture, texCoords)), 1.0);
          }
        }
      ";

      Shader shader = new Shader(vertex, fragment);
      SetShader("canvasInverted", shader);
    }

    private void Sprite()
    {
      string vertex = @"
        #version 450 core
        layout (location = 0) in vec4 a_position;
        <texcoord_attribute>

        <texture_unit_vert_dec>
        <3d_projection_uniform>

        void main()
        {
          <texcoord_vert_main>
          <texture_unit_vert>
          <3d_projection>
          gl_Position = viewPosition;
        }
      ";

      string fragment = @"
        #version 450 core

        out vec4 FragColor;
        <texcoord_frag>

        <texture_unit_frag_dec>
        <sampler_frag_dec>

        void main()
        {
          <texture_unit_sampler_frag>
          if (tex.a < 0.1)
          {
            discard;
          }
          FragColor = tex;
        }
      ";

      Shader shader = new Shader(vertex, fragment);
      SetShader("spriteBasic", shader);
    }

    //private void Diffuse()
    //{
    //  string vertex = @"
    //    #version 450 core
    //    layout (location = 0) in vec3 a_position;

    //    <texcoord_attribute>
		  //  <normal_attribute>
    //    <3d_projection_uniform>

    //    out vec3 FragPos;

    //    void main()
    //    {
    //      <normal_vert_main>
    //      <texcoord_vert_main>
    //      <3d_projection>
    //      FragPos = worldPosition.xyz;
    //      gl_Position = screenPosition;
    //    }
    //  ";

    //  string fragment = @"
    //    #version 450 core

    //    out vec4 FragColor;
    //    in vec3 FragPos;

    //    <texcoord_frag>
		  //  <normal_frag>

    //    uniform float u_shininess;
    //    uniform vec3 u_ambientLight;
    //    uniform vec3 u_lightDirection;

    //    struct PointLight {
    //      int used;
				//	vec3 position;
    //      float range;

				//	vec3 diffuse;
				//	vec3 specular;
				//};

				//#define NO_POINT_LIGHTS 16
				//uniform PointLight u_pointLights[16];

    //    uniform sampler2D u_images[1];

    //    vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
    //    {
    //      if (light.used == 0) {
    //        return vec3(0.0);
    //      }
    //      vec3 lightDir = normalize(light.position - fragPos);
    //      float diff = max(dot(normal, lightDir), 0.0);

    //      vec3 reflectDir = reflect(-lightDir, normal);
    //      float spec = pow(max(dot(viewDir, reflectDir), 0.0), u_shininess);

    //      float distance = length(light.position - fragPos);
    //      float atten = 1.0 / (distance / light.range);

    //      vec3 diffuse = diff * vec3(texture(u_images[0], texcoord));

    //      diffuse *= atten;

    //      return (diffuse);
    //    }

    //    void main()
    //    {
			 //   float directionalLight = dot(normal, u_lightDirection);

    //      vec4 light = max(vec4(u_ambientLight.xyz, 1.0), directionalLight);
    //      vec4 albedo = texture(u_images[0], texcoord);
    //      albedo *= light;

    //      vec3 norm = normalize(normal);
    //      vec3 viewDir = normalize(vec3(0.0) - FragPos);

    //      vec3 result = vec3(0.0);
    //      for (int i = 0; i < NO_POINT_LIGHTS; i++)
    //      {
    //        result += CalculatePointLight(u_pointLights[i], norm, FragPos, viewDir);
    //      }
          
    //      FragColor = vec4(result, 1.0);
    //    }
    //  ";

    //  string test = @"

    //    vec3 CalculatePointLight(PointLight light, vec3 normal, vec3 fragPos)
    //    {
    //      vec3 lightDir = normalize(light.position - fragPos);
    //      float diff = max(dot(normal, lightDir), 0.0);

    //      vec3 reflectDir = reflect(-lightDir, normal);
    //      float spec = pow(max(dot(viewDir, reflectDir), 0.0), u_shininess);

    //      return reflectDir;
    //    }";

    //  Shader shader = new Shader(vertex, fragment);
    //  SetShader("diffuse", shader);
    //}

    private void Diffuse()
    {
      Shader shader = CreateShader("diffuse");
      SetShader("diffuse", shader);
    }

    private Shader CreateShader(string fileName)
    {
      Stream vert = executingAssembly.GetManifestResourceStream($"Firefly.Core.Shader.Library.{fileName}.vert");
      Stream frag = executingAssembly.GetManifestResourceStream($"Firefly.Core.Shader.Library.{fileName}.frag");

      string vertSource = string.Empty;
      string fragSource = string.Empty;

      using (StreamReader reader = new StreamReader(vert))
      {
        vertSource = reader.ReadToEnd();
      }

      using (StreamReader reader = new StreamReader(frag))
      {
        fragSource = reader.ReadToEnd();
      }

      return new Shader(vertSource, fragSource);
    }
  }
}
