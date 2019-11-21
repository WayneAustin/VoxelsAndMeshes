using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static Vector3Int size = new Vector3Int(16, 32, 16);
    public Mesh mesh;
    public bool ready = false;
    public Vector3Int position;

    Block[] blocks;

    public Chunk (Vector3Int pos)
    {
        position = pos;
    }

    public void GenerateBlockArray ()
    {
        blocks = new Block[size.x * size.y * size.z];
        int index = 0;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    int value = Mathf.CeilToInt(Mathf.PerlinNoise(x / 32f, z / 32f) * 15f + 20f);

                    if (y > value)
                    {
                        index++;
                        continue;
                    }

                    if (y == value)
                    {
                        blocks[index] = Block.Grass;
                    }

                    if (y < value && y > value - 3)
                    {
                        blocks[index] = Block.Dirt;
                    }

                    if (y <= value - 3)
                    {
                        blocks[index] = Block.Stone;
                    }

                    index++;
                }
            }
        }
    }

    public IEnumerator GenerateMesh ()
    {
        MeshBuilder builder = new MeshBuilder(position, blocks);
        builder.Start();

        yield return new WaitUntil(() => builder.Update());

        mesh = builder.GetMesh(ref mesh);
        ready = true;

        builder = null;
    }

}
