using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public interface IRasterization
    {
        public IEnumerable<(int, int)> Rasterize(float x1, float y1, float x2, float y2);
    }
}
