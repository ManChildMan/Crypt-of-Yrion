/// <summary>
/// 
/// </summary>
public static class TerrainType
{
    public const int Impassable = -100;
    public const int Impassable_Rubble_01 = -101;

    public const int Chasm = 0;

    public const int Floor = 100;
    public const int Floor_01 = 101;

    public const int Wall = 200;
    public const int Wall_Stone_01 = 201;

    public const int Water = 300;
    public const int Water_Shallow = 301;
    public const int Water_Deep = 302;

    public const int Torch = 400;
    public const int Torch_WallMounted_North = 401;
    public const int Torch_WallMounted_South = 402;
    public const int Torch_WallMounted_East = 403;
    public const int Torch_WallMounted_West = 404;

    public const int Item = 500;
    public const int Item_Chest_North = 501;
    public const int Item_Chest_South = 502;
    public const int Item_Chest_East = 503;
    public const int Item_Chest_West = 504;

    public const int Stairs = 600;
    public const int Stairs_UpNorth = 601;
    public const int Stairs_UpSouth = 602;
    public const int Stairs_UpEast = 603;
    public const int Stairs_UpWest = 604;
    public const int Stairs_DownNorth = 601;
    public const int Stairs_DownSouth = 602;
    public const int Stairs_DownEast = 603;
    public const int Stairs_DownWest = 604;

    public const int Generator_Empty = -1;
    public const int Generator_Default = 1;
    public const int Generator_Secondary = 2;
    public const int Generator_Selection = int.MaxValue;
}
