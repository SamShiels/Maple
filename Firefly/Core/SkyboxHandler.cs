using Firefly.Core.Buffer;
using Firefly.Core.Shader;
using Firefly.Core.Texture;
using Firefly.Rendering;
using Firefly.Texturing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core
{
  internal class SkyboxHandler
  {
    private ShaderManager shaderManager;
    private TextureManager textureManager;
    private TextureComponent textureComponent;
    private Material material;
    private int VAO;
    private bool initialized;

    private VertexBufferObject<float> Positions { get; set; }
    private VertexBufferObject<float> TextureCoordinates { get; set; }

    internal SkyboxHandler(ShaderManager shaderManager, TextureManager textureManager)
    {
      this.shaderManager = shaderManager;
      this.textureManager = textureManager;

      string vs = @"
        #version 450 core
        layout (location = 0) in vec3 a_Position;

        out vec3 v_Position;

        void main()
        {
				  v_Position = a_Position;
          gl_Position = vec4(a_Position.xy, 1.0, 1.0); 
        }
      ";

      string fs = @"
        #version 450 core
        out vec4 FragColor;

        in vec3 v_Position;

			  uniform mat4 u_projectionMatrix;
			  uniform mat4 u_viewMatrix;
			  uniform samplerCube u_skybox;

        void main()
        {   
          vec4 t = u_projectionMatrix * u_viewMatrix * vec4(v_Position, 1.0);
				  FragColor = texture(u_skybox, normalize(t.xyz / t.w));
        }
      ";

      Rendering.Shader shader = new Rendering.Shader(vs, fs);
      material = new Material(shader);
      textureComponent = new TextureComponent();
    }

    public void DrawSkybox(Cubemap cubemap, Matrix4 projectionMatrix, Matrix4 viewMatrix)
    {
      if (!initialized)
      {
        textureComponent.CreateTexture(cubemap);
        CreateVBOs();
      }

      ShaderComponent shader = shaderManager.GetComponent(material);
      GL.BindVertexArray(VAO);
      shader.Use();

      int projLocation = shader.GetUniformLocation("u_projectionMatrix");
      GL.UniformMatrix4(projLocation, false, ref projectionMatrix);
      int viewLocation = shader.GetUniformLocation("u_viewMatrix");
      GL.UniformMatrix4(viewLocation, false, ref viewMatrix);

      textureComponent.SetUnit(TextureUnit.Texture0);
      GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 6);
    }

    private void CreateVBOs()
    {
      VAO = GL.GenVertexArray();
      GL.BindVertexArray(VAO);
      Positions = new VertexBufferObject<float>(DrawType.Static, false);
      TextureCoordinates = new VertexBufferObject<float>(DrawType.Static, false);

      Positions.PushData(new float[] { -1.0f, -1.0f, -1.0f,
                                        1.0f, -1.0f, -1.0f,
                                        1.0f,  1.0f, -1.0f,
                                       -1.0f, -1.0f, -1.0f,
                                        1.0f,  1.0f, -1.0f,
                                       -1.0f,  1.0f, -1.0f });

      TextureCoordinates.PushData(new float[] { 0.0f, 0.0f,
                                                1.0f, 0.0f,
                                                1.0f, 1.0f,
                                                0.0f, 0.0f,
                                                1.0f, 1.0f,
                                                0.0f, 1.0f });

      // bind the positions buffer and enable the positions attribute pointer
      Positions.Bind();
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
      TextureCoordinates.Bind();
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

      Positions.BufferData();
      TextureCoordinates.BufferData();

      initialized = true;
    }
  }
}
