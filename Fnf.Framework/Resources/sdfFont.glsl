[VertexShader]
#version 330

layout (location = 0) in vec2 pos;
layout (location = 1) in vec2 coord;

out vec2 texCoord;

uniform vec2 position;
uniform vec2 scale;
uniform float rotation;

uniform vec2 UnitsPerPixel;
uniform float FontSize;

void main()
{
	texCoord = coord;

	vec2 newPos = pos * FontSize * scale;
	float esin = sin(rotation);
	float ecos = cos(rotation);
	float newX = ecos * newPos.x - esin * newPos.y;
	float newY = esin * newPos.x + ecos * newPos.y;
	newPos = vec2(newX, newY) * UnitsPerPixel;
	newPos += position * UnitsPerPixel;
	gl_Position = vec4(newPos, 0.0, 1.0);
}

[FragmentShader]
#version 330

out vec4 color;

in vec2 texCoord;

uniform sampler2D tex;
uniform vec3 textColor;
uniform float edge1;
uniform float edge2;

void main()
{
	color = vec4(textColor, smoothstep(0.473, 0.487, texture(tex, texCoord).a));
}