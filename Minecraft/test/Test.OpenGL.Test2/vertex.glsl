#version 330 core
layout (location = 0) in vec2 aPos;
out vec4 color;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0F, 1.0F);
}