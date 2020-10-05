#version 330 core

layout(location = 0) out vec4 color;

in vec2 texCoord;
in vec4 colortest;
in vec3 N;
in float visiblity;

uniform sampler2D texture0;
uniform vec4 FOG_Color;

uniform vec4 AmbienceColor;

void main()
{
    color = texture(texture0, texCoord) * AmbienceColor;
    color = mix(FOG_Color, color, visiblity);
}