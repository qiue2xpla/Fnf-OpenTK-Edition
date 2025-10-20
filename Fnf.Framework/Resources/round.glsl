[VertexShader]
#version 330

layout (location = 0) in vec2 pos;
out vec2 coord;  

uniform mat3 transform;

void main() 
{ 
	coord = pos; 
	gl_Position = vec4(transform * vec3(pos, 1), 1);
}

[FragmentShader]
#version 330

in vec2 coord;
out vec4 color;

uniform vec4 col;
uniform vec2 rect;
uniform vec4 radius;
uniform float smoothness;

void main() 
{
	float minValue = min(rect.x, rect.y);

	float cornerBasedRadius = 0;

	if(coord.x > 0)
	{
		if(coord.y > 0)
		{
			// Topright
			cornerBasedRadius = radius.x;
		}
		else
		{
			// DownRight
			cornerBasedRadius = radius.w;
		}
	}
	else
	{
		if(coord.y > 0)
		{
			// Topleft
			cornerBasedRadius = radius.y;
		}
		else
		{
			// Downleft
			cornerBasedRadius = radius.z;
		}
	}


	float radiusUV = 2 * cornerBasedRadius / minValue;
	vec2 rectUV = rect / minValue;

	vec2 size = vec2(rectUV - radiusUV);
    vec2 dist = max(abs(coord * rectUV) - size, 0);

    color = col * vec4(1, 1, 1, 1 - smoothstep(radiusUV - (smoothness / minValue), radiusUV, length(dist)));
}