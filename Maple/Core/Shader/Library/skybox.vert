#version 450 core
layout (location = 0) in vec2 a_Position;

out vec4 TexCoords;
uniform mat4 u_projectionMatrix;

void main()
{
	TexCoords = (u_projectionMatrix * vec4(-a_Position.xy, 1.0, 1.0));
    gl_Position = vec4(a_Position, 1.0, 1.0);
}