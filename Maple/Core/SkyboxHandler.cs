using Maple.Core.Buffer;
using Maple.Core.Shader;
using Maple.Core.Texture;
using Maple.Rendering;
using Maple.Texturing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core
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

    internal SkyboxHandler(ShaderManager shaderManager, TextureManager textureManager)
    {
      this.shaderManager = shaderManager;
      this.textureManager = textureManager;

      string vs = @"
        #version 450 core
        layout (location = 0) in vec2 a_Position;

        out vec4 TexCoords;
			  uniform mat4 u_projectionMatrix;

        void main()
        {
				  TexCoords = (u_projectionMatrix * vec4(a_Position.xy, 1.0, 1.0));
          gl_Position = vec4(a_Position, 1.0, 1.0);
        }
      ";

      string fs = @"
        #version 450 core
        out vec4 FragColor;

        in vec4 TexCoords;

			  uniform samplerCube u_skybox;

        void main()
        {
				  FragColor = texture(u_skybox, TexCoords.xyz);
        }
      ";

      Rendering.Shader shader = new Rendering.Shader(vs, fs);
      material = new Material(shader);
      textureComponent = new TextureComponent();
    }

    public void DrawSkybox(Cubemap cubemap, Matrix4 cameraLocalToWorldMatrix, Matrix4 projection)
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
      // Take the current camera's projection matrix and rotate it
      Matrix4 rotationMatrix = cameraLocalToWorldMatrix;
      Matrix4 projectionRotated = projection * rotationMatrix;

      GL.UniformMatrix4(projLocation, false, ref projectionRotated);

      // Apply the skybox texture to unit 0 and draw using our dedicated VAO
      textureComponent.SetUnit(TextureUnit.Texture0);
      GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, 6);
    }

    private void CreateVBOs()
    {
      VAO = GL.GenVertexArray();
      GL.BindVertexArray(VAO);
      Positions = new VertexBufferObject<float>(DrawType.Static, false);

      Positions.PushData(new float[] { -1.0f, -1.0f,
                                        1.0f, -1.0f,
                                        1.0f,  1.0f,
                                       -1.0f, -1.0f,
                                        1.0f,  1.0f,
                                       -1.0f,  1.0f, });

      // bind the positions buffer and enable the positions attribute pointer
      Positions.Bind();
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

      Positions.BufferData();

      initialized = true;
    }
  }
}
