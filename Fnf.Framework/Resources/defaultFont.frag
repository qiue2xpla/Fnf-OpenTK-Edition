#version 330

out vec4 color;
in vec2 texCoord;

uniform sampler2D tex;
uniform vec3 textColor;

float Sample(float x, float y)
{
	return texture(tex, texCoord + vec2(x,y)).a;
}

void main()
{
	float spread = 1;
	vec2 ts = textureSize(tex,0);
	float w = spread / ts.x;
	float h = spread / ts.y;

	float sum = 0;

	sum += Sample(-w, h);
	sum += Sample(0, h);
	sum += Sample(w, h);

	sum += Sample(-w, 0);
	sum += Sample(0 , 0);
	sum += Sample(w, 0);

	sum += Sample(-w, -h);
	sum += Sample(0, -h);
	sum += Sample(w, -h);

	color = vec4(textColor, sum / 9);
}