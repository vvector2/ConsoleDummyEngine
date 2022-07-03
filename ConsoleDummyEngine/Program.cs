using System.Collections.Generic;
using System.Threading;
using System.Windows.Media.Media3D;

namespace ConsoleDummyEngine
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var renderer = new MyGame(203, 203, new PerspectiveCamera());
            renderer.StartRender();
        }
    }

    public class MyGame : Renderer
    {
        private int i = 0;
        public MyGame(int width, int height, ICamera camera) : base(width, height, camera)
        {
        }
        
        protected override void Setup()
        {
            base.Setup();
            var mesh = Helpers.GetBox(1);
            //var mesh = Helpers.ReadObjFile("assets/al.obj");
            AddMesh(mesh);
        }

        protected override void BeforeRender()
        {
            base.BeforeRender();
            
            var mesh = this.meshes[0];
            var rotMatrix = Matrix3D.Identity;

            rotMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), i));
            rotMatrix.Scale(new Vector3D(0.1, 0.1, 0.1));
            rotMatrix.Translate(new Vector3D(0, 0, 1));

            mesh.Matrix3D = rotMatrix;

            i++;
        }
    }
}