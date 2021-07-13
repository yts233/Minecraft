#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
out vec3 objectColor;
out vec3 normal;
out vec3 deltaPos;
uniform vec3 color;
uniform vec3 centerPosition;
uniform vec2 position;
uniform float offset;
uniform mat4 view;
uniform mat4 projection;
void main() {
    vec3 worldPosition = vec3(aPos.x * 12 + position.x * 12, aPos.y * 4 + 128, aPos.z * 12 + position.y * 12 - offset);
    gl_Position = projection * view * vec4(worldPosition, 1.0F);
    objectColor = color;
    normal = aNormal;
    deltaPos = worldPosition - centerPosition;
}
