#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec3 aCol;
out vec4 color;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * vec4(aPos.x, aPos.y, 0.0F, 1.0F);
    color = vec4(aCol,1.0F);
}