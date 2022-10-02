using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    class Matrix4
    {
        private const int SIZE = 4;
        private float[,] matrix = new float[SIZE, SIZE];


        public float this[int index1, int index2]
        {
            get
            {
                return matrix[index1, index2];
            }
            set
            {
                matrix[index1, index2] = value;
            }
        }

        public static Matrix4 operator *(Matrix4 a, Matrix4 b )
        {
            Matrix4 matrix = new Matrix4();
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    for (int k = 0; k < SIZE; k++)
                    {
                        matrix[i,j] += a[i, k] * b[k, j];
                    }
                }
            }
            return matrix;
        }

        public static Vector3 operator *(Matrix4 matrix, Vector3 vector)
        {
            /* Matrix4 vectorMatrix = new Matrix4();
             for (int i = 0; i < 4; i++)
             {
                 vectorMatrix[i, 0] = vector[i];
             }
             //vectorMatrix[3, 0] = 1;

             Matrix4 resultMatrix =  matrix * vectorMatrix;

             return new Vector3(resultMatrix[0, 0], resultMatrix[1, 0], resultMatrix[2, 0], resultMatrix[3,0]);*/
            Vector3 result = new Vector3();
            result.x = matrix[0, 0] * vector.x + matrix[0, 1] * vector.y + matrix[0, 2] * vector.z + matrix[0, 3] * vector.w;
            result.y = matrix[1, 0] * vector.x + matrix[1, 1] * vector.y + matrix[1, 2] * vector.z + matrix[1, 3] * vector.w;
            result.z = matrix[2, 0] * vector.x + matrix[2, 1] * vector.y + matrix[2, 2] * vector.z + matrix[2, 3] * vector.w;
            result.w = matrix[3, 0] * vector.x + matrix[3, 1] * vector.y + matrix[3, 2] * vector.z + matrix[3, 3] * vector.w;
            return result;
        }

        public static Matrix4 One()
        {
            Matrix4 matrix = new Matrix4();
            for (int i = 0; i < SIZE; i++)
            {
                matrix[i,i] = 1;
            }
            return matrix;
        }
    }
}
