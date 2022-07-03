using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class FirstPersonCameraControl
    {
        private readonly ICamera camera;

        public FirstPersonCameraControl(ICamera camera)
        {
            this.camera = camera;
        }

        public void Handle(bool isUpKeyDown, bool isDownKeyDown,bool isLeftKeyDown,bool isRightKeyDown, Point mousePos)
        {
            var v = new Vector3D();
            v.X += (isLeftKeyDown ? -1 : 0) + (isRightKeyDown ? 1 : 0);
            v.Z += (isUpKeyDown ? 1 : 0) + (isDownKeyDown ? -1 : 0);
            v = v * 0.1;
            
            camera.Matrix.Translate(v);
        }
    }
}