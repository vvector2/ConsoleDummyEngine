using ConsoleGameEngine;

namespace ConsoleDummyEngine
{
    public class FrameBuffer
    {
        private readonly ConsoleEngine engine;
        private readonly PixelInfo[,] tmpBuffer;
        private readonly PixelInfo[,] buffer;
        public readonly int width;
        public readonly int height;

        public FrameBuffer(ConsoleEngine engine)
        {
            this.engine = engine;

            width = engine.WindowSize.X;
            height = engine.WindowSize.Y;

            this.buffer = new PixelInfo[width, height];
            this.tmpBuffer = new PixelInfo[width, height];

            ClearBuffer();
            ClearTmpBuffer();
        }

        private void ClearTmpBuffer()
        {
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                tmpBuffer[i, j] = new PixelInfo(0, 0);
        }

        public void ClearBuffer()
        {
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                buffer[i, j] = new PixelInfo(0, 0);

            engine.ClearBuffer();
        }

        public void Render()
        {
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                if (buffer[i, j].set)
                    engine.SetPixel(new Point(i, j), buffer[i, j].color, buffer[i, j].consoleCharacter);

            engine.DisplayBuffer();
        }

        public void Flush()
        {
            Flush(0, 0, width -1, height -1);
        }
        public void Flush(int x0, int y0 , int x1, int y1)
        {
            for (int i = x0; i <= x1; i++)
            for (int j = y0; j <= y1; j++)
                if (tmpBuffer[i, j].set && (!buffer[i, j].set || tmpBuffer[i, j].zBuffer < buffer[i, j].zBuffer))
                    buffer[i, j] = tmpBuffer[i, j];

            ClearTmpBuffer();
        }

        public void SetPixel(Point p, double zBuffer, int color, ConsoleCharacter consoleCharacter)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= width || p.Y >= height)
            {
                return;
            }

            var pixel = tmpBuffer[p.X, p.Y];

            pixel.color = color;
            pixel.zBuffer = zBuffer;
            pixel.consoleCharacter = consoleCharacter;
            pixel.set = true;

            tmpBuffer[p.X, p.Y] = pixel;
        }

        public PixelInfo[,] GetTmpBuffer() => tmpBuffer;
    }
}