#version 450 core
layout (location = 0) in vec3 a_position;
layout (location = 1) in vec2 a_texcoord;
layout (location = 2) in vec3 a_normal;

out vec2 texcoord;
out vec3 normal;

uniform mat4 u_modelMatrix;
uniform mat4 u_viewMatrix;
uniform mat4 u_projectionMatrix;
uniform mat4 u_lightSpaceMatrix;

out vec3 FragWorldPosition;
out vec4 FragPositionInLightSpace;

void main()
{
    vec4 worldNormal = transpose(inverse(u_modelMatrix)) * vec4(a_normal.xyz, 1.0);
    normal = normalize(vec3(worldNormal.xyz));
    texcoord = a_texcoord;
    
    vec4 worldPosition =  u_modelMatrix * vec4(a_position, 1.0);
	vec4 screenPosition = u_projectionMatrix * u_viewMatrix * worldPosition;

    FragWorldPosition = worldPosition.xyz;
    FragPositionInLightSpace = u_lightSpaceMatrix * worldPosition;

    gl_Position = screenPosition;
}