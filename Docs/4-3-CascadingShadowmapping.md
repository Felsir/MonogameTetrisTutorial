# Cascading Shadowmapping
The basic technique is explained in the [previous article](4-3-Shadowmapping.md). This method may work for a lot of projects but there is a drawback in this solution when the player's environment grows. 

### The problem explained
In the basic shadowmapping technique, the world is viewed from the lights point of view. To capture the right section of the gameworld, the view-projection for the lightsource is calculated. The light's frustum is calculated to capture the *entire* camera frustum. If the gameworld is quite small, the shadowmap can contain all pixels needed to calculate the shadows.

Once the world grows and the player can see things way in the distance, the shadowmap needs to grow to caputure everything. This results in a tradoff- either the resolution of the shadow is low (blocky shadows) or rendering of the shadowmap becomes resource heavy.

### The solution
The compromise is to slice the camera's frustum on sections: near, mid, far. This means we have detailed shadows nearby (similar to the single shadowmap) and low resolution shadows in the distance. The blockyness of distant shadows isn't a big issue, as the shadows are smaller and less detailed.

1. Split the camera frustum into near, mid and far sections;
2. Calculate the lights view and projections to encompass these camera splits;
3. Render shadowmaps for each cascade;
4. Render the finalscene, reading the right shadowmap depending on the distance of the fragment.


