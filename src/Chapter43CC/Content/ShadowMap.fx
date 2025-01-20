#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix ViewProjection;
matrix World;
float Cascade = 0;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct ShadowMapVSOutput
{
    float4 Position : SV_Position;
    float Depth : TEXCOORD0;
};

ShadowMapVSOutput ShadowMapVS(VertexShaderInput input)
{
    ShadowMapVSOutput output;

    // Calculate position as usual- the ViewProjection Matrix already takes
    // the new light frustum into account.
    output.Position = mul(input.Position, mul(World, ViewProjection));
    
    // Calculate depth. The division by the W component brings the component
    // into homogenous space- which means the depth is normalized on a 0-1 scale.
    output.Depth = output.Position.z / output.Position.w;
    
    return output;
}

float4 ShadowMapPS(ShadowMapVSOutput input) : COLOR
{
    // Depending on the Cascade a different color component is used:
    if(Cascade == 0)
        return float4(input.Depth, 0, 0, 0); 
    if (Cascade == 1)
        return float4(0, input.Depth, 0, 0);
    if (Cascade == 2)
        return float4(0, 0, input.Depth, 0);
    else
        return float4(0, 0, 0, 0);
}

technique ShadowMapDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL ShadowMapVS();
        PixelShader = compile PS_SHADERMODEL ShadowMapPS();
    }
};