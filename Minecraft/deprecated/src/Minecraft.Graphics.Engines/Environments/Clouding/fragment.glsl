#version 330 core
in vec3 deltaPos;
in vec3 objectColor;
in vec3 normal;
out vec4 FragColor;

void main() {
    const vec3 up = vec3(0F, 1F, 0F);
    vec3 lightColor = vec3(1F);
    vec3 ambient = 0.8F * lightColor;
    float diffA = max(dot(normal, normalize(vec3(1.0F,5.0F,1.0F))), 0.0);
    float diffB = max(dot(normal, normalize(vec3(-2.0F,1.0F,-1.0F))), 0.0);
    vec3 diffuseA = diffA * lightColor * .7F;
    vec3 diffuseB = diffB * lightColor * .1F;
    float skySpec = max(min(512 / length(deltaPos), 1.0F), 0.5F);
    vec3 result = (ambient + diffuseA + diffuseB) * objectColor;
    FragColor = vec4(result.x * skySpec, result.y * skySpec, result.z, 1.0F);
}
