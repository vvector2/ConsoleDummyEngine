using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class Mesh
    {
        private readonly IEnumerable<Triangle> triangles;
        public Matrix3D matrix3D = Matrix3D.Identity;
        public bool WireFrame { get; set; }

        public Mesh(IEnumerable<Triangle> triangles)
        {
            this.triangles = triangles;
        }

        public IEnumerable<Triangle> GetWorldTriangles() => triangles.Select(tri => tri.Transform(matrix3D));
    }

    public struct Triangle
    {
        public Vector3D p1;
        public Vector3D p2;
        public Vector3D p3;
        public Vector3D normal;

        public Triangle(Vector3D p1, Vector3D p2, Vector3D p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            
            this.normal = Vector3D.CrossProduct(p2 - p1, p3 - p1);
            this.normal.Normalize();
        }

        public Triangle Transform(Matrix3D m)
        {
            var tri = this;
            tri.p1 = m.Transform3D(tri.p1);
            tri.p2 = m.Transform3D(tri.p2);
            tri.p3 = m.Transform3D(tri.p3);
            
            tri.normal = m.Transform(this.normal);
            tri.normal.Normalize();
            
            return tri;
        }
    }

}