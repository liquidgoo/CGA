using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public class Polygon
    {
        public int length { get; }
        public Vector4[] vertices { get; }
        public  Vector3[] verticesTextures { get; }
        public Vector3[] verticesNormals { get; }
        public int[] ind;
        public void setVertex(Vector4 vertex, int index)
        {
            vertices[index] = vertex;
        }

        public void setVertexTexture(Vector3 vertexTexture, int index)
        {
            verticesTextures[index] = vertexTexture;
        }

        public void setVertexNormal(Vector3 vertexNormal, int index)
        {
            verticesNormals[index] = vertexNormal;
        }

        public Polygon(int size)
        {
            this.length = size;
            vertices = new Vector4[size];
            verticesTextures = new Vector3[size];
            verticesNormals = new Vector3[size];
            ind = new int[size];
        }
    }
}
