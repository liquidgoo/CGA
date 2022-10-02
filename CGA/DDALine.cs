using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    class DDALine : IRasterization
    {
        public IEnumerable<(int,int)> Rasterize(float x1, float y1, float x2, float y2)
        {
            int steps = (int) MathF.Round( MathF.MaxMagnitude(x2 - x1, y2 - y1));

            float x = x1;
            float y = y1;

            for (int i = 0; i < steps; i++)
            {
                x += (x2 - x1) / steps;
                y += (y2 - y1) / steps;
                yield return ((int)MathF.Round(x), (int)MathF.Round(y));
            }
        }
    }
}
