#version 330 core
layout(location=0) in vec2 aPos;
layout(location=1) in vec4 aCol;
layout(location=2) in vec2 aTex;

out vec4 color;
out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
	gl_Position=projection*view*model* vec4(aPos,0F,1F);
	color=aCol;
	texCoord=aTex;
}