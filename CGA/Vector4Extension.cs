using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public static class Vector4Extension
    {
        public static Vector4 Cross(this Vector4 v1, Vector4 v2)
        {
            System.Numerics.Vector3 tempV1 = new System.Numerics.Vector3(v1.X, v1.Y, v1.Z);
            System.Numerics.Vector3 tempV2 = new System.Numerics.Vector3(v2.X, v2.Y, v2.Z);
            System.Numerics.Vector3 tempResult = System.Numerics.Vector3.Cross(tempV1, tempV2);
            return new Vector4(tempResult, v1.W * v2.W);
        }
    }
}
