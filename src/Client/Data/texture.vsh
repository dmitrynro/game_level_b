#version 400

uniform mat4 Transform;

layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoord;

out vec2 fragTexCoord;

void main()
{
	gl_Position = vec4(Position, 0.0, 1.0) * Transform;
	fragTexCoord = TexCoord;
}
