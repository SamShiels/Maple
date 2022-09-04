#version 450 core
in vec2 texCoords;

out vec4 FragColor;

uniform sampler2D frameBufferTexture;

void main()
{   
    FragColor = vec4(vec3(texture(frameBufferTexture, texCoords)), 1.0);
}