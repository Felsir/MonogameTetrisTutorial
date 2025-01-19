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
// Because we need to find the right cascade we need the split values
// once the right cascade is found, the corresponding LightViewProjection is used to 
// do the calculations.
float4 CascadeSplits;
matrix LightViewProjection[3];

bool ShowCascades = true;

float3 LightDirection; // Light direction used to add shading
float ShadowStrength = 0.25; // value between 0-1, where 0 means black shadows

// Depth bias is a factor to take into account that the pixels are not a 1:1 match
// we need some leeway to find the right pixel.
float DepthBias = 0.01;

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
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

// See comment in the vertex shader why the additional components exist.
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float4 Position3D : TEXCOORD0;
    float4 CascadePosition : TEXCOORD2;
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
    output.Position3D = worldPosition;
    // The position of this element in camera coordinates,
    // we need this to calculate the cross reference in the shadowmap.
    // (in this shadermodel, it is not possible to read from the SV_POSITION, so we need to copy it).
    output.CascadePosition = output.Position;
    
    return output;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    //Very simple shading based on the shadow strength.
    float4 diffuse = DiffuseColor;
    float diffuseIntensity = clamp(dot(input.Normal.xyz, LightDirection.xyz), 0, 1);
    


    // Shadowmap section 
    
    // Find the right cascade, based on the distance from the camera.
    float cascadeIndex = 0;
    if (input.CascadePosition.z > CascadeSplits[0] && input.CascadePosition.z <= CascadeSplits[1])
    {
        cascadeIndex = 0;
    }
    else if (input.CascadePosition.z > CascadeSplits[1] && input.CascadePosition.z <= CascadeSplits[2])
    {
        cascadeIndex = 1;
    }
    else if (input.CascadePosition.z > CascadeSplits[2] && input.CascadePosition.z <= CascadeSplits[3])
    {
        cascadeIndex = 2;
    }
    
    // -- This only exists for visualising the cascades:
    if (ShowCascades)
    {
        if (cascadeIndex == 0)
            diffuse = float4(1, 0, 0, 1);
        if (cascadeIndex == 1)
            diffuse = float4(0, 1, 0, 1);
        if (cascadeIndex == 2)
            diffuse = float4(0, 0, 1, 1);
    }
    
    // Find the position of this pixel in light space in the projection
    float4 lightingPosition = mul(input.Position3D, LightViewProjection[cascadeIndex]);

    // Find the position in the shadow map for this pixel
    float2 ShadowTexCoord = 0.5 * lightingPosition.xy /
            lightingPosition.w + float2(0.5, 0.5);
        
    // Rendering y coordinate needs flipping
    // sampling and rendering from a different y direction.
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    // Get the current depth stored in the shadow map (red component for close, green for mid, blue for distant)
    float shadowdepth = tex2D(shadowSampler, ShadowTexCoord)[cascadeIndex];

    // Calculate the current pixel depth
    // The bias is used to prevent floating point errors that occur when
    // the pixel of the occluder is being drawn
    float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;
        
    float shadowIntensity = 1; //1 means no shadow
    // Check to see if this pixel is in front or behind the value in the shadow map
    if (shadowdepth < ourdepth)
    {
        // Set a shadow value for this pixel. 
        // Intentionally a bit brighter than absolutely dark, it gives a
        // better visual effect when combined with the other simple shading.
        shadowIntensity = 0.2;
    }
    
    // Shade the final pixel. It combines the simple shading and shadow shading and picks whichever is darkest.
    diffuse *= ShadowStrength + (1 - ShadowStrength) * min(diffuseIntensity, shadowIntensity);
    return diffuse;
}


technique CascadedDiffuseDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};