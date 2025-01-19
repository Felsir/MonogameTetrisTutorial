using Microsoft.Xna.Framework;

namespace Chapter43CC.Camera
{
    public class Camera
    {
        private int _screenWidth, _screenHeight;
        private float _fieldOfView;
        private Vector3 _position, _target;

        private Matrix _view, _projection;

        private float[] _splits;
        public Matrix[] CascadeProjection = new Matrix[4];

        // These two values determine the distance a projection can "see". 
        // In this case the closest object we can render is 0.5 unit from the camera.
        // The furthest the camera can see is 10 units away. 
        private const float nearPlane = 0.5f;
        private const float farPlane = 10f;

        // Return a vector4 of the splits, so the shader can read them.
        public Vector4 Splits
        {
            get
            {
                return new Vector4(_splits[0], _splits[1], _splits[2], _splits[3]);
            }
        }

        public Camera(Vector3 position, Vector3 target, int screenWidth, int screenHeight, float fieldOfView)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _fieldOfView = fieldOfView;

            _position = position;
            _target = target;

            // based on the near and far plane, the cascade splits are calculated. 
            float planeDistance = farPlane - nearPlane;
            _splits = new float[] { nearPlane, nearPlane+ (planeDistance * 0.2f) , nearPlane + (planeDistance * 0.5f), farPlane };

            CalculateMatrices();
        }

        private void CalculateMatrices()
        {
            _view = Matrix.CreateLookAt(_position, _target, Vector3.Up);
            float aspect = (float)_screenWidth / (float)_screenHeight;
            _projection = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, nearPlane, farPlane);

            CascadeProjection[0] = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, nearPlane, _splits[1]);
            CascadeProjection[1] = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, _splits[1], _splits[2]);
            CascadeProjection[2] = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, _splits[2], farPlane);
            //CascadeProjection[3] = Matrix.CreatePerspectiveFieldOfView(_fieldOfView, aspect, _splits[1], farPlane);
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
