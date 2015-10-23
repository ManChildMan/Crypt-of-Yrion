using UnityEngine;
using System.Collections;

public class Level3Map : MapCrafter {

    public override int[,] GetMapData(Map map)
    {
        Texture2D texture = (Texture2D)Resources.Load("Level3");
        map.Width = texture.width;
        map.Depth = texture.height;
        int[,] mapData = new int[texture.width, texture.height];
        Color32[] pixelData = texture.GetPixels32();
        //HashSet<string> colors = new HashSet<string>();
        for (int x = 0; x < texture.width; x++)
        {
            for (int z = 0; z < texture.height; z++)
            {
                Color32 pixelColor = pixelData[z * texture.width + x];
                if (pixelColor.Equals(new Color32(255, 255, 255, 255)))
                {
                    mapData[x, z] = TerrainType.Floor_01;
                }
                else if (pixelColor.Equals(new Color32(128, 128, 128, 255)))
                {
                    mapData[x, z] = TerrainType.Wall_Stone_01;
                }
                else if (pixelColor.Equals(new Color32(0, 0, 0, 255)))
                {
                    mapData[x, z] = TerrainType.Impassable_Rubble_01;
                }
                else if (pixelColor.Equals(new Color32(0, 0, 255, 255)))
                {
                    mapData[x, z] = TerrainType.Water_Shallow;
                }
                else if (pixelColor.Equals(new Color32(175, 79, 0, 255)))
                {
                    mapData[x, z] = TerrainType.Pillar_03;
                }
                else if (pixelColor.Equals(new Color32(137, 61, 0, 255)))
                {
                    mapData[x, z] = TerrainType.Pillar_01;
                }
                else if (pixelColor.Equals(new Color32(216, 97, 0, 255)))
                {
                    mapData[x, z] = TerrainType.Pillar_02;
                }
                // Used for retrieving correct encoding values when creating a map.
                //string name = pixelColor.r + " " + pixelColor.g + " " + pixelColor.b + " " + pixelColor.a;
                //if (!colors.Contains(name))
                //{
                //    colors.Add(name);
                //    Debug.Log(name);
                //}
            }
        }
        return mapData;
    }

    public override int[,] GetPropData(Map map)
    {
        int[,] propData = new int[map.Width, map.Depth];
        propData[17, 4] = TerrainType.Torch_WallMounted_East;
        propData[17, 8] = TerrainType.Torch_WallMounted_East;
        propData[17, 12] = TerrainType.Torch_WallMounted_East;
        propData[30, 4] = TerrainType.Torch_WallMounted_West;
        propData[30, 8] = TerrainType.Torch_WallMounted_West;
        propData[30, 12] = TerrainType.Torch_WallMounted_West;
        propData[14, 16] = TerrainType.Torch_WallMounted_East;
        propData[14, 19] = TerrainType.Torch_WallMounted_East;
        propData[33, 16] = TerrainType.Torch_WallMounted_West;
        propData[33, 19] = TerrainType.Torch_WallMounted_West;
        propData[14, 31] = TerrainType.Torch_WallMounted_South;
        propData[17, 31] = TerrainType.Torch_WallMounted_South;
        propData[33, 31] = TerrainType.Torch_WallMounted_South;
        propData[30, 31] = TerrainType.Torch_WallMounted_South;
        propData[20, 34] = TerrainType.Torch_WallMounted_East;
        propData[20, 36] = TerrainType.Torch_WallMounted_East;
        propData[20, 38] = TerrainType.Torch_WallMounted_East;
        propData[20, 40] = TerrainType.Torch_WallMounted_East;
        propData[27, 34] = TerrainType.Torch_WallMounted_West;
        propData[27, 36] = TerrainType.Torch_WallMounted_West;
        propData[27, 38] = TerrainType.Torch_WallMounted_West;
        propData[27, 40] = TerrainType.Torch_WallMounted_West;
        propData[11, 67] = TerrainType.Torch_WallMounted_East;
        propData[11, 73] = TerrainType.Torch_WallMounted_East;
        propData[11, 79] = TerrainType.Torch_WallMounted_East;
        propData[36, 67] = TerrainType.Torch_WallMounted_West;
        propData[36, 73] = TerrainType.Torch_WallMounted_West;
        propData[36, 79] = TerrainType.Torch_WallMounted_West;
        propData[16, 103] = TerrainType.Torch_WallMounted_South;
        propData[18, 103] = TerrainType.Torch_WallMounted_South;
        propData[20, 103] = TerrainType.Torch_WallMounted_South;
        propData[31, 103] = TerrainType.Torch_WallMounted_South;
        propData[29, 103] = TerrainType.Torch_WallMounted_South;
        propData[27, 103] = TerrainType.Torch_WallMounted_South;
        return propData;
    }
}
