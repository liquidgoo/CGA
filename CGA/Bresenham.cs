using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    class Bresenham : IRasterization
    {
        public IEnumerable<(int, int)> Rasterize(float x1, float y1, float x2, float y2)
        {
            if (Math.Abs(y2 - y1) > Math.Abs(x2 - x1))
            {
                foreach((int, int) point in Rasterize(y1, x1, y2, x2) )
                {
                    yield return (point.Item2, point.Item1);
                }
                yield break;
            }
            if (y1 > y2)
            {
                float temp = y2;
                y2 = y1;
                y1 = temp;
                temp = x2;
                x2 = x1;
                x1 = temp;
            }


            int slope = (int)MathF.Round(y2 - y1);
            int threshold = (int) MathF.Abs( MathF.Round(x2 - x1));

            int deltaX = x2 - x1 > 0 ? 1 : -1;

            int x = (int)MathF.Round(x1);
            int y = (int)MathF.Round(y1);
            int error = 0;
            while (x != (int) MathF.Round(x2))
            {
                yield return (x, y);

                x += deltaX;
                error += slope;
                if (2 * error >= threshold)
                {
                    y++;
                    error -= threshold;
                }
            }
        }
    }
}
