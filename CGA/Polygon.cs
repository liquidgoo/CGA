using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public struct Polygon
    {
        public int length { get; }
        public Vector3[] vertices { get; }
        public  Vector3[] verticesTextures { get; }
        public Vector3[] verticesNormals { get; }

        public void setVertex(Vector3 vertex, int index)
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
            vertices = new Vector3[size];
            verticesTextures = new Vector3[size];
            verticesNormals = new Vector3[size];
        }
    }
}
