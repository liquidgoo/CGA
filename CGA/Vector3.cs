using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{

    public enum ProjectionMode
    {
        Orthographic,
        Pespective
    }
    public class Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public  float w;

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2: 
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public float magnitude
        {
            get
            {
                return MathF.Sqrt(x * x + y * y + z * z);
            }
        }
        public Vector3 normalized
        {
            get
            {
                return this / magnitude;
            }
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return a + (-b);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }
        public static float operator *(Vector3 a, Vector3 b)
        {
            return a.Dot(b);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return a * (1 / b);
        }


        public float Dot(Vector3 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public  Vector3 Cross(Vector3 other)
        {
            return new Vector3(y * other.z - z * other.y,
                z * other.x - x * other.z,
                x * other.y - y * other.x);
        }
        public Vector3 Scale(Vector3 scale)
        {
            return new Vector3(x * scale.x, y * scale.y, z * scale.z);
        }
        
        public Vector3 RotateX(float angle)
        {
            Matrix4 matrix = Matrix4.One();
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            matrix[1, 1] = cos;
            matrix[1, 2] = -sin;
            matrix[2, 1] = sin;
            matrix[2, 2] = cos;
            return matrix * this;
        }


        public Vector3 RotateY(float angle)
        {
            Matrix4 matrix = Matrix4.One();
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 2] = sin;
            matrix[2, 0] = -sin;
            matrix[2, 2] = cos;
            return matrix * this;
        }

        public Vector3 RotateZ(float angle)
        {
            Matrix4 matrix = Matrix4.One();
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;
            return matrix * this;
        }

        public Vector3 LocalToWorld(Vector3 xAxis, Vector3 yAxis, Vector3 zAxis, Vector3 translation)
        {
            Matrix4 matrix = Matrix4.One();
            for (int i = 0; i < 3; i++)
            {
                matrix[i, 0] = xAxis[i];
            }
            for (int i = 0; i < 3; i++)
            {
                matrix[i,1] = yAxis[i];
            }
            for (int i = 0; i < 3; i++)
            {
                matrix[i,2] = zAxis[i];
            }
            for (int i = 0; i < 3; i++)
            {
                matrix[i,3] = translation[i];
            }
            return matrix * this;
        }

        public Vector3 WorldToView(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 zAxis = (target - eye).normalized;
            Vector3 xAxis = (up.Cross(zAxis)).normalized;
            Vector3 yAxis = up;

            Matrix4 matrix = Matrix4.One();

            for (int i =0; i < 3; i++)
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

            return matrix * this;
        }

        public Vector3 ViewToClip(float width, float height, float zNear, float zFar, ProjectionMode projectionMode)
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
                matrix[2, 2] *= zFar ;
                matrix[2, 3] *= zFar * zNear;
                matrix[3, 2] = 1;
                matrix[3, 3] = 0;
            }

            Vector3 result = matrix * this;
            if (projectionMode == ProjectionMode.Pespective)
            {
                result /= result.w;
            }
            return result;
        }

        public Vector3 ViewToClipFOV(float FOV, float aspect, float zNear, float zFar)
        {/*
            float height = 2 * MathF.Tan(FOV / 2);
            float width = aspect * height;
            return ViewToClip(width, height, zNear, zFar, ProjectionMode.Pespective);*/

            Matrix4 matrix = Matrix4.One();
            matrix[1, 1] = 1/ MathF.Tan(FOV/2);
            matrix[0, 0] = matrix[1, 1]/aspect;


            matrix[2, 2] =  zFar / (zFar - zNear);
            matrix[2, 3] = -zNear * zFar / (zFar - zNear);


                matrix[3, 2] = 1;
                matrix[3, 3] = 0;

            Vector3 result = matrix * this;
 
            result /= result.w;
            return result;
        }

        public Vector3 ClipToScreen(float width, float height, float xMin, float yMin)
        {
            Matrix4 matrix = Matrix4.One();

            matrix[0,0] = width / 2;
            matrix[1, 1] = -height / 2;
            matrix[0, 3] = xMin + width / 2;
            matrix[1, 3] = yMin + height / 2;

            return matrix * this;
        }

        public void updateFrom(Vector3 other)
        {
            for (int i =0; i < 4; i++)
            {
                this[i] = other[i];
            }
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 1;
        }
        public Vector3(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector3() : this(0,0,0) { }

    }
}
