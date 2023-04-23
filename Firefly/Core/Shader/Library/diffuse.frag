#version 450 core

out vec4 FragColor;
in vec3 FragWorldPosition;
in vec4 FragPositionInLightSpace;

in vec2 texcoord;
in vec3 normal;

struct PointLight {
    vec4 positionRange;
    vec4 colorIntensity;
};

struct DirectionalLight {
    vec4 directionIntensity;
    vec4 color;
};

struct AmbientLight {
    vec4 color;
};

const int POINT_LIGHT_COUNT = 32;
const int DIRECTIONAL_LIGHT_COUNT = 4;

layout (std140) uniform PointLightBlock {
	PointLight pointLights[POINT_LIGHT_COUNT];
};

layout (std140) uniform DirectionalLightBlock {
	DirectionalLight directionalLights[DIRECTIONAL_LIGHT_COUNT];
};

layout (std140) uniform AmbientLightBlock {
	vec4 ambientLight;
};

uniform vec3 u_lightDirection;
uniform sampler2D u_images[2];
uniform sampler2D u_shadowMaps[1];

vec3 CalculateDirectionalLight(DirectionalLight directionalLight)
{
    vec3 lightDirection = directionalLight.directionIntensity.xyz;
    float lightIntensity = directionalLight.directionIntensity.w;

    if (lightIntensity == 0.0) {
        return vec3(0.0);
    }

    vec3 lightColor = directionalLight.color.xyz;

    float angle = max(dot(normal, normalize(lightDirection)), 0.0);

    return lightColor * angle * lightIntensity;
}

vec3 CalculatePointLight(PointLight pointLight, vec3 calculatedNormal)
{
    vec3 lightPosition = pointLight.positionRange.xyz;
    float lightRange = pointLight.positionRange.w;

    if (lightRange == 0.0) {
		return vec3(0.0);
	}

    vec3 lightColor = pointLight.colorIntensity.xyz;
    float lightIntensity = pointLight.colorIntensity.w;

    vec3 lightDirToFragPosition = lightPosition - FragWorldPosition;
    float lightDistance = length(lightDirToFragPosition);

    if (lightDistance > lightRange) {
        return vec3(0.0);
    }

    float angle = max(dot(normal, normalize(lightDirToFragPosition)), 0.0);

    float lightDistanceFactor = min(1.0 - lightDistance / lightRange, 1.0);

    return lightColor * angle * lightDistanceFactor * lightIntensity;
}

float CalculateDirectionalShadow(vec4 fragPositionInLightSpace)
{
    vec3 projectedCoords = fragPositionInLightSpace.xyz / fragPositionInLightSpace.w;

    projectedCoords = projectedCoords * 0.5 + 0.5;
    float closestDepth = texture(u_shadowMaps[0], projectedCoords.xy).r;
    float currentDepth = projectedCoords.z - 0.005;
    float shadow = currentDepth > closestDepth ? 1.0 : 0.0;

    return shadow;
}

void main()
{
    vec3 diffuse = vec3(0.0);
    vec3 calculatedNormal = normal;

    vec3 normalMap = texture(u_images[1], texcoord).rgb;

    //calculatedNormal = calculatedNormal + normalMap;

    for (int i = 0; i < POINT_LIGHT_COUNT; i++)
    {
        diffuse += CalculatePointLight(pointLights[i], calculatedNormal);
    }
    for (int i = 0; i < DIRECTIONAL_LIGHT_COUNT; i++)
    {
        diffuse += CalculateDirectionalLight(directionalLights[i]);
    }

    float shadow = CalculateDirectionalShadow(FragPositionInLightSpace);

    vec4 lighting = max(vec4(diffuse - shadow, 1.0), ambientLight);
    FragColor = texture(u_images[0], texcoord) * lighting;
}