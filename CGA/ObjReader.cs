using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CGA
{
    public class ObjReader
    {
        public Vector4[] vertices;
        private List<Vector4> v = new List<Vector4>();
        public List<Vector3> verticesTextures { get; }
        public List<Vector3> verticesNormals { get; }
        public List<Polygon> polygons { get; set; }


        private const int VERTEX_TEXTURE_LENGTH = 3;
        public ObjReader copy()
        {
            ObjReader objReader = new ObjReader();


            vertices.CopyTo(objReader.vertices, 0);



            objReader.polygons = polygons;
/*            foreach(Polygon poly in polygons)
            {
                Polygon newPoly = new Polygon(poly.length);
                for (int i = 0; i < poly.length; i++)
                {
                    newPoly.vertices[i] = objReader.vertices[poly.ind[i]];
                }
                objReader.polygons.Add(newPoly);
            }*/

            return objReader;
        }
        private void readVertex(string[] tokens)
        {
            v.Add(new Vector4(
                float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]), 1f));
            //TODO 4?
        }

        private void readVertexTexture(string[] tokens)
        {
            if (tokens.Length < 4)
            {
                string[] temp = tokens;
                tokens = new string[VERTEX_TEXTURE_LENGTH + 1];
                temp.CopyTo(tokens, 0);   
                for (int i = temp.Length; i < tokens.Length; i++)
                {
                    tokens[i] = "0";
                }
            }

            verticesTextures.Add(new Vector3(
                float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
        }
        private void readVertexNormal(string[] tokens)
        {
            verticesNormals.Add(new Vector3(
                float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
        }
        private void readPolygon(string[] tokens)
        {
            Polygon polygon = new Polygon(tokens.Length - 1);

            for (int i = 1; i < tokens.Length; i++)
            {
                string[] verticesIndices = tokens[i].Split('/');

                int vertexIndex = int.Parse(verticesIndices[0]);
                polygon.setVertex(vertices[vertexIndex > 0 ? vertexIndex - 1 : v.Count - vertexIndex], i -1);
                polygon.ind[i - 1] = vertexIndex - 1;

                int vertexTextureIndex = verticesIndices.Length > 1 && !verticesIndices[1].Equals("") ?
                    int.Parse(verticesIndices[1]) : 0;
                //TODO NaN?
                Vector3 vertexTexture = vertexTextureIndex == 0 ? new Vector3(0, 0, 0) :
                    verticesTextures[vertexTextureIndex > 0 ? vertexTextureIndex - 1 : verticesTextures.Count - vertexTextureIndex];
                polygon.setVertexTexture(vertexTexture, i - 1);


                int vertexNormalIndex = verticesIndices.Length > 2 ? int.Parse(verticesIndices[2]) : 0;
                //TODO NaN?
                Vector3 vertexNormal = vertexNormalIndex == 0 ? new Vector3(0, 0, 0) :
                    verticesNormals[vertexNormalIndex > 0 ? vertexNormalIndex - 1 : verticesNormals.Count - vertexNormalIndex];
                polygon.setVertexNormal(vertexNormal, i - 1);


                polygons.Add(polygon);
            }
        }

       
        private void readLine(string line)
        {
            string[] tokens = line.Split(' ');

            switch (tokens[0]) 
            {
                case "v": 
                    readVertex(tokens);
                    break;
                case "vt":
                    readVertexTexture(tokens);
                    break;
                case "vn":
                    readVertexNormal(tokens);
                    break;
                case "f":
                    readPolygon(tokens);
                    break;
                default:
                    break;
            }
        }
        public ObjReader(string file) : this()
        {

            foreach (string line in File.ReadAllLines(file))
            {
                readLine(line);
            }
            vertices = new Vector4[v.Count];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = v[i];
            }
        }
        public ObjReader()
        {
            verticesTextures = new List<Vector3>();
            verticesNormals = new List<Vector3>();
            polygons = new List<Polygon>();
        }
    }
}
