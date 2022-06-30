using System;
using System.Windows.Media.Media3D;
using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class Rasterizer
    {
        private const int BG_COLOR = 0;
        private const double Z_BUFFER_EPS = 10e-10;
        private const ConsoleCharacter DEFAULT_CHAR = ConsoleCharacter.Full;
        
        private readonly ConsoleEngine consoleEngine;
        private readonly double[,] zBuffer;

        public Rasterizer(ConsoleEngine consoleEngine)
        {
            this.consoleEngine = consoleEngine;
            zBuffer = new double[consoleEngine.WindowSize.X, consoleEngine.WindowSize.Y];
        }

        public void ClearBuffer()
        {
            for (var i = 0; i < consoleEngine.WindowSize.X; i++)
            for (var j = 0; j < consoleEngine.WindowSize.Y; j++)
                zBuffer[i, j] = 1;

            consoleEngine.ClearBuffer();
        }

        public void DisplayBuffer()
        {
            consoleEngine.DisplayBuffer();
        }

        public void FillTriangle(Vector3D pa, Vector3D pb, Vector3D pc, int fgColor)
        {
            var a = pa.RoundTo2D();
            var b = pb.RoundTo2D();
            var c = pc.RoundTo2D();

            Point min = new Point(Math.Min(Math.Min(a.X, b.X), c.X), Math.Min(Math.Min(a.Y, b.Y), c.Y));
            Point max = new Point(Math.Max(Math.Max(a.X, b.X), c.X), Math.Max(Math.Max(a.Y, b.Y), c.Y));

            var area = Orient(a, b, c);

            Point p = new Point();
            for (p.Y = min.Y; p.Y < max.Y; p.Y++)
            {
                for (p.X = min.X; p.X < max.X; p.X++)
                {
                    int w0 = Orient(a, b, p);
                    int w1 = Orient(b, c, p);
                    int w2 = Orient(c, a, p);

                    if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                    {
                        float w0Area = (float)w0 / area;
                        float w1Area = (float)w1 / area;
                        float w2Area = (float)w2 / area;

                        var z = w1Area * pa.Z + pb.Z * w2Area + pc.Z * w0Area;
                        SetPixel(p, fgColor, BG_COLOR, DEFAULT_CHAR, z);
                    }
                }
            }
        }

        public void Triangle(Vector3D p1, Vector3D p2, Vector3D p3, int fgColor)
        {
            Line(p1, p2, fgColor);
            Line(p2, p3, fgColor);
            Line(p3, p1, fgColor);
        }

        public void Line(Vector3D p1, Vector3D p2, int fgColor )
        {
            var start = p1.RoundTo2D();
            var end = p2.RoundTo2D();

            Point delta = end - start;
            Point da = Point.Zero, db = Point.Zero;
            if (delta.X < 0) da.X = -1;
            else if (delta.X > 0) da.X = 1;
            if (delta.Y < 0) da.Y = -1;
            else if (delta.Y > 0) da.Y = 1;
            if (delta.X < 0) db.X = -1;
            else if (delta.X > 0) db.X = 1;
            int longest = Math.Abs(delta.X);
            int shortest = Math.Abs(delta.Y);

            if (!(longest > shortest))
            {
                longest = Math.Abs(delta.Y);
                shortest = Math.Abs(delta.X);
                if (delta.Y < 0) db.Y = -1;
                else if (delta.Y > 0) db.Y = 1;
                db.X = 0;
            }

            int numerator = longest >> 1;
            Point p = new Point(start.X, start.Y);
            for (int i = 0; i <= longest; i++)
            {
                var z = p1.Z * (1 - (double)i / longest ) + p2.Z * i / longest;
                SetPixel(p, fgColor, BG_COLOR, DEFAULT_CHAR, z);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    p += da;
                }
                else
                {
                    p += db;
                }
            }
        }

        private void SetPixel(Point p, int fgColor, int bgColor, ConsoleCharacter character, double z)
        {
            if (zBuffer[p.X, p.Y] - z > Z_BUFFER_EPS && z > 0)
            {
                zBuffer[p.X, p.Y] = z;
                consoleEngine.SetPixel(p, fgColor, bgColor, character);
            }
        }

        private int Orient(Point a, Point b, Point c)
        {
            return ((b.X - a.X) * (c.Y - a.Y)) - ((b.Y - a.Y) * (c.X - a.X));
        }
    }
}