using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : ThreadedProcess
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
        this.position = pos;
        this.blocks = blocks;
    }

    protected override ThreadFunction()
    {

    }
}
