#version 450 core

out vec4 FragColor;
in vec3 FragPos;

in vec2 texcoord;
in vec3 normal;

struct PointLight {
    vec4 positionRange;
    vec4 color;
};

struct AmbientLight {
    vec4 color;
};

const int POINT_LIGHT_COUNT = 32;

layout (std140) uniform PointLightBlock {
	PointLight lights[POINT_LIGHT_COUNT];
};

layout (std140) uniform AmbientLightBlock {
	vec4 ambientLight;
};

uniform float u_shininess;
uniform vec3 u_lightDirection;
uniform sampler2D u_images[1];

vec3 CalculatePointLight(vec3 lightPosition, float lightRange, vec3 color, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(lightPosition - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), u_shininess);

    float distance = length(lightPosition - fragPos);
    float atten = 1.0 / (distance / lightRange);

    return color * atten;
}

void main()
{
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(vec3(0.0) - FragPos);

    vec3 diffuse = vec3(ambientLight.rgb);
    for (int i = 0; i < POINT_LIGHT_COUNT; i++)
    {
        diffuse += CalculatePointLight(lights[i].positionRange.xyz, lights[i].positionRange.w, lights[i].color.rgb, norm, FragPos, viewDir);
    }
    vec4 albedo = texture(u_images[0], texcoord);
    FragColor = albedo * vec4(diffuse, 1.0);
}