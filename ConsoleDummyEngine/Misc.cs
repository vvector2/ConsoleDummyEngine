using System;
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
    
    public struct PixelInfo
    {
        public PixelInfo(float zBuffer, int color, ConsoleCharacter consoleCharacter = ConsoleCharacter.Full)
        {
            this.zBuffer = zBuffer;
            this.color = color;
            this.consoleCharacter = consoleCharacter;
            set = false;
        }
        public bool set;
        public double zBuffer;
        public int color;
        public ConsoleCharacter consoleCharacter;
    }
    
    
    public struct Triangle
    {
        public Point3D p1;
        public Point3D p2;
        public Point3D p3;
        public int color;

        public Triangle(Point3D p1, Point3D p2, Point3D p3, int color = 0)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.color = color;
        }

        public Triangle Transform(Matrix3D m)
        {
            var tri = this;
            tri.p1 = m.Transform(tri.p1);
            tri.p2 = m.Transform(tri.p2);
            tri.p3 = m.Transform(tri.p3);
            return tri;
        }
    }
    
    public static class Helpers
    {
        public static Point3D Clone(this Point3D p) => new Point3D(p.X, p.Y, p.Z);
        public static Point FloorTo2D(this Point3D p) => new Point((int)Math.Floor(p.X), (int)Math.Floor(p.Y));
        
        public static Mesh GetBox(double size)
        {
            //front
            var tri1 = new Triangle(new Point3D(-size / 2, 0, 0), new Point3D(size / 2, 0, 0),
                new Point3D(size / 2, size, 0), 1);
            var tri2 = new Triangle(new Point3D(-size / 2, 0, 0), new Point3D(size / 2, size, 0),
                new Point3D(-size / 2, size, 0), 1);

            //top
            var tri3 = new Triangle(new Point3D(-size / 2, size, 0), new Point3D(size / 2, size, 0),
                new Point3D(size / 2, size, size), 2);
            var tri4 = new Triangle(new Point3D(-size / 2, size, 0), new Point3D(size / 2, size, size),
                new Point3D(-size / 2, size, size), 2);

            //bottom
            var tri5 = new Triangle(new Point3D(-size / 2, 0, 0), new Point3D(size / 2, 0, 0),
                new Point3D(size / 2, 0, size), 3);
            var tri6 = new Triangle(new Point3D(-size / 2, 0, 0), new Point3D(size / 2, 0, size),
                new Point3D(-size / 2, 0, size), 3);

            //back
            var tri7 = new Triangle(new Point3D(-size / 2, 0, size), new Point3D(size / 2, 0, size),
                new Point3D(size / 2, size, size), 4);
            var tri8 = new Triangle(new Point3D(-size / 2, 0, size), new Point3D(size / 2, size, size),
                new Point3D(-size / 2, size, size), 4);

            // //left
            var tri9 = new Triangle(new Point3D(-size / 2, 0, 0), new Point3D(-size / 2, 0, size),
                new Point3D(-size / 2, size, 0), 5);
            var tri10 = new Triangle(new Point3D(-size / 2, size, size), new Point3D(-size / 2, 0, size),
                new Point3D(-size / 2, size, 0), 5);

            //right
            var tri11 = new Triangle(new Point3D(size / 2, 0, 0), new Point3D(size / 2, 0, size),
                new Point3D(size / 2, size, 0), 6);
            var tri12 = new Triangle(new Point3D(size / 2, size, size), new Point3D(size / 2, 0, size),
                new Point3D(size / 2, size, 0), 6);

            var mesh = new Mesh(new List<Triangle>()
                { tri1, tri2, tri3, tri4, tri5, tri6, tri7, tri8, tri9, tri10, tri11, tri12 });
            mesh.matrix3D.OffsetZ = -size / 2;
            mesh.matrix3D.OffsetY = -size / 2;
            mesh = new Mesh(mesh.GetWorldTriangles());

            return mesh;
        }
    }
    
    
}