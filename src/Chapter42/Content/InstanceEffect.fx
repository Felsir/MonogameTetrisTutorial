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

float3 DiffuseLightDirection = float3(1, 0.75, 0.5);
float4 DiffuseColor = float4(1, 1, 1, 1);

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
    float3 WorldPos : TEXCOORD1;
    float4 CustomColor : TEXCOORD2;
};

VertexShaderOutput MainVS(VertexShaderInput input,
float4 WorldRow1 : POSITION1, float4 WorldRow2 : POSITION2, float4 WorldRow3 : POSITION3, float4 WorldRow4 : POSITION4,
float4 CustomColor : COLOR0)
{
    // We received our regular model data via VertexShaderInput input,
    // the additional instance parameters are in WorldRow1-4 and CustomColor.
    // Let's construct the Matrix:

	float4x4 InstanceWorld = float4x4(WorldRow1, WorldRow2, WorldRow3, WorldRow4);

	VertexShaderOutput output = (VertexShaderOutput)0;

    // Transform the position using the instance's world matrix
    float4 worldPosition = mul(input.Position, InstanceWorld);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Transform the normal
    output.Normal = input.Normal;
    output.WorldNormal = normalize(mul(input.Normal, (float3x3) InstanceWorld));

    // Copy texture coordinates
    output.TexCoord = input.TexCoord;

    output.CustomColor = CustomColor;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float intensity = dot(normalize(DiffuseLightDirection), input.WorldNormal);
    if (intensity < 0)
        intensity = 0;

    return input.CustomColor * (DiffuseColor * intensity);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};