using UnityEngine;
using System.Collections.Generic;

public class MarchingCubes
{
    public Mesh CreateMesh(float[,,] data, float surfaceLevel)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        int width = data.GetLength(0);
        int height = data.GetLength(1);
        int depth = data.GetLength(2);

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                for (int z = 0; z < depth - 1; z++)
                {
                    Vector3[] cornerPositions = new Vector3[8];
                    float[] cornerValues = new float[8];
                    for(int i = 0; i < 8; i++)
                    {
                        cornerPositions[i] = new Vector3(x + VoxelTables.corners[i, 0], y + VoxelTables.corners[i, 1], z + VoxelTables.corners[i, 2]);
                        cornerValues[i] = data[(int)cornerPositions[i].x, (int)cornerPositions[i].y, (int)cornerPositions[i].z];
                    }
                    
                    int cubeIndex = 0;
                    if (cornerValues[0] < surfaceLevel) cubeIndex |= 1;
                    if (cornerValues[1] < surfaceLevel) cubeIndex |= 2;
                    if (cornerValues[2] < surfaceLevel) cubeIndex |= 4;
                    if (cornerValues[3] < surfaceLevel) cubeIndex |= 8;
                    if (cornerValues[4] < surfaceLevel) cubeIndex |= 16;
                    if (cornerValues[5] < surfaceLevel) cubeIndex |= 32;
                    if (cornerValues[6] < surfaceLevel) cubeIndex |= 64;
                    if (cornerValues[7] < surfaceLevel) cubeIndex |= 128;

                    for (int i = 0; VoxelTables.triTable[cubeIndex, i] != -1; i += 3)
                    {
                        int edgeIndex1 = VoxelTables.triTable[cubeIndex, i];
                        int edgeIndex2 = VoxelTables.triTable[cubeIndex, i + 1];
                        int edgeIndex3 = VoxelTables.triTable[cubeIndex, i + 2];

                        vertices.Add(Interpolate(cornerPositions[VoxelTables.edgeConnections[edgeIndex1, 0]], cornerValues[VoxelTables.edgeConnections[edgeIndex1, 0]], cornerPositions[VoxelTables.edgeConnections[edgeIndex1, 1]], cornerValues[VoxelTables.edgeConnections[edgeIndex1, 1]], surfaceLevel));
                        vertices.Add(Interpolate(cornerPositions[VoxelTables.edgeConnections[edgeIndex2, 0]], cornerValues[VoxelTables.edgeConnections[edgeIndex2, 0]], cornerPositions[VoxelTables.edgeConnections[edgeIndex2, 1]], cornerValues[VoxelTables.edgeConnections[edgeIndex2, 1]], surfaceLevel));
                        vertices.Add(Interpolate(cornerPositions[VoxelTables.edgeConnections[edgeIndex3, 0]], cornerValues[VoxelTables.edgeConnections[edgeIndex3, 0]], cornerPositions[VoxelTables.edgeConnections[edgeIndex3, 1]], cornerValues[VoxelTables.edgeConnections[edgeIndex3, 1]], surfaceLevel));
                        
                        triangles.Add(vertices.Count - 3);
                        triangles.Add(vertices.Count - 2);
                        triangles.Add(vertices.Count - 1);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private Vector3 Interpolate(Vector3 p1, float v1, Vector3 p2, float v2, float surfaceLevel)
    {
        float t = (surfaceLevel - v1) / (v2 - v1);
        return p1 + t * (p2 - p1);
    }
}