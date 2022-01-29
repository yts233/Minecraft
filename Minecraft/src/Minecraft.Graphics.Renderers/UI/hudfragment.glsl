#version 330 core

in vec4 color;
in vec2 texCoord;
out vec4 FragColor;
uniform sampler2D texture1;

void main()
{
	vec4 outColor = texture(texture1, texCoord) * color;
	if(outColor.w<.0001F)
		discard;
    FragColor = outColor;
}