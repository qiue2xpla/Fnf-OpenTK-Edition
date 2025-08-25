#version 330

out vec2 coord;  
layout (location = 0) in vec2 pos;
layout (location = 1) in vec2 ucoord;

uniform vec2 position; 
uniform vec2 size;
uniform float rotation; 

uniform float ratioGRID;

vec2 rotate(vec2 inp)
{
	inp.x /= ratioGRID;
	vec2 sn = inp * sin(rotation);
	vec2 cn = inp * cos(rotation);
	float x = cn.x - sn.y;
	float y = sn.x + cn.y;
	return vec2(x * ratioGRID, y);
}

void main() 
{ 
	coord = ucoord; 
	gl_Position = vec4(rotate(pos * size) + position, 0.0, 1.0);
}