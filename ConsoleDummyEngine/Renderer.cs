using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class Renderer
    {
        private const double FAR = 50;
        private const double NEAR = 0.1;
        
        private const double LEFT = -2;
        private const double RIGHT = 2;
        private const double TOP = 1.5;
        private const double BOTTOM = -1.5;

        private const double FOE = 60;
        
        private readonly int width = 203;
        private readonly int height = 203;
        
        private readonly ConsoleEngine consoleEngine;
        private readonly FrameBuffer frameBuffer;
        private readonly Rasterizer rasterizer;

        private readonly List<Mesh> meshes = new List<Mesh>();

        
        public delegate void BeforeRender();
        public BeforeRender beforeRender;
        public Renderer(int width, int height)
        {
            this.width = width;
            this.height = height;
            
            consoleEngine = new ConsoleEngine(width, height, 4, 3);
            frameBuffer = new FrameBuffer(consoleEngine);
            rasterizer = new Rasterizer(frameBuffer);
        }

        private void Setup()
        {
            consoleEngine.Borderless();
        }
        
        private Point3D ProjectionOrthographic(Point3D p)
        {
            var xClip = p.X;
            var yClip = p.Y;
            
            var xNdc = 0.5 + xClip / ((RIGHT - LEFT) / 2);
            var yNdc = 0.5 - yClip / ((TOP - BOTTOM) / 2);

            return new Point3D(xNdc * width, yNdc * height, p.Z);
        }
        
        private Point3D ProjectionPerspective(Point3D p)
        {
            var xClip = (NEAR / p.Z) * p.X;
            var yClip = (NEAR / p.Z) * p.Y;
            
            var halfFrustumSquare = Math.Tan(FOE / 2 * Math.PI / 180) * NEAR;
            
            var xNdc = 0.5 + xClip / halfFrustumSquare;
            var yNdc = 0.5 - yClip / halfFrustumSquare;

            return new Point3D(xNdc * width, yNdc * height, p.Z);
        }

        private void DrawMesh(Mesh mesh)
        {
            var triangles = mesh.GetWorldTriangles();
            foreach (var tri in triangles)
            {
                var p1 = ProjectionOrthographic(tri.p1);
                var p2 = ProjectionOrthographic(tri.p2);
                var p3 = ProjectionOrthographic(tri.p3);
                
                if (mesh.WireFrame)
                    rasterizer.DrawWireframeTriangle(p1,p2,p3);
                else
                    rasterizer.DrawFillTriangle(p1,p2,p3, tri.color);
                
            }
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
                beforeRender();
                foreach (var mesh in meshes)
                {
                    DrawMesh(mesh);
                }

                frameBuffer.Render();
                frameBuffer.ClearBuffer();
            }
        }

    }
}