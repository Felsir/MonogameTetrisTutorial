using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chapter43.Light
{
    public class ShadowCastingLight
    {
        private Vector3 _lightDirection;

        public ShadowCastingLight(Vector3 lightDirection)
        {
            _lightDirection = Vector3.Normalize(lightDirection);
        }

        public Vector3 LightDirection
        {
            get
            {
                return _lightDirection;
            }
            set
            {
                _lightDirection = Vector3.Normalize(value);
            }
        }

        // The reason this function has the camera projection
        // as a separate parameter is so this can create a cascading shadowmap.
        public Matrix CalculateMatrix(Matrix cameraView, Matrix cameraProjection)
        {
            // Generate the frustum of the camera for which we're casting the shadow
            // Here we obtain the shape of the frustum as it points into the 3D world.
            // We need this so we know what area of the world we observe in our 
            // shadowmap.
            BoundingFrustum cameraFrustum = new BoundingFrustum(cameraView * cameraProjection);


            // Next, we generate the frustum of the lightsource.
            // Our light is directional, so we need to encompass the entire 
            // camera frustum as our light affects the entire scene in the frustum:
                        
            // Get the corners of the frustum
            Vector3[] frustumCorners = cameraFrustum.GetCorners();

            // Transform the positions of the corners into the direction of the light
            // Create Matrix to rotate point in the direction of the light;
            Matrix RotateInLightDirection = Matrix.CreateLookAt(Vector3.Zero, -_lightDirection, Vector3.Up);

            // Perform the rotation
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                frustumCorners[i] = Vector3.Transform(frustumCorners[i], RotateInLightDirection);
            }

            // Find the smallest box around the points
            BoundingBox lightBoundingBox = BoundingBox.CreateFromPoints(frustumCorners);
            Vector3 boxSize = lightBoundingBox.Max - lightBoundingBox.Min;


            // The position of the light should be in the center
            // of the back side of the boundingbox. 
            Vector3 halfBoxSize = boxSize * 0.5f;
            Vector3 lightPosition = lightBoundingBox.Min + halfBoxSize;
            lightPosition.Z = lightBoundingBox.Min.Z;

            // We need the position everything back into world coordinates
            // to do this we transform position by the inverse of the lightrotation we calculated earlier
            lightPosition = Vector3.Transform(lightPosition, Matrix.Invert(RotateInLightDirection));

            // Create the view matrix
            Matrix lightView = Matrix.CreateLookAt(lightPosition,
                                                   lightPosition - _lightDirection,
                                                   Vector3.Up);

            // Finally, create the projection matrix for the light
            // The projection is orthographic, because the light is a directional light.
            Matrix lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y,
                                                               -boxSize.Z, boxSize.Z);

            // Store the calculated Matrix. View and Projection are combined for ease of use later
            return lightView * lightProjection;
        }

    }
}
