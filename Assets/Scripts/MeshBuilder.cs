using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : ThreadedProcess
{
    // 0001 1111
    byte[] faces = new byte[Chunk.size.x * Chunk.size.y * Chunk.size.z];

    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    Vector3Int position;
    Block[] blocks;

    int sizeEstimate = 0;
    int vertexIndex = 0, trianglesIndex = 0;
    bool isVisible = false;

    public MeshBuilder (Vector3Int pos, Block[] blocks)
    {
        position = pos;
        this.blocks = blocks;
    }

    public override void ThreadFunction()
    {
        // Generate faces
        int index = 0;

        Chunk[] neighbours = new Chunk[6];
        bool[] exists = new bool[6];

        exists[0] = World.instance.GetChunkAt(position.x, position.y, position.z + Chunk.size.z, out neighbours[0]);
        exists[1] = World.instance.GetChunkAt(position.x + Chunk.size.x, position.y, position.z, out neighbours[1]);
        exists[2] = World.instance.GetChunkAt(position.x, position.y, position.z - Chunk.size.z, out neighbours[2]);
        exists[3] = World.instance.GetChunkAt(position.x - Chunk.size.x, position.y, position.z, out neighbours[3]);
        exists[4] = World.instance.GetChunkAt(position.x, position.y + Chunk.size.y, position.z, out neighbours[4]);
        exists[5] = World.instance.GetChunkAt(position.x, position.y - Chunk.size.y, position.z, out neighbours[5]);


        for (int x = 0; x < Chunk.size.x; x++)
        {
            for (int y = 0; y < Chunk.size.y; y++)
            {
                for (int z = 0; z < Chunk.size.z; z++)
                {
                    if (blocks[index].IsTransparent())
                    {
                        faces[index] = 0;
                        index++;
                        continue;
                    }

                    if (z == 0 && (exists [2] == false || neighbours[2].GetBlockAt(position.x + x, position.y + y, position.z + z - 1) == Block.Air))
                    {
                        faces[index] |= (byte)Directions.South;
                        sizeEstimate += 4;
                    }
                    else if (z > 0 && blocks[index - 1] == Block.Air)
                    {
                        faces[index] |= (byte)Directions.South;
                        sizeEstimate += 4;
                    }

                    if (z == Chunk.size.z - 1 && (exists[0] == false || neighbours[0].GetBlockAt(position.x + x, position.y + y, position.z + z + 1) == Block.Air))
                    {
                        faces[index] |= (byte)Directions.North;
                        sizeEstimate += 4;
                    }
                    else if (z < Chunk.size.z - 1 && blocks[index + 1] == Block.Air)
                    {
                        faces[index] |= (byte)Directions.North;
                        sizeEstimate += 4;
                    }

                    if (y == 0 && (exists[5] == false || neighbours[5].GetBlockAt(position.x + x, position.y + y  - 1, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Directions.Down;
                        sizeEstimate += 4;
                    }
                    else if (y > 0 && blocks[index - Chunk.size.z] == Block.Air)
                    {
                        faces[index] |= (byte)Directions.Down;
                        sizeEstimate += 4;
                    }

                    if (y == Chunk.size.y - 1 && (exists[4] == false || neighbours[4].GetBlockAt(position.x + x, position.y + y + 1, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Directions.Up;
                        sizeEstimate += 4;
                    }
                    else if (y < Chunk.size.y - 1 && blocks[index + Chunk.size.z] == Block.Air)
                    {
                        faces[index] |= (byte)Directions.Up;
                        sizeEstimate += 4;
                    }

                    if (x == 0 && (exists[3] == false || neighbours[3].GetBlockAt(position.x + x - 1, position.y + y, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Directions.West;
                        sizeEstimate += 4;
                    }
                    else if (x > 0 && blocks[index - Chunk.size.z * Chunk.size.y] == Block.Air)
                    {
                        faces[index] |= (byte)Directions.West;
                        sizeEstimate += 4;
                    }

                    if (x == Chunk.size.x - 1 && (exists[1] == false || neighbours[1].GetBlockAt(position.x + x + 1, position.y + y, position.z + z) == Block.Air))
                    {
                        faces[index] |= (byte)Directions.East;
                        sizeEstimate += 4;
                    }
                    else if (x < Chunk.size.x -1 && blocks[index + Chunk.size.z * Chunk.size.y] == Block.Air)
                    {
                        faces[index] |= (byte)Directions.East;
                        sizeEstimate += 4;
                    }

                    isVisible = true;

                    index++;
                }
            }
        }

        if (isVisible == false)
        {
            return;
        }

        // Generate mesh

        vertices = new Vector3[sizeEstimate];
        uvs = new Vector2[sizeEstimate];
        triangles = new int[(int)(sizeEstimate * 1.5f)];

        index = 0; ;

        for (int x = 0; x < Chunk.size.x; x++)
        {
            for (int y = 0; y < Chunk.size.y; y++)
            {
                for (int z = 0; z < Chunk.size.z; z++)
                {
                    if (faces [index] == 0)
                    {
                        index++;
                        continue;
                    }

                    if ((faces[index] & (byte)Directions.North) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures(blocks[index], Directions.North, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte)Directions.East) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                        triangles[trianglesIndex] = vertexIndex;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex + 1;

                        triangles[trianglesIndex + 3] = vertexIndex + 2;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Directions.East, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte)Directions.South) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y, z + position.z);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);

                        triangles[trianglesIndex] = vertexIndex;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex + 1;

                        triangles[trianglesIndex + 3] = vertexIndex + 2;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Directions.South, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte)Directions.West) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);

                        triangles[trianglesIndex] = vertexIndex + 1;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex;

                        triangles[trianglesIndex + 3] = vertexIndex + 1;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 2;

                        TextureController.AddTextures(blocks[index], Directions.West, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte)Directions.Up) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x, y + position.y + 1, z + position.z + 1);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y + 1, z + position.z + 1);

                        triangles[trianglesIndex] = vertexIndex;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex + 1;

                        triangles[trianglesIndex + 3] = vertexIndex + 2;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Directions.Up, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    if ((faces[index] & (byte)Directions.Down) != 0)
                    {
                        vertices[vertexIndex] = new Vector3(x + position.x, y + position.y, z + position.z);
                        vertices[vertexIndex + 1] = new Vector3(x + position.x, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 2] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);
                        vertices[vertexIndex + 3] = new Vector3(x + position.x + 1, y + position.y, z + position.z + 1);

                        triangles[trianglesIndex] = vertexIndex;
                        triangles[trianglesIndex + 1] = vertexIndex + 2;
                        triangles[trianglesIndex + 2] = vertexIndex + 1;

                        triangles[trianglesIndex + 3] = vertexIndex + 2;
                        triangles[trianglesIndex + 4] = vertexIndex + 3;
                        triangles[trianglesIndex + 5] = vertexIndex + 1;

                        TextureController.AddTextures(blocks[index], Directions.Down, vertexIndex, uvs);

                        vertexIndex += 4;
                        trianglesIndex += 6;
                    }

                    index++;
                }
            }
        }
    }

    public Mesh GetMesh (ref Mesh copy)
    {
        if (copy == null)
        {
            copy = new Mesh();
        }
        else
        {
            copy.Clear();
        }

        if (isVisible == false || vertexIndex == 0)
        {
            return copy;
        }

        if (vertexIndex == 65000)
        {
            copy.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        copy.vertices = vertices;
        copy.uv = uvs;
        copy.triangles = triangles;

        copy.RecalculateNormals();

        return copy;
    }
}
