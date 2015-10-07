using UnityEngine;
using System.Collections;

/// <summary>
/// Used for manually constructing maps.
/// Inherit this class and set an instance of it to the MapCrafter field in the Map class.
/// </summary>
public abstract class MapCrafter : MonoBehaviour
{
    public abstract int[,] GetMapData(Map map);
    public abstract int[,] GetPropData(Map map);
}
