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
        layout (location = 0) in vec2 a_Position;

        out vec2 TexCoords;

        void main()
        {
				  TexCoords = -a_Position.xy;
          gl_Position = vec4(a_Position, 1.0, 1.0);
        }
      ";

      string fs = @"
        #version 450 core
        out vec4 FragColor;

        in vec2 TexCoords;

			  uniform mat4 u_projectionMatrix;
			  uniform mat4 u_viewMatrix;

			  uniform samplerCube u_skybox;

        void main()
        {
          vec4 direction = u_projectionMatrix * u_viewMatrix * vec4(TexCoords, 1.0, 1.0);
				  FragColor = texture(u_skybox, direction.xyz);
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

      //GL.DepthMask(false);
      ShaderComponent shader = shaderManager.GetComponent(material);
      GL.BindVertexArray(VAO);
      shader.Use();

      int projLocation = shader.GetUniformLocation("u_projectionMatrix");
      GL.UniformMatrix4(projLocation, false, ref projectionMatrix);
      int viewLocation = shader.GetUniformLocation("u_viewMatrix");
      Matrix4 viewMatrixWithoutTranslation = viewMatrix.ClearTranslation();
      GL.UniformMatrix4(viewLocation, false, ref viewMatrixWithoutTranslation);

      textureComponent.SetUnit(TextureUnit.Texture0);
      GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 6);
      //GL.DepthMask(true);
    }

    private void CreateVBOs()
    {
      VAO = GL.GenVertexArray();
      GL.BindVertexArray(VAO);
      Positions = new VertexBufferObject<float>(DrawType.Static, false);
      TextureCoordinates = new VertexBufferObject<float>(DrawType.Static, false);

      Positions.PushData(new float[] { -1.0f,  1.0f,
                                        1.0f, -1.0f,
                                        1.0f,  1.0f,
                                       -1.0f,  1.0f,
                                       -1.0f, -1.0f,
                                        1.0f, -1.0f, });

      // bind the positions buffer and enable the positions attribute pointer
      Positions.Bind();
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

      Positions.BufferData();

      initialized = true;
    }
  }
}
