using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class Mesh
    {
        public Matrix3D Matrix3D { get; set; } = Matrix3D.Identity;
        public bool WireFrame { get; set; }
        private readonly IEnumerable<Triangle> triangles;
        public readonly Guid Id;

        public Mesh(IEnumerable<Triangle> triangles)
        {
            this.triangles = triangles;
            Id = Guid.NewGuid();
        }

        public IEnumerable<Triangle> GetWorldTriangles() => triangles.Select(tri => tri.Transform(Matrix3D));
    }

    public struct Plane
    {
        public Vector3D P;
        public Vector3D N;
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