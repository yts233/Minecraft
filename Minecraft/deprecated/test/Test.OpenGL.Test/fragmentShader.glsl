#version 330 core
in vec4 col;
in vec2 texCoord;
out vec4 FragColor;

uniform sampler2D texture0;

void main() {
    vec4 texColor = texture(texture0, texCoord);
    if (texColor.a < 0.1)
        discard;
    FragColor = col * texColor;
}