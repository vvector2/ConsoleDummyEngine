using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class Rasterizer
    {
        private readonly FrameBuffer frameBuffer;

        public Rasterizer(FrameBuffer frameBuffer)
        {
            this.frameBuffer = frameBuffer;
        }

        public void DrawWireframeTriangle(Point3D p1, Point3D p2, Point3D p3)
        {
            DrawLine(p1, p2);
            DrawLine(p2, p3);
            DrawLine(p3, p1);

            frameBuffer.Flush();
        }

        public void DrawFillTriangle(Point3D p12D, Point3D p22D, Point3D p32D, int color)
        {
            DrawLine(p12D, p22D, color);
            DrawLine(p22D, p32D, color);
            DrawLine(p32D, p12D, color);

            var tmpBuffer = frameBuffer.GetTmpBuffer();

            var listPoint = new List<Point>();
            for (int i = 0; i < frameBuffer.width; i++)
            for (int j = 0; j < frameBuffer.height; j++)
                if (tmpBuffer[i, j].set)
                    listPoint.Add(new Point(i, j));
            
            if (listPoint.Count == 0)
                return;

            var minX = listPoint.Select(p => p.X).Min();
            var minY = listPoint.Select(p => p.Y).Min();

            var maxX = listPoint.Select(p => p.X).Max();
            var maxY = listPoint.Select(p => p.Y).Max();

            for (int i = minY; i <= maxY; i++)
            {
                int start = -1;
                int end = -1;
                for (int j = minX; j <= maxX; j++)
                {
                    if (tmpBuffer[j, i].set && start == -1)
                    {
                        start = j;
                        end = j;
                        continue;
                    }

                    if (start != -1 && tmpBuffer[j, i].set)
                    {
                        end = j;
                    }
                }

                if (start == -1)
                    continue;

                var startPixel = tmpBuffer[start, i];
                var endPixel = tmpBuffer[end, i];
                double len = end - start + 1;

                for (int k = start + 1; k < end; k++)
                {
                    var zBuffer = startPixel.zBuffer * (1 - ((k - start) / len)) +
                                  endPixel.zBuffer * (k - start) / len;
                    frameBuffer.SetPixel(new Point(k, i), zBuffer, color, ConsoleCharacter.Full);
                }
            }

            frameBuffer.Flush(minX, minY, maxX, maxY);
        }

        private void DrawLine(Point3D p1, Point3D p2, int color = 4,
            ConsoleCharacter consoleCharacter = ConsoleCharacter.Full)
        {
            if (p2.X - p1.X < 0)
            {
                Point3D temp = p1;
                p1 = p2;
                p2 = temp;
            }
            
            var deltaX = p2.X - p1.X;
            var deltaY = p2.Y - p1.Y;

            var deltaXAbs = Math.Abs(deltaX);
            var deltaYAbs = Math.Abs(deltaY);

            var steps = deltaXAbs > deltaYAbs ? deltaXAbs : deltaYAbs;

            var incrementX = deltaX / steps;
            var incrementY = deltaY / steps;

            var currentPoint = p1.Clone();
            for (var i = 0; i <= steps; i++)
            {
                var drawPoint = currentPoint.FloorTo2D();
                var zBuffer = p1.Z * (1 - (i / steps)) + p2.Z * (i / steps);
                frameBuffer.SetPixel(drawPoint, zBuffer, color, consoleCharacter);

                currentPoint.X += incrementX;
                currentPoint.Y += incrementY;
            }
            frameBuffer.SetPixel(p2.FloorTo2D(), p2.Z, color, consoleCharacter);
        }
    }
}