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

// This is the view-projection in relation to the frustum of the lightsource
// we need this to calculate the cross reference pixel
matrix LightViewProjection; 
float3 LightDirection;
float ShadowStrength = 0.4; // value between 0-1, where 0 means black shadows

// Depth bias is a factor to take into account that the pixels are not a 1:1 match
// we need some leeway to find the right pixel.
float DepthBias = 0.001;

// Color of the our object.
float4 DiffuseColor = float4(1, 0, 0, 1);

// The shadowmap that should have been created in a earlier step
// this is the sampler for the shadow depth information.
texture ShadowTexture;
sampler shadowSampler = sampler_state
{
    Texture = (ShadowTexture);
    MAGFILTER = LINEAR;
    MINFILTER = LINEAR;
    MIPFILTER = LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
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
    float4 Position3D : TEXCOORD0;
};


VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    // Transform the position using the matrices
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Create the normal for simple shading
    output.Normal = normalize(mul(input.Normal, (float3x3) World));

    // The position of this element in the 3D world coordinates, 
    // we need this to calculate the cross reference in the shadowmap.
    output.Position3D = worldPosition;
    
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    //Very simple shading based on the shadow strength.
    float4 diffuse = DiffuseColor;
    float diffuseIntensity = clamp(dot(input.Normal.xyz, LightDirection.xyz), 0, 1);
    diffuse *= ShadowStrength + (1 - ShadowStrength) * diffuseIntensity;
    
    // Insert additional shading techniques such as specular or rimlights.
    
    // From this point the shadowmap component is calculated.
    //
    // Find the position of this pixel in light space in the projection
    float4 lightingPosition = mul(input.Position3D, LightViewProjection);
    
    lightingPosition.xyz = 0.5 * lightingPosition.xyz / lightingPosition.w; // transform into homogenous space
    
    // Check if the found pixel is inside the light frustum
    if (lightingPosition.x > -0.5 && lightingPosition.x<0.5 && lightingPosition.y>-0.5 && lightingPosition.y < 0.5)
    {
        lightingPosition = mul(input.Position3D, LightViewProjection);

        // Find the position in the shadow map for this pixel
        float2 ShadowTexCoord = 0.5 * lightingPosition.xy /
            lightingPosition.w + float2(0.5, 0.5);
        
        // Rendering y coordinate needs flipping
        // sampling and rendering from a different y direction.
        ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

        // Get the current depth stored in the shadow map (red component for close)
        float shadowdepth = tex2D(shadowSampler, ShadowTexCoord).r;

        // Calculate the current pixel depth
        // The bias is used to prevent floating point errors that occur when
        // the pixel of the occluder is being drawn
        float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;
        
        // Check to see if this pixel is in front or behind the value in the shadow map
        if (shadowdepth < ourdepth)
        {
            // Shadow the pixel by multiplying by the shadowstrength
            diffuse.rgb *= ShadowStrength;
        }
    }
    
    return diffuse;
}



technique BasicDiffuseDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};