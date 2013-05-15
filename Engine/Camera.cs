using SlimDX;
using System;

namespace MagicBall.Engine
{
    class Camera
    {
        public Vector3 Position;
        public Vector3 Up;
        public Vector3 Target;

        public Camera()
        {
            Position = -Vector3.UnitZ;
            Up = Vector3.UnitY;
            Target = Vector3.Zero;
        }

        public void SetPositionSpherical(float head, float pitch, float r)
        {
            Quaternion q = Quaternion.RotationYawPitchRoll(head, pitch, 0);
            q.Normalize();

            Matrix m = Matrix.RotationQuaternion(q);

            Vector3 viewDir = new Vector3(m.M31, m.M32, m.M33);

            Up = new Vector3(m.M21, m.M22, m.M23);
            Position = -viewDir * r;
        }

        public Matrix CameraView
        {
            get
            {
                return Matrix.LookAtLH(Position, Target, Up);
            }
        }
    }
}
