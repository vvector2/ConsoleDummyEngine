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
            return new Vector3D(ndc.X * width, ndc.Y * height, ndc.Z);
        }

        private void DrawMesh(Mesh mesh)
        {
            var triangles = mesh.GetWorldTriangles();
            foreach (var tri in triangles)
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