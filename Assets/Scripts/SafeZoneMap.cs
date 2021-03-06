﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Constructs the safe zone map.
/// </summary>
public sealed class SafeZoneMap : MapCrafter {

    public override int[,] GetMapData(Map map)
    {
        Texture2D texture = (Texture2D)Resources.Load("SafeZoneMap");
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
        return propData;
    }
}
