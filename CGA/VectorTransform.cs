using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public class VectorTransform
    {
        public Vector3 Scale(Vector3 original, Vector3 scale)
        {
            return new Vector3(original.x * scale.x, original.y * scale.y, original.z * scale.z);
        }

        public Vector4 RotateX(Vector4 v, float angle)
        {
            Matrix4x4 matrix = Matrix4x4.CreateRotationX(angle);
            return Vector4.Transform(v, matrix);
        }


        public Vector4 RotateY(Vector4 v, float angle)
        {
            Matrix4x4 matrix = Matrix4x4.CreateRotationY(angle);
            return Vector4.Transform(v, matrix);
        }

        public Vector4 RotateZ(Vector4 v, float angle)
        {
            Matrix4x4 matrix = Matrix4x4.CreateRotationZ(angle);
            return Vector4.Transform(v, matrix);
        }

        public Vector4 LocalToWorld(Vector4 original, Vector4 xAxis, Vector4 yAxis, Vector4 zAxis, Vector4 translation)
        {
            Matrix4x4 matrix = Matrix4x4.Identity;

            matrix.M11 = xAxis.X;
            matrix.M21 = xAxis.Y;
            matrix.M31 = xAxis.Z;
            matrix.M41 = xAxis.W;

            matrix.M12 = yAxis.X;
            matrix.M22 = yAxis.Y;
            matrix.M32 = yAxis.Z;
            matrix.M42 = yAxis.W;

            matrix.M13 = yAxis.X;
            matrix.M23 = yAxis.Y;
            matrix.M33 = yAxis.Z;
            matrix.M43 = yAxis.W;

            matrix.M14 = yAxis.X;
            matrix.M24 = yAxis.Y;
            matrix.M34 = yAxis.Z;
            matrix.M44 = yAxis.W;

            return Vector4.Transform(original, matrix);
        }

        public Vector4 WorldToView(Vector4 original, Vector4 eye, Vector4 target, Vector4 up)
        {
            Vector4 zAxis = Vector4.Normalize(target - eye);
            Vector4 xAxis = Vector4.Normalize(up.Cross(zAxis));
            Vector4 yAxis = up;

            Matrix4x4 matrix = Matrix4x4.Identity;

            matrix.M11 = xAxis.X;
            matrix.M21 = xAxis.Y;
            matrix.M31 = xAxis.Z;
            matrix.M41 = xAxis.W;

            matrix.M12 = yAxis.X;
            matrix.M22 = yAxis.Y;
            matrix.M32 = yAxis.Z;
            matrix.M42 = yAxis.W;

            matrix.M13 = yAxis.X;
            matrix.M23 = yAxis.Y;
            matrix.M33 = yAxis.Z;
            matrix.M43 = yAxis.W;

            matrix.M14 = Vector4.Dot(Vector4.Negate(xAxis), eye);
            matrix.M24 = Vector4.Dot(Vector4.Negate(yAxis), eye);
            matrix.M34 = Vector4.Dot(Vector4.Negate(zAxis), eye);

            return Vector4.Transform(original, matrix);
        }

        public Vector4 ViewToClip(Vector4 original, float width, float height, float zNear, float zFar, ProjectionMode projectionMode)
        {
            Matrix4x4 matrix = Matrix4x4.Identity;

            matrix.M11 = 2 / width;
            matrix.M22 = 2 / height;
            matrix.M33 = 1 / (zFar - zNear);
            matrix.M34 = -zNear / (zFar - zNear);

            if (projectionMode == ProjectionMode.Pespective)
            {
                //matrix[0, 0] *= zNear;
                //matrix[1, 1] *= zNear;
                matrix.M33 *= zFar;
                matrix.M34 *= zFar * zNear;
                matrix.M43 = 1;
                matrix.M44 = 0;
            }

            Vector4 result = Vector4.Transform(original, matrix);
            if (projectionMode == ProjectionMode.Pespective)
            {
                result /= result.W;
            }
            return result;
        }

        public Vector4 ViewToClipFOV(Vector4 original, float FOV, float aspect, float zNear, float zFar)
        {/*
            float height = 2 * MathF.Tan(FOV / 2);
            float width = aspect * height;
            return ViewToClip(width, height, zNear, zFar, ProjectionMode.Pespective);*/

            Matrix4x4 matrix = Matrix4x4.Identity;
            matrix.M22 = 1 / MathF.Tan(FOV / 2);
            matrix.M11 = matrix.M22 / aspect;


            matrix.M33 = zFar / (zFar - zNear);
            matrix.M34 = -zNear * zFar / (zFar - zNear);


            matrix.M43 = 1;
            matrix.M44 = 0;

            Vector4 result = Vector4.Transform(original, matrix);

            result /= result.W;
            return result;
        }

        public Vector4 ClipToScreen(Vector4 original, float width, float height, float xMin, float yMin)
        {
            Matrix4x4 matrix = Matrix4x4.Identity;

            matrix.M11 = width / 2;
            matrix.M22 = -height / 2;
            matrix.M14 = xMin + width / 2;
            matrix.M24 = yMin + height / 2;

            return Vector4.Transform(original, matrix);
        }

        public void updateFrom(Vector4 original, Vector4 other)
        {
            original.X = other.X;
            original.Y = other.Y;
            original.Z = other.Z;
            original.W = other.W;
        }

    }
}
