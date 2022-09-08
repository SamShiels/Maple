using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Firefly.Core.Shader
{
  internal class ShaderComponent : IDisposable
  {
		private int program;
    private string vertexShaderSource;
    private string fragmentShaderSource;
		private Dictionary<string, string> shaderChunks;
		private Rendering.Uniform[] uniforms;
		private bool usesTextureUnits = false;

		private bool disposedValue = false;

		private Dictionary<string, int> uniformLocations;
		private Dictionary<string, int> pointLightUniformLocations;

		public ShaderComponent(string vertexShaderSource, string fragmentShaderSource, Rendering.Uniform[] uniforms, Dictionary<string, string> shaderChunks)
    {
      this.vertexShaderSource = vertexShaderSource;
      this.fragmentShaderSource = fragmentShaderSource;
			this.uniforms = uniforms;
			this.shaderChunks = shaderChunks;
			Compile();
			ObtainUniforms();
			CheckTextureUnits();
		}

		/// <summary>
		/// Activate the program.
		/// </summary>
		public void Use()
    {
			GL.UseProgram(program);
    }

		/// <summary>
		/// Get a uniform location by name.
		/// </summary>
		/// <param name="uniformName"></param>
		/// <returns></returns>
		public int GetUniformLocation(string uniformName)
    {
			int location = -1;
			uniformLocations.TryGetValue(uniformName, out location);
			return location;
		}

		public bool UsesTextureUnits()
    {
			return usesTextureUnits;
    }

		private void ObtainUniforms()
    {
			uniformLocations = new Dictionary<string, int>();
			int screenToClipLocation = GL.GetUniformLocation(program, "u_projectionMatrix");
			uniformLocations.Add("u_projectionMatrix", screenToClipLocation);
			int modelMatrixLocation = GL.GetUniformLocation(program, "u_modelMatrix");
			uniformLocations.Add("u_modelMatrix", modelMatrixLocation);
			int viewMatrixLocation = GL.GetUniformLocation(program, "u_viewMatrix");
			uniformLocations.Add("u_viewMatrix", viewMatrixLocation);
			int imagesLocation = GL.GetUniformLocation(program, "u_images");
			if (imagesLocation != -1)
			{
				uniformLocations.Add("u_images", imagesLocation);
			}

			if (uniforms != null) {
				for (int i = 0; i < uniforms.Length; i++)
				{
					Rendering.Uniform uniform = uniforms[i];
					int location = GL.GetUniformLocation(program, uniform.name);
					uniformLocations.Add(uniform.name, location);
				}
			}

			uniformLocations.Add("PointLightBlock", GL.GetUniformBlockIndex(program, "PointLightBlock"));
			uniformLocations.Add("AmbientLightBlock", GL.GetUniformBlockIndex(program, "AmbientLightBlock"));
		}

		public void BindUniformBlock(string blockName, int blockBindingPoint)
    {
			int location = -1;
			uniformLocations.TryGetValue(blockName, out location);
			GL.UniformBlockBinding(program, location, blockBindingPoint);
    }

		/// <summary>
		/// Checks if this shader uses the texture unit attribute.
		/// </summary>
		private void CheckTextureUnits()
    {
			bool texUnitLocation = vertexShaderSource.Contains("a_texUnit");
			usesTextureUnits = texUnitLocation;
		}

		/// <summary>
		/// Create our shaders and link the program.
		/// </summary>
		private void Compile()
		{
			vertexShaderSource = InjectShaderChunks(vertexShaderSource);
			fragmentShaderSource = InjectShaderChunks(fragmentShaderSource);

			int vs = CreateShader(ShaderType.VertexShader, vertexShaderSource);
			int fs = CreateShader(ShaderType.FragmentShader, fragmentShaderSource);

			int Program = GL.CreateProgram();
			GL.AttachShader(Program, vs);
			GL.AttachShader(Program, fs);
			GL.LinkProgram(Program);

			program = Program;

			GL.DetachShader(Program, vs);
			GL.DetachShader(Program, fs);
			GL.DeleteShader(vs);
			GL.DeleteShader(fs);
		}

		/// <summary>
		/// Create and compile a shader.
		/// </summary>
		/// <param name="type">Vertex or fragment.</param>
		/// <param name="source">The shader source code.</param>
		/// <returns></returns>
		private int CreateShader(ShaderType type, string source)
		{
			int shader = GL.CreateShader(type);
			GL.ShaderSource(shader, source);
			GL.CompileShader(shader);

			string infoLogVert = GL.GetShaderInfoLog(shader);
			if (infoLogVert != string.Empty)
			{
				Console.WriteLine(infoLogVert);
				Console.WriteLine(type.ToString());
				Console.WriteLine(source);
			}

			return shader;
		}

		private string InjectShaderChunks(string shader)
    {
			Regex regex = new Regex("\\<.*?\\>");

			MatchCollection matches = regex.Matches(shader);
			foreach (Match match in matches)
      {
				string chunkName = match.Value.Replace("<", "");
				chunkName = chunkName.Replace(">", "");
				string chunk;
				shaderChunks.TryGetValue(chunkName, out chunk);

				shader = shader.Replace(match.Value, chunk);
			}

			return shader;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				GL.DeleteProgram(program);

				disposedValue = true;
			}
		}
		~ShaderComponent()
		{
			GL.DeleteProgram(program);
		}

		public void Dispose()
    {
			Dispose(true);
			GC.SuppressFinalize(this);
    }
	}
}
