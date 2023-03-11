#version 450 core
out vec4 FragColor;

in vec4 TexCoords;

uniform samplerCube u_skybox;

void main()
{
	FragColor = texture(u_skybox, TexCoords.xyz);
}