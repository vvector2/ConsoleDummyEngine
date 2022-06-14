using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ConsoleDummyEngine
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //lol
            var mesh1 = Helpers.GetBox(0.25);
            var mesh2 = Helpers.GetBox(0.25);
            var mesh3 = Helpers.GetBox(0.25);
            var mesh4 = Helpers.GetBox(0.25);
            var mesh5 = Helpers.GetBox(0.25);

            var renderer = new Renderer(203, 203);
            renderer.AddMesh(mesh1, mesh2, mesh3, mesh4, mesh5);

            var i = 0;
            renderer.beforeRender += () =>
            {
                var rotMatrix = Matrix3D.Identity;
                rotMatrix.Rotate(new Quaternion(new Vector3D(1, 1, 1), i));

                mesh1.matrix3D = rotMatrix;
                mesh1.matrix3D.OffsetX = 0.5;
                mesh1.matrix3D.OffsetZ = 2;
                
                mesh2.matrix3D = rotMatrix;
                mesh2.matrix3D.OffsetX = -0.5;
                mesh2.matrix3D.OffsetZ = 2;
                
                mesh3.matrix3D = rotMatrix;
                mesh3.matrix3D.OffsetY = -0.5;
                mesh3.matrix3D.OffsetZ = 2;
                
                mesh4.matrix3D = rotMatrix;
                mesh4.matrix3D.OffsetY = 0.5;
                mesh4.matrix3D.OffsetZ = 2;
                
                mesh5.matrix3D = rotMatrix;
                mesh5.matrix3D.OffsetZ = 2;
                
                i+= 3;
            };

            renderer.StartRender();
        }
    }
}