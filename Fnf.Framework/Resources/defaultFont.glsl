[VertexShader]
#version 330

layout (location = 0) in vec2 pos;
layout (location = 1) in vec2 coord;

out vec2 texCoord;

uniform mat3 transform;
uniform float fontSize;

void main()
{
	texCoord = coord;
	gl_Position = vec4(transform * vec3(pos, 1), 1.0);
}

[FragmentShader]
#version 330

out vec4 color;
in vec2 texCoord;

uniform sampler2D tex;
uniform vec3 textColor;

float Pixel(float x, float y)
{
	return texture(tex, texCoord + vec2(x,y)).r;
}

float Sample(float sampleArea, int sampleCount)
{
	vec2 texSize = textureSize(tex,0);

	float cursorX = -(sampleArea / texSize.x) / 2;
	float cursorY = (sampleArea / texSize.y) / 2;

	float sum = 0;

	for (int y = 0; y < sampleCount; y++)
	{
		for (int x = 0; x < sampleCount; x++)
		{
			sum += Pixel(cursorX, cursorY);
			cursorX += (sampleArea / texSize.x) / (sampleCount - 1);
		}
		cursorX = -(sampleArea / texSize.x) / 2;
		cursorY -= (sampleArea / texSize.y) / (sampleCount - 1);
	}

	return sum / (sampleCount * sampleCount);
}

void main()
{
	color = vec4(textColor, Sample(1, 3));
}