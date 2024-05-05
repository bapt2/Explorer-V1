using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="SpriteDataBase", menuName ="Item/DataBase", order = 1)]
public class ItemSpriteDataBase : ScriptableObject, ISerializationCallbackReceiver
{
    public Sprite[] sprites;
    public Dictionary<int, Sprite> spriteDictionary = new();

    public Sprite CheckSpriteByID(int id)
    {
        if (spriteDictionary.ContainsKey(id))
            foreach (KeyValuePair<int, Sprite> kvp in spriteDictionary)
                if (kvp.Key == id)
                    return kvp.Value;

        Debug.LogError($"sprite database don't contain key: {id}");
        return null;

    }

    public void OnAfterDeserialize()
    {
        spriteDictionary = new Dictionary<int, Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteDictionary.Add(i, sprites[i]);
        }
    }

    public void OnBeforeSerialize()
    {
    }
}
