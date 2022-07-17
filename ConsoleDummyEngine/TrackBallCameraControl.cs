using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class TrackBallCameraControl
    {
        private readonly ICamera camera;
        private const double panSpeed = 0.05;
        private const double rotateSpeed = 0.5;
        private Point lastPos;
        private bool leftMouseState = false;
        private Vector3D target = new Vector3D(0,0,2);

        public TrackBallCameraControl(ICamera camera)
        {
            this.camera = camera;
        }

        public void Handle(bool isUpKeyDown, bool isDownKeyDown, bool isLeftKeyDown, bool isRightKeyDown,
            Point mousePos, bool leftMouseDown)
        {
            if (leftMouseState && !leftMouseDown)
            {
                leftMouseState = false;   
            }
            else if (!leftMouseState && leftMouseDown)
            {
                leftMouseState = true;
                lastPos = mousePos;
            }
            else if (leftMouseState && leftMouseDown)
            {
                var diffP = lastPos - mousePos;
                var diff = new Vector3D(diffP.X, diffP.Y, 0);

                var cameraPosition = camera.Matrix.Offset();

                var eye = cameraPosition - target;

                var eyeN = eye;
                eyeN.Normalize();

                var up = camera.Matrix.Up();
                up.Normalize();

                var sideWayDir = Vector3D.CrossProduct(up, eyeN);
                sideWayDir.Normalize();
                sideWayDir *= diff.X;

                var UpDir = up * diff.Y;

                var moveDir = sideWayDir + UpDir;

                var axis = Vector3D.CrossProduct(moveDir, eye);
                axis.Normalize();

                var angle = rotateSpeed * diff.Length;

                if (angle < 1)
                    return;
                
                var q = new Quaternion(axis, angle);

                var matrix = camera.Matrix;
                matrix.Translate(- target);
                matrix.Rotate(q);
                matrix.Translate(target);

                camera.Matrix = matrix;

                lastPos = mousePos;
            }
            else
            {
                var panVectorBase = new Vector3D();
                panVectorBase.X += (isLeftKeyDown ? -1 : 0) + (isRightKeyDown ? 1 : 0);
                panVectorBase.Y += (isUpKeyDown ? 1 : 0) + (isDownKeyDown ? -1 : 0);
                panVectorBase *= panSpeed;

                var baseX = new Vector3D(camera.Matrix.M11, camera.Matrix.M12, camera.Matrix.M13);
                var baseY = new Vector3D(camera.Matrix.M21, camera.Matrix.M22, camera.Matrix.M23);
                var panVector = baseX * panVectorBase.X + baseY * panVectorBase.Y;

                var m = camera.Matrix;
                m.Translate(panVector);
                camera.Matrix = m;

                target += panVector;
            }
        }
    }
}