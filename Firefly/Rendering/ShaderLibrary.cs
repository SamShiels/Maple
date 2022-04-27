using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Rendering
{
  public class ShaderLibrary
  {
    private Dictionary<string, ShaderProgram> library;

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
      library = new Dictionary<string, ShaderProgram>();
      Sprite();
      Diffuse();
      Canvas();
      CanvasInverted();
    }

    public ShaderProgram GetShader(string shaderName)
    {
      ShaderProgram shader;
      library.TryGetValue(shaderName, out shader);
      return shader;
    }

    private void SetShader(string shaderName, ShaderProgram shader)
    {
      library.Add(shaderName, shader);
    }

    private void Canvas()
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

        uniform sampler2D frameBufferTexture;

        void main()
        {   
          FragColor = vec4(vec3(texture(frameBufferTexture, texCoords)), 1.0);
        }
      ";

      ShaderProgram shader = new ShaderProgram(vertex, fragment);
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

      ShaderProgram shader = new ShaderProgram(vertex, fragment);
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
          gl_Position = position;
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

      ShaderProgram shader = new ShaderProgram(vertex, fragment);
      SetShader("spriteBasic", shader);
    }

    private void Diffuse()
    {
      string vertex = @"
        #version 450 core
        layout (location = 0) in vec4 a_position;

        <texcoord_attribute>
		    <normal_attribute>
        <3d_projection_uniform>

        void main()
        {
          <normal_vert_main>
          <texcoord_vert_main>
          <3d_projection>
          gl_Position = position;
        }
      ";

      string fragment = @"
        #version 450 core

        out vec4 FragColor;
        <texcoord_frag>
		    <normal_frag>

        uniform vec3 u_ambientLight;
        uniform vec3 u_lightDirection;

        uniform sampler2D u_images[1];

        void main()
        {
			    float directionalLight = dot(normal, u_lightDirection);

          vec4 light = max(vec4(u_ambientLight.xyz, 1.0), directionalLight);
          vec4 albedo = texture(u_images[0], texcoord);
          albedo *= light;
          
          FragColor = albedo;
        }
      ";

      ShaderProgram shader = new ShaderProgram(vertex, fragment);
      SetShader("diffuse", shader);
    }
  }
}
