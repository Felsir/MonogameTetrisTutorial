using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapter43.Camera
{
    public class Camera
    {
        private int _screenWidth, _screenHeight;
        private float _fieldOfView;
        private Vector3 _position, _target;

        private Matrix _view, _projection;

        // These two values determine the distance a projection can "see". 
        // In this case the closest object we can render is 0.5 unit from the camera.
        // The furthest the camera can see is 5 units away. 
        // The reason these small values are chosen, is to ensure our light frustum result
        // fits in our 4096x4096 texturesize with enough detail.
        // To fix this- Cascading Shadowmaps are the solution.
        private const float nearPlane = 0.1f;
        private const float farPlane = 5f;

        public Camera(Vector3 position, Vector3 target, int screenWidth, int screenHeight, float fieldOfView)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _fieldOfView = fieldOfView;

            _position = position;
            _target = target;

            CalculateMatrices();
        }

        private void CalculateMatrices()
        {
            _view = Matrix.CreateLookAt(_position, _target, Vector3.Up);
            float aspect = (float)_screenWidth / (float)_screenHeight;
            _projection = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, nearPlane, farPlane);
        }

        public Matrix View
        {
            get
            {
                return _view;
            }
        }

        public Matrix Projection
        {
            get
            {
                return _projection;
            }
        }

        public void SetCameraPosition(Vector3 position)
        {
            _position = position;
            CalculateMatrices();
        }

        public void SetCameraTarget(Vector3 target)
        {
            _target = target;
            CalculateMatrices();
        }
    }
}
