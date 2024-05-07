using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadInput : MonoBehaviour, IDataPersistence
{
    string input;
    
    public void ReadStringInput(string s)
    {
        input = s;
    }

    public void LoadData(GameData data)
    {
        input = data.worldName;
    }

    public void SaveData(GameData data)
    {
        if (input != "")
            data.worldName = input;
    }

}
