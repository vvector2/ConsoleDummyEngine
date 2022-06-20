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

    public static class Helpers
    {
        public static Vector3D Transform3D(this Matrix3D m, Vector3D v) =>
            m.Transform(v) + new Vector3D(m.OffsetX, m.OffsetY, m.OffsetZ);

        public static Point FloorTo2D(this Vector3D p) => new Point((int)Math.Floor(p.X), (int)Math.Floor(p.Y));

        public static void SetWhiteAndGrayPalette(ConsoleEngine engine)
        {
            var colors = new List<Color>();
            for (var i = 0; i < 16; i++)
                colors.Add(new Color(255 - i * 15, 255 - i * 15, 255 - i * 15));

            colors.Reverse();
            engine.SetPalette(colors.ToArray());
        }

        public static int GetColor(double intensity)
        {
            intensity -= Double.Epsilon;
            
            var v = intensity * 16;
            var color = (int) Math.Floor(v) ;
            return color;
        }

        public static Mesh GetBox(double size)
        {
            var a = size / 2;

            //front
            var tri1 = new Triangle(
                new Vector3D(-a, -a, -a),
                new Vector3D(-a, a, -a),
                new Vector3D(a, a, -a));
            var tri2 = new Triangle(
                new Vector3D(-a, -a, -a),
                new Vector3D(a, a, -a),
                new Vector3D(a, -a, -a));

            //top
            var tri3 = new Triangle(
                new Vector3D(-a, a, -a),
                new Vector3D(-a, a, a),
                new Vector3D(a, a, a));
            var tri4 = new Triangle(
                new Vector3D(a, a, -a),
                new Vector3D(-a, a, -a),
                new Vector3D(a, a, a));

            //bottom
            var tri5 = new Triangle(
                new Vector3D(-a, -a, a),
                new Vector3D(-a, -a, -a),
                new Vector3D(a, -a, a));
            var tri6 = new Triangle(
                new Vector3D(-a, -a, -a),
                new Vector3D(a, -a, -a),
                new Vector3D(a, -a, a));

            //back
            var tri7 = new Triangle(
                new Vector3D(-a, a, a),
                new Vector3D(-a, -a, a),
                new Vector3D(a, a, a));
            var tri8 = new Triangle(
                new Vector3D(a, a, a),
                new Vector3D(-a, -a, a),
                new Vector3D(a, -a, a));

            // //left
            var tri9 = new Triangle(
                new Vector3D(-a, -a, -a),
                new Vector3D(-a, -a, a),
                new Vector3D(-a, a, -a));
            var tri10 = new Triangle(
                new Vector3D(-a, -a, a),
                new Vector3D(-a, a, a),
                new Vector3D(-a, a, -a));

            //right
            var tri11 = new Triangle(
                new Vector3D(a, -a, a),
                new Vector3D(a, -a, -a),
                new Vector3D(a, a, -a));
            var tri12 = new Triangle(
                new Vector3D(a, a, a),
                new Vector3D(a, -a, a),
                new Vector3D(a, a, -a));

            var mesh = new Mesh(new List<Triangle>()
                { tri1, tri2, tri3, tri4, tri5, tri6, tri7, tri8, tri9, tri10, tri11, tri12 });

            return mesh;
        }
    }
}