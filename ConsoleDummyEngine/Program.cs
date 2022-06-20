using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ConsoleDummyEngine
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            var mesh = Helpers.ReadObjFile("assets/al.obj");
            var renderer = new Renderer(203, 203);
            renderer.AddMesh(mesh);
            
            var i = 180;
            renderer.beforeRender += () =>
            {
                var rotMatrix = Matrix3D.Identity;
                
                rotMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), i));
                rotMatrix.Scale(new Vector3D(0.1, 0.1, 0.1));
                rotMatrix.Translate(new Vector3D(0,0, 1));

                mesh.matrix3D = rotMatrix;
                
                i+= 3;
            };

            renderer.StartRender();
        }
    }
}