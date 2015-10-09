using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents the textures for all item images (for item slots).
/// </summary>
public class ItemImages : MonoBehaviour
{
    private Dictionary<string, Sprite> sprites;

	void Start ()
    {
        sprites = new Dictionary<string, Sprite>();
        Sprite[] localSprites = Resources.LoadAll<Sprite>("Items");
        for (int i = 0; i < localSprites.Length; i++)
        {
            Sprite localSprite = localSprites[i];
            sprites.Add(localSprite.name, localSprite);
        }
	}

    public Sprite GetItemSprite(string itemName)
    {
        return sprites[itemName];
    }
}
