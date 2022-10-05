using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public class VectorTransform
    {
        public float magnitude(Vector3 v)
        {
            return MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }

        public Vector3 normalized(Vector3 v)
        {
            return v / magnitude(v);
        }

        public Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.y * v2.z - v1.z * v2.y,
                v1.z * v2.x - v1.x * v2.z,
                v1.x * v2.y - v1.y * v2.x);
        }
        public Vector3 Scale(Vector3 original, Vector3 scale)
        {
            return new Vector3(original.x * scale.x, original.y * scale.y, original.z * scale.z);
        }

        public Vector3 RotateX(Vector3 v, float angle)
        {
            Matrix4 matrix = Matrix4.One();
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            matrix[1, 1] = cos;
            matrix[1, 2] = -sin;
            matrix[2, 1] = sin;
            matrix[2, 2] = cos;
            return matrix * v;
        }


        public Vector3 RotateY(Vector3 v, float angle)
        {
            Matrix4 matrix = Matrix4.One();
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 2] = sin;
            matrix[2, 0] = -sin;
            matrix[2, 2] = cos;
            return matrix * v;
        }

        public Vector3 RotateZ(Vector3 v, float angle)
        {
            Matrix4 matrix = Matrix4.One();
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;
            return matrix * v;
        }

        public Vector3 LocalToWorld(Vector3 original, Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 translation)
        {
            Matrix4 matrix = Matrix4.One();
            for (int i = 0; i < 3; i++)
            {
                matrix[i, 0] = xAxis[i];
            }
            for (int i = 0; i < 3; i++)
            {
                matrix[i, 1] = yAxis[i];
            }
            for (int i = 0; i < 3; i++)
            {
                matrix[i, 2] = zAxis[i];
            }
            for (int i = 0; i < 3; i++)
            {
                matrix[i, 3] = translation[i];
            }
            return matrix * original;
        }

        public Vector3 WorldToView(Vector3 original, Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 zAxis = normalized(target - eye);
            Vector3 xAxis = normalized(Cross(up, zAxis));
            Vector3 yAxis = up;

            Matrix4 matrix = Matrix4.One();

            for (int i = 0; i < 3; i++)
            {
                matrix[0, i] = xAxis[i];
            }

            for (int i = 0; i < 3; i++)
            {
                matrix[1, i] = yAxis[i];
            }

            for (int i = 0; i < 3; i++)
            {
                matrix[2, i] = zAxis[i];
            }
            matrix[0, 3] = -xAxis * eye;
            matrix[1, 3] = -yAxis * eye;
            matrix[2, 3] = -zAxis * eye;

            return matrix * original;
        }

        public Vector3 ViewToClip(Vector3 original, float width, float height, float zNear, float zFar, ProjectionMode projectionMode)
        {
            Matrix4 matrix = Matrix4.One();

            matrix[0, 0] = 2 / width;
            matrix[1, 1] = 2 / height;
            matrix[2, 2] = 1 / (zFar - zNear);
            matrix[2, 3] = -zNear / (zFar - zNear);

            if (projectionMode == ProjectionMode.Pespective)
            {
                //matrix[0, 0] *= zNear;
                //matrix[1, 1] *= zNear;
                matrix[2, 2] *= zFar;
                matrix[2, 3] *= zFar * zNear;
                matrix[3, 2] = 1;
                matrix[3, 3] = 0;
            }

            Vector3 result = matrix * original;
            if (projectionMode == ProjectionMode.Pespective)
            {
                result /= result.w;
            }
            return result;
        }

        public Vector3 ViewToClipFOV(Vector3 original, float FOV, float aspect, float zNear, float zFar)
        {/*
            float height = 2 * MathF.Tan(FOV / 2);
            float width = aspect * height;
            return ViewToClip(width, height, zNear, zFar, ProjectionMode.Pespective);*/

            Matrix4 matrix = Matrix4.One();
            matrix[1, 1] = 1 / MathF.Tan(FOV / 2);
            matrix[0, 0] = matrix[1, 1] / aspect;


            matrix[2, 2] = zFar / (zFar - zNear);
            matrix[2, 3] = -zNear * zFar / (zFar - zNear);


            matrix[3, 2] = 1;
            matrix[3, 3] = 0;

            Vector3 result = matrix * original;

            result /= result.w;
            return result;
        }

        public Vector3 ClipToScreen(Vector3 original, float width, float height, float xMin, float yMin)
        {
            Matrix4 matrix = Matrix4.One();

            matrix[0, 0] = width / 2;
            matrix[1, 1] = -height / 2;
            matrix[0, 3] = xMin + width / 2;
            matrix[1, 3] = yMin + height / 2;

            return matrix * original;
        }

        public void updateFrom(Vector3 original, Vector3 other)
        {
            for (int i = 0; i < 4; i++)
            {
                original[i] = other[i];
            }
        }

    }
}
