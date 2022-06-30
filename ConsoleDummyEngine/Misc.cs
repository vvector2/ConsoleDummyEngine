using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Serialization.Wavefront;

namespace ConsoleDummyEngine
{
    public static class Helpers
    {
        public static Mesh ReadObjFile(string path)
        {
            var reader = new ObjFileFormat();
            var scene = reader.LoadData(path);
            var child = (Polygon)scene.SceneContainer.Children[0];
            var vertices = child.Vertices.ToArray();
            var tris = new List<Triangle>();
            foreach (var f in child.Faces)
            {
                var currentTris = new List<Triangle>();
                for(var i = 1; i < f.Count - 1 ; i ++)
                    currentTris.Add(new Triangle(
                        vertices[f.Indices[0].Vertex].ToBaseVector3D(),
                        vertices[f.Indices[i].Vertex].ToBaseVector3D(),
                        vertices[f.Indices[i + 1].Vertex].ToBaseVector3D()));
                
                tris.AddRange(currentTris);
            }

            return new Mesh(tris);
        }

        public static Vector3D ToBaseVector3D(this Vertex v) => new Vector3D(v.X, v.Y, v.Z);

        public static Vector3D Transform3D(this Matrix3D m, Vector3D v) =>
            m.Transform(v) + new Vector3D(m.OffsetX, m.OffsetY, m.OffsetZ);

        public static Point RoundTo2D(this Vector3D p) => new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));

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
            var v = intensity * 16;
            var color = (int)Math.Floor(v);
            return color == 16 ? 15 : color;
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
                { tri1, tri2,  tri3, tri4, tri5, tri6, tri7, tri8, tri9, tri10, tri11, tri12 });

            return mesh;
        }
    }
}