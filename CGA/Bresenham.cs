using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    class Bresenham : IRasterization
    {
        public IEnumerator<(int, int)> Rasterize(float x1, float y1, float x2, float y2)
        {
            int slope = (int)MathF.Round(y2 - y1);
            int threshold = (int)MathF.Round(x2 - x1);

            int deltaX = x2 - x1 > 0 ? 1 : -1;
            int deltaY = y2 - y1 > 0 ? 1 : -1;
            int x = (int)MathF.Round(x1);
            int y = (int)MathF.Round(y1);
            int error = 0;
            while (x != (int) MathF.Round(x2))
            {
                yield return (x, y);

                x += deltaX;
                error += slope;
                if (2 * error > threshold)
                    y += deltaY;
            }
        }
    }
}
