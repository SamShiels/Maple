using Renderer.Materials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Renderer.Shaders
{
	public class ShaderManager
	{
		private Dictionary<int, ShaderComponent> components;
		private Dictionary<string, string> shaderChunks;

		public ShaderManager(int maxFragmentTextureUnits)
    {
			components = new Dictionary<int, ShaderComponent>();
			shaderChunks = new Dictionary<string, string>();

			// 2D projection chunks
			shaderChunks.Add("2d_projection_uniform", @"uniform mat3 u_projectionMatrix;");
			shaderChunks.Add("2d_projection", @"vec4 position = vec4((u_projectionMatrix * a_position).xy, a_position.z, 1.0);");

			// 3D projection chunks
			shaderChunks.Add("3d_projection_uniform", @"uniform mat4 u_projectionMatrix; uniform mat4 u_modelMatrix;");
			shaderChunks.Add("3d_projection", @"vec4 position = a_position * u_modelMatrix; position = u_projectionMatrix * position;");

			// Sampler chunks
			shaderChunks.Add("sampler_frag_dec", string.Format(@"uniform sampler2D u_images[{0}];", maxFragmentTextureUnits));

			// Batch texture chunks
			shaderChunks.Add("texture_unit_vert_dec", @"layout (location = 3) in float a_texUnit; out float texunit;");
			shaderChunks.Add("texture_unit_frag_dec", @"in float texunit;");
			shaderChunks.Add("texture_unit_vert", @"texunit = a_texUnit;");
			//shaderChunks.Add("texture_unit_sampler_frag", @"vec4 tex = texture(u_images[int(texunit)], texcoord);");
			shaderChunks.Add("texture_unit_sampler_frag", @"vec4 tex = texture(u_images[int(floor(texunit + 0.5))], texcoord);");

			shaderChunks.Add("texcoord_attribute", @"layout (location = 1) in vec2 a_texcoord; out vec2 texcoord;");
			shaderChunks.Add("normal_attribute", @"layout (location = 2) in vec3 a_normal; out vec3 normal;");
			 
			shaderChunks.Add("texcoord_vert_main", @"texcoord = a_texcoord;");
			shaderChunks.Add("normal_vert_main", @"vec4 worldNormal = vec4(a_normal.xyz, 1.0) * transpose(inverse(u_modelMatrix)); normal = normalize(vec3(worldNormal.xyz));");

			shaderChunks.Add("texcoord_frag", @"in vec2 texcoord;");
			shaderChunks.Add("normal_frag", @"in vec3 normal;");
		}

		public ShaderComponent GetComponent(Material material)
		{
			ShaderComponent component;
			Shader shader = material.Shader;
			bool exists = components.TryGetValue(shader.Id, out component);
			if (!exists)
      {
				component = CreateComponent(material.Shader, material.Uniforms);
      }
			return component;
		}

		private ShaderComponent CreateComponent(Shader shader, Uniform[] Uniforms) {
			ShaderComponent component = new ShaderComponent(shader.VertexShaderSource, shader.FragmentShaderSource, Uniforms, shaderChunks);
			components.Add(shader.Id, component);
			return component;
		}
	}
}
