[VertexShader]
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

[FragmentShader]
#version 330

out vec4 color;
in vec2 coord;

uniform vec4 col;
uniform float ratioUI;
uniform float radius;
uniform float smoothness;

vec2 fix(vec2 raw)
{
	if(ratioUI > 1) // tall
	{
		return vec2(raw.x, raw.y * ratioUI);
	}
	else
	{
		return vec2(raw.x / ratioUI, raw.y);
	}
}


void main() 
{
	vec2 size = vec2(1 - radius);

	if(ratioUI > 1) // tall
	{
		size.y = ratioUI - radius;
	}
	else // short
	{
		size.x = 1 / ratioUI - radius;
	}

	vec2 dist = max( abs(fix(coord)), size ) - size;
	float alpha = 1 - smoothstep(radius - smoothness, radius, length(dist));
	color = vec4(col.rgb, alpha * col.a);
}
