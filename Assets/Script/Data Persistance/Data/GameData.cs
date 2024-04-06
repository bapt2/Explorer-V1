using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public string worldName;

    public SerializableDictionary<string, bool> enciclopedie;

    public GameData()
    {
        this.worldName = "New World";
        enciclopedie = new SerializableDictionary<string, bool>();
    }
}
