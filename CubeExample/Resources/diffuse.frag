#version 450 core

out vec4 FragColor;
in vec3 FragPos;

in vec2 texcoord;
in vec3 normal;

struct PointLight {
    vec4 positionRange;
    vec4 colorIntensity;
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

uniform vec3 u_lightDirection;
uniform sampler2D u_images[1];

vec3 CalculatePointLight(PointLight pointLight, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 position = pointLight.positionRange.xyz;
    float range = pointLight.positionRange.w;
    vec3 color = pointLight.colorIntensity.xyz;
    float intensity = pointLight.colorIntensity.w;

    vec3 lightDir = normalize(position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 reflectDir = reflect(-lightDir, normal);

    float distance = length(position - fragPos);
    float atten = 1.0 / (distance / range / intensity);

    return color * atten;
}

void main()
{
    vec3 norm = normalize(normal);
    vec3 viewDir = normalize(vec3(0.0) - FragPos);

    vec3 diffuse = vec3(0.0);
    for (int i = 0; i < POINT_LIGHT_COUNT; i++)
    {
        diffuse += CalculatePointLight(lights[i], norm, FragPos, viewDir);
    }
    vec4 albedo = texture(u_images[0], texcoord);
    FragColor = albedo * vec4(max(diffuse, ambientLight.rgb), 1.0);
}