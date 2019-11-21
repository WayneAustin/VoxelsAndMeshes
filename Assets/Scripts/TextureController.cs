using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureController
{
    public static Dictionary<string, Vector2[]> textureMap = new Dictionary<string, Vector2[]> ();

    public static void Initialise (string texturePath, Texture texture)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(texturePath);

        foreach (Sprite s in sprites)
        {
            Vector2[] uvs = new Vector2[4];

            uvs[0] = new Vector2(s.rect.xMin / texture.width, s.rect.yMin / texture.height);
            uvs[1] = new Vector2(s.rect.xMax / texture.width, s.rect.yMin / texture.height);
            uvs[2] = new Vector2(s.rect.xMin / texture.width, s.rect.yMax / texture.height);
            uvs[3] = new Vector2(s.rect.xMax / texture.width, s.rect.yMax / texture.height);

            textureMap.Add(s.name, uvs);
        }
    }

    public static bool AddTextures(Block block, Directions direction, int index, Vector2[] uvs)
    {
        string key = FastGetKey(block, direction);

        Vector2[] text;
        if (textureMap.TryGetValue(key, out text))
        {
            uvs[index] = text[0];
            uvs[index + 1] = text[1];
            uvs[index + 2] = text[2];
            uvs[index + 3] = text[3];

            return true;
        }

        text = textureMap["Default"];

        uvs[index] = text[0];
        uvs[index + 1] = text[1];
        uvs[index + 2] = text[2];
        uvs[index + 3] = text[3];

        return false;
    }

    static string FastGetKey(Block block, Directions direction)
    {
        if (block == Block.Stone)
            return "Stone";

        if (block == Block.Dirt)
            return "Dirt";

        if (block == Block.Grass) { 
            if (direction == Directions.Up)
                return "Grass_Up";

            if (direction == Directions.Down)
                return "Dirt";

            return "Grass_Side";
        }

        if (block == Block.WoodPlanks)
            return "WoodenPlanks";

        return "Default";
    }
}
