using Firefly.World.Lighting;
using Firefly.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Lighting
{
  internal class DirectionalShadowHandler
  {
    private int depthTextureHandle;
    private int FBOHandle;

    private bool initialized = false;

    private const int width = 1024;
    private const int height = 1024;

    private Rendering.Shader depthShader;
    private Material depthMaterial;

    public DirectionalShadowHandler()
    {
    }

    public Matrix4 GetLightProjectionMatrix(DirectionalLight light, Vector3 assignedCameraPosition)
    {
      Vector3 lightEulerAngles = light.Transform.EulerAngles;
      Matrix4 lightProjectionMatrix = Matrix4.CreateOrthographic(20f, 20f, -10f, 10f);

      return lightProjectionMatrix;
    }

    public Material GetDepthMaterial()
    {
      Initialize();
      return depthMaterial;
    }

    public void Bind()
    {
      Initialize();
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
    }

    private void Initialize()
    {
      if (!initialized)
      {
        depthTextureHandle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, depthTextureHandle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        FBOHandle = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTextureHandle, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        string vs = @"
          #version 450 core
          layout (location = 0) in vec3 a_Position;
          
          uniform mat4 lightSpaceMatrix;
          uniform mat4 u_modelMatrix;

          void main()
          {
            gl_Position = lightSpaceMatrix * u_modelMatrix * vec4(a_Position, 1.0);
          }
        ";

        string fs = @"
          #version 450 core

          void main()
          {
            // gl_FragDepth = gl_FragCoord.z;
          }
        ";

        depthShader = new Rendering.Shader(vs, fs);
        depthMaterial = new Material(depthShader);
        initialized = true;
      }
    }
  }
}
