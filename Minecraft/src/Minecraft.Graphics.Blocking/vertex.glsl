#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTex;
layout (location = 2) in vec3 aNor;
out vec2 texCoord;
out vec4 color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = projection * view * model * vec4(aPos, 1.0F);
    //gl_Position = vec4(aPos, 1.0F);
    texCoord = vec2(aTex.x, aTex.y);
    const float amb = 0.7F;
    const float dlig = 0.3F;
    const vec3 lightDir = normalize(vec3(1.0F,3.0F,2.0F));
    float diff = max(dot(aNor,lightDir),0.0F);
    float light = diff * dlig + amb;
    color = vec4(vec3(1.0F)*light,1.0F);
}