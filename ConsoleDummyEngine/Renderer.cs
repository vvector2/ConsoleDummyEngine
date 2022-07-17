using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class Renderer
    {
        private readonly int width;
        private readonly int height;

        protected readonly ICamera camera;
        protected readonly ConsoleEngine consoleEngine;
        protected readonly List<Mesh> meshes = new List<Mesh>();

        private readonly Rasterizer rasterizer;

        public Renderer(int width, int height, ICamera camera)
        {
            this.width = width;
            this.height = height;
            this.camera = camera;

            consoleEngine = new ConsoleEngine(width, height, 4, 3);
            rasterizer = new Rasterizer(consoleEngine);

            Helpers.SetWhiteAndGrayPalette(consoleEngine);
        }

        private Vector3D Projection(Vector3D p)
        {
            var ndc = camera.Project(p);
            return new Vector3D(ndc.X * width, ndc.Y * height , ndc.Z);
        }

        private List<Triangle> ClipTriangles(IEnumerable<Triangle> triangles)
        {
            var trianglesList = triangles.ToList();
            var planes = camera.GetClippingPlanes();
            foreach (var plane in planes)
            {
                var trisCopy = trianglesList.ToArray();
                trianglesList = new List<Triangle>();
                foreach (var tri in trisCopy)
                {
                    var insidePoint = new List<Vector3D>();
                    var outsidePoint = new List<Vector3D>();

                    var d1 = Helpers.CalcSignedDistanceBetweenPlaneAndPoint(plane.N, plane.P, tri.p1);
                    var d2 = Helpers.CalcSignedDistanceBetweenPlaneAndPoint(plane.N, plane.P, tri.p2);
                    var d3 = Helpers.CalcSignedDistanceBetweenPlaneAndPoint(plane.N, plane.P, tri.p3);

                    if (d1 >= 0)
                        insidePoint.Add(tri.p1);
                    else
                        outsidePoint.Add(tri.p1);
                    
                    if (d2 >= 0)
                        insidePoint.Add(tri.p2);
                    else
                        outsidePoint.Add(tri.p2);
                    
                    if (d3 >= 0)
                        insidePoint.Add(tri.p3);
                    else
                        outsidePoint.Add(tri.p3);
                    

                    switch (insidePoint.Count)
                    {
                        case 0:
                            break;
                        case 3:
                            trianglesList.Add(tri);
                            break;
                        case 1:
                            if (d1 < 0 && d3 < 0)
                            {
                                trianglesList.Add(new Triangle(insidePoint[0],
                                    Helpers.IntersectPoint(outsidePoint[1], insidePoint[0], plane.N, plane.P),
                                    Helpers.IntersectPoint(outsidePoint[0], insidePoint[0], plane.N, plane.P)));
                            }else
                                trianglesList.Add(new Triangle(insidePoint[0],
                                Helpers.IntersectPoint(outsidePoint[0], insidePoint[0], plane.N, plane.P),
                                Helpers.IntersectPoint(outsidePoint[1], insidePoint[0], plane.N, plane.P)));
                            break;
                        case 2:
                            var p1 = Helpers.IntersectPoint(insidePoint[0], outsidePoint[0], plane.N, plane.P);
                            var p2 = Helpers.IntersectPoint(insidePoint[1], outsidePoint[0], plane.N, plane.P);

                            if (d2 > 0 )
                            {
                                trianglesList.Add(new Triangle(p1, insidePoint[0], insidePoint[1]));
                                trianglesList.Add(new Triangle(p1, insidePoint[1], p2));
                            }
                            else
                            {
                                trianglesList.Add(new Triangle(p1,insidePoint[1], insidePoint[0] ));
                                trianglesList.Add(new Triangle(p1,p2, insidePoint[1]));   
                            }
                            break;
                    }
                }
            }

            return trianglesList;
        }

        private IEnumerable<Triangle> ToEyeTriangle(IEnumerable<Triangle> tris)
        {
            var inverseCameraMatrix = camera.Matrix;
            inverseCameraMatrix.Invert();
            return tris.Select(t => t.Transform(inverseCameraMatrix));
        } 

        private void DrawMesh(Mesh mesh)
        {
            var triangles = mesh.GetWorldTriangles();
            var eyeTriangles = ToEyeTriangle(triangles);
            var clippedTris = ClipTriangles(eyeTriangles);
            
            foreach (var tri in clippedTris)
            {
                var cameraDotProduct = Vector3D.DotProduct(tri.normal, new Vector3D(0, 0, -1));

                if (cameraDotProduct < 0)
                    continue;

                var p1 = Projection(tri.p1);
                var p2 = Projection(tri.p2);
                var p3 = Projection(tri.p3);

                if (mesh.WireFrame)
                    rasterizer.Triangle(p1, p2, p3, 15);
                else
                {
                    var color = Helpers.GetColor(cameraDotProduct);
                    rasterizer.FillTriangle(p1, p2, p3, color);
                }
            }
        }

        protected virtual void BeforeRender()
        {
        }

        protected virtual void Setup()
        {
            consoleEngine.Borderless();
        }

        public void AddMesh(params Mesh[] mesh)
        {
            meshes.AddRange(mesh);
        }

        public void StartRender()
        {
            Setup();
            while (true)
            {
                BeforeRender();
                foreach (var mesh in meshes)
                {
                    DrawMesh(mesh);
                }

                rasterizer.DisplayBuffer();
                rasterizer.ClearBuffer();
            }
        }
    }
}