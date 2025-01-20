# Cascading Shadowmapping
The basic technique is explained in the [previous article](4-3-Shadowmapping.md). This method may work for a lot of projects but there is a drawback in this solution when the player's environment grows. 

## Do you need Cascades?
The answer to that question is basically: how big is your game world? Let's get into detail of the problem and how that is solved:

### The problem explained
In the basic shadowmapping technique, the world is viewed from the lights point of view. To capture the right section of the gameworld, the view-projection for the lightsource is calculated. The light's frustum is calculated to capture the *entire* camera frustum. If the gameworld is quite small, the shadowmap can contain all pixels needed to calculate the shadows.

Once the world grows and the player can see things way in the distance, the shadowmap needs to grow to caputure everything. This results in a tradoff- either the resolution of the shadow is low (blocky shadows) or rendering of the shadowmap becomes resource heavy.

### The solution
The compromise is to slice the camera's frustum on sections: near, mid, far. This means we have detailed shadows nearby (similar to the single shadowmap) and low resolution shadows in the distance. The blockyness of distant shadows isn't a big issue, as the shadows are smaller and less detailed.

In order to render our scene with this technique we take the following steps:
1. Split the camera frustum into near, mid and far sections;
2. Calculate the lights view and projections to encompass these camera splits;
3. Render shadowmaps for each cascade;
4. Render the finalscene, reading the right shadowmap depending on the distance of the fragment.

## The code
The code is available in this repository, check the `chapter43cc.sln`. I tried to comment the code as much as possible so it should be self explanatory. The sections below highlight some of the code. 
In many cases the data for near, middle and far is stored in a array- where `0` represents near, `1` represents middle, `2` represents far.

### The shadowmap texture
In the end we need three lookups, one for each cascade. One way of doing this is passing three shadowmap textures. This also means our diffuse effect needs to read from three textures. 

To optimise this, it is possible to store three cascades by using the r,g and b values: the near cascade is stored in the red component, the middle in green and far in blue (if needed, you could add another cascade in the alpha channel). 

Let's make sure we define the shadowmap rendertarget to accept our color channels:

```csharp
            // The rendertarget is RGBA so we can encode depth data in each color component.
            _shadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Color, DepthFormat.Depth24);
```

Because each cascade is rendered, two things are needed between each cascade:
1. The shader must only affect the color corresponding to the cascade, while leaving the other values unaffected.
2. The depthmap must be reset- each cascade should render from a clean slate.

To affect only a specific color component, a special blendstate is defined, this is the red component only blendstate:

```csharp
    _cascadeBlendState[0] = new BlendState()
    {
        ColorWriteChannels = ColorWriteChannels.Red,
        ColorSourceBlend = Blend.One,
        ColorDestinationBlend = Blend.Zero,
        ColorBlendFunction = BlendFunction.Add
    };
```

In the `Draw()` loop the depthmap is reset in this statement:

```csharp
    GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
```

Finally in the shadowmap shader we add a uniform `float Cascade` to tell the pixelshader to output to the right channel:

```HLSL
float4 ShadowMapPS(ShadowMapVSOutput input) : COLOR
{
    // Depending on the Cascade a different color component is used:
    if(Cascade == 0)
        return float4(input.Depth, 1, 1, 0); 
    if (Cascade == 1)
        return float4(0, input.Depth, 0, 0);
    if (Cascade == 2)
        return float4(0, 0, input.Depth, 0);
    else
        return float4(0, 0, 0, 0);
}
```

