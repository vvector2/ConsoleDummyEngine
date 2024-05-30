using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var renderer = new MyGame(86, 86, new OrthographicCamera());
            renderer.StartRender();
        }
    }

    public class MyGame : Renderer
    {
        private int i = 0;
        private TrackBallCameraControl control;
        public bool Debug { get; set; } = false;

        public MyGame(int width, int height, ICamera camera) : base(width, height, camera)
        {
            control = new TrackBallCameraControl(camera);
        }

        protected override void Setup()
        {
            base.Setup();
            // var mesh = Helpers.GetBox(1);
            // mesh.WireFrame = false;
            var mesh = Helpers.ReadObjFile("assets/car.obj");
            var rotMatrix = Matrix3D.Identity;
            
            rotMatrix.Scale(new Vector3D(0.3, 0.3, 0.3));
            rotMatrix.Translate(new Vector3D(0, 0, 1.5));

            mesh.Matrix3D = rotMatrix;
            AddMesh(mesh);
        }

        protected override void BeforeRender()
        {
            base.BeforeRender();
            control.Handle(
                consoleEngine.GetKeyDown(ConsoleKey.W), 
                consoleEngine.GetKeyDown(ConsoleKey.S),
                consoleEngine.GetKeyDown(ConsoleKey.A), 
                consoleEngine.GetKeyDown(ConsoleKey.D),
                consoleEngine.GetMousePos(), 
                consoleEngine.GetMouseLeft());
        }
    }
}