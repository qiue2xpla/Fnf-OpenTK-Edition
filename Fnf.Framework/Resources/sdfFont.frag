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