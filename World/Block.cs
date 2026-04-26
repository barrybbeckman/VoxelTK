using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTK.World
{
    internal class Block
    {
        public Vector3 position;

        public Dictionary<Faces, FaceData> faces;

        public List<Vector2> dirtUV = new List<Vector2>
        {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        };
        public Block(Vector3 position)
        {
            this.position = position;

            faces = new Dictionary<Faces, FaceData>
            {
                {Faces.Front, new FaceData {
                    vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.Front]),
                    uv = dirtUV
                } },
                {Faces.Back, new FaceData {
                    vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.Back]),
                    uv = dirtUV
                } },
                {Faces.Left, new FaceData {
                    vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.Left]),
                    uv = dirtUV
                } },
                {Faces.Right, new FaceData {
                    vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.Right]),
                    uv = dirtUV
                } },
                {Faces.Top, new FaceData {
                    vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.Top]),
                    uv = dirtUV
                } },
                {Faces.Bottom, new FaceData {
                    vertices = AddTransformedVertices(FaceDataRaw.rawVertexData[Faces.Bottom]),
                    uv = dirtUV
                } },

            };
        }
        public List<Vector3> AddTransformedVertices(List<Vector3> vertices)
        {
            List<Vector3> transformedVertices = new List<Vector3>();
            foreach (var vert in vertices)
            {
                transformedVertices.Add(vert + position);
            }
            return transformedVertices;
        }
        public FaceData GetFace(Faces face)
        {
            return faces[face];
        }
    }
}