using Firefly.World.Lighting;
using Firefly.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;
using Firefly.Core.Texture;
using Firefly.Texturing;
using System.Reflection.Metadata;
using Firefly.Core.Shader;

namespace Firefly.Core.Lighting
{
  internal class DirectionalShadowHandler
  {
    private bool initialized = false;

    private const int width = 2048;
    private const int height = 2048;

    private Rendering.Shader depthShader;
    private Material depthMaterial;

    private int maxTextureUnits;
    private int maxShadowMapCount;

    private Dictionary<uint, DirectionalLight> cachedLights;
    private Dictionary<uint, int> FBOHandles;
    private List<int> depthTextureHandles;

    public DirectionalShadowHandler(int maxTextureUnits, int maxShadowMapCount)
    {
      this.maxTextureUnits = maxTextureUnits;
      this.maxShadowMapCount = maxShadowMapCount;

      depthTextureHandles = new List<int>();
      FBOHandles = new Dictionary<uint, int>();

      cachedLights = new Dictionary<uint, DirectionalLight>();

      string vs = @"
        #version 450 core
        layout (location = 0) in vec3 a_Position;
          
        uniform mat4 u_lightMatrix;
        uniform mat4 u_modelMatrix;

        void main()
        {
          gl_Position = u_modelMatrix * vec4(a_Position, 1.0);
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
    }

    public Matrix4 GetLightProjectionMatrix(DirectionalLight light)
    {
      Matrix4 lightProjectionMatrix = Matrix4.CreateOrthographicOffCenter(-10f, 10f, -10f, 10f, -20f, 20f);
      return lightProjectionMatrix;
    }

    public Matrix4 GetLightViewMatrix(DirectionalLight light)
    {
      Matrix4 viewMatrix = Matrix4.LookAt(light.Transform.Forward, Vector3.Zero, Vector3.UnitY);
      return viewMatrix;
    }

    public Material GetDepthMaterial()
    {
      return depthMaterial;
    }

    public void BindFrameBuffer(DirectionalLight light)
    {
      Initialize(light);

      if (FBOHandles.TryGetValue(light.Id, out int handle))
      {
        GL.Viewport(0, 0, width, height);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
      }
    }

    public void BindDepthMap()
    {
      for (int i = 0; i < Math.Min(depthTextureHandles.Count, 4); i++)
      {
        GL.ActiveTexture(TextureUnit.Texture0 + maxTextureUnits + i);
        GL.BindTexture(TextureTarget.Texture2D, depthTextureHandles[i]);
      }
    }

    public void UploadSamplerPositions(ShaderComponent shader)
    {
      int samplerLocation = shader.GetUniformLocation("u_shadowMaps");
      if (samplerLocation > -1)
      {
        int[] samplers = new int[Math.Min(depthTextureHandles.Count, 4)];
        for (int i = 0; i < Math.Min(depthTextureHandles.Count, 4); i++)
        {
          samplers[i] = maxTextureUnits + i;
        }
        GL.Uniform1(samplerLocation, samplers.Length, samplers);
      }
    }

    private void Initialize(DirectionalLight light)
    {
      uint lightId = light.Id;
      if (!cachedLights.ContainsKey(lightId))
      {
        int depthTextureHandle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, depthTextureHandle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        depthTextureHandles.Add(depthTextureHandle);

        int FBOHandle = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBOHandle);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTextureHandle, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        FBOHandles.Add(lightId, FBOHandle);

        cachedLights.Add(lightId, light);
      }
    }
  }
}
