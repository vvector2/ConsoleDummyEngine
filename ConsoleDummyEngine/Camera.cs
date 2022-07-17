using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ConsoleDummyEngine
{
    public interface ICamera
    {
        Matrix3D Matrix { get; set; }
        Vector3D Project(Vector3D p);
        Plane[] GetClippingPlanes();
    }

    public class PerspectiveCamera : ICamera
    {
        private const double FAR = 50;
        private const double NEAR = 0.1;
        private const double FOE = 80;

        public Matrix3D Matrix { get; set; } = Matrix3D.Identity;

        public PerspectiveCamera()
        {
            
        }

        public Vector3D Project(Vector3D p)
        {
            var xClip = NEAR * p.X / p.Z;
            var yClip = NEAR * p.Y / p.Z;
            var zClip = (FAR * p.Z - FAR * NEAR )  / (FAR - NEAR) / p.Z;
            
            var halfFrustumSquare = Math.Tan(FOE / 2 * Math.PI / 180) * NEAR;
            
            var xNdc = 0.5 + xClip / halfFrustumSquare;
            var yNdc = 0.5 - yClip / halfFrustumSquare;

            return new Vector3D(xNdc, yNdc, zClip);
        }

        public Plane[] GetClippingPlanes()
        {
            throw new NotImplementedException();
        }
    }
    
    public class OrthographicCamera: ICamera
    {
        private const double FAR = 50;
        private const double NEAR = 0.1;
        
        private const double LEFT = -2;
        private const double RIGHT = 2;
        private const double TOP = 1.5;
        private const double BOTTOM = -1.5;
        
        private readonly Plane[] planes = new []
        {
            new Plane() { N = new Vector3D(0, 0, 1), P = new Vector3D(0, 0, NEAR) },
            new Plane() { N = new Vector3D(0, 0, -1), P = new Vector3D(0, 0, FAR) },
            new Plane() { N = new Vector3D(1, 0, 0), P = new Vector3D(LEFT, 0, NEAR) },
            new Plane() { N = new Vector3D(-1, 0, 0), P = new Vector3D(RIGHT , 0, NEAR) },
            new Plane() { N = new Vector3D(0, 1, 0), P = new Vector3D(0, BOTTOM , NEAR) },
            new Plane() { N = new Vector3D(0, -1, 0), P = new Vector3D(0, TOP, NEAR) },
        }; 

        public Matrix3D Matrix { get; set; } = Matrix3D.Identity;
        
        public OrthographicCamera()
        {
            
        }

        public Vector3D Project(Vector3D p)
        {
            var xClip = p.X;
            var yClip = p.Y;
            var zClip = (p.Z - NEAR) / (FAR - NEAR);
            
            var xNdc = 0.5 + xClip / ((RIGHT - LEFT) / 2);
            var yNdc = 0.5 - yClip / ((TOP - BOTTOM) / 2);

            return new Vector3D(xNdc ,yNdc , zClip);
        }

        public Plane[] GetClippingPlanes() => planes;
    }
}