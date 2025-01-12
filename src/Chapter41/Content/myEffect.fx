#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix View;
matrix Projection;
matrix World;
float3 DiffuseLightDirection = float3(-0.5, 0.15, 0.5);
float4 DiffuseColor = float4(1, 1, 1, 1);

texture Texture;

sampler diffuseSampler = sampler_state
{
    Texture = (Texture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float3 WorldNormal : NORMAL1;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    // Transform the position using the matrices
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Copy the normal and texture coordinates
    output.Normal = input.Normal;
    output.TexCoord = input.TexCoord;

    // Calculate the world normal
    output.WorldNormal = normalize(mul(input.Normal, (float3x3) World));

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float intensity = dot(normalize(DiffuseLightDirection), input.WorldNormal);
    if (intensity < 0)
        intensity = 0;

    float4 textureColor = tex2D(diffuseSampler, input.TexCoord);

    return textureColor * DiffuseColor * intensity;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};