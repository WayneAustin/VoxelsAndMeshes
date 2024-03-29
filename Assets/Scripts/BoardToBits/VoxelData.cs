﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelData
{
    int[,,] data = new int[,,]
    {
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } },
        { { 0,1,1 }, { 1,1,1 }, { 1,1,0 }, { 1,1,2 } }
    };

    public int Width
    {
        get
        {
            return data.GetLength(0);
        }
    }

    public int Depth
    {
        get
        {
            return data.GetLength(2);
        }
    }

    public int Height
    {
        get
        {
            return data.GetLength(1);
        }
    }

    public int GetCell(int x, int y, int z)
    {
        return data[x, y, z];
    }

    public int GetNeighbour(int x, int y, int z, Direction dir)
    {
        DataCoordinate offsetToCheck = offsets[(int)dir];
        DataCoordinate neighbourCoord = new DataCoordinate(x + offsetToCheck.x, y + offsetToCheck.y, z + offsetToCheck.z);

        if (neighbourCoord.x < 0 || neighbourCoord.x >= Width || neighbourCoord.y < 0 || neighbourCoord.y >= Height || neighbourCoord.z < 0 || neighbourCoord.z >= Depth)
        {
            return 0;
        }
        else
        {
            return GetCell(neighbourCoord.x, neighbourCoord.y, neighbourCoord.z);
        }
    }

    struct DataCoordinate
    {
        public int x;
        public int y;
        public int z;

        public DataCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    DataCoordinate[] offsets =
    {
        new DataCoordinate(0, 0, 1),
        new DataCoordinate(1, 0, 0),
        new DataCoordinate(0, 0, -1),
        new DataCoordinate(-1, 0, 0),
        new DataCoordinate(0, 1, 0),
        new DataCoordinate(0, -1, 0)
    };
}
