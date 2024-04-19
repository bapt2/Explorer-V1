using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadInput : MonoBehaviour, IDataPersistence
{
    string input;
    

    public void LoadData(GameData data)
    {
        input = data.worldName;
    }

    public void SaveData(GameData data)
    {
        data.worldName = input;
    }

    public void ReadStringInput(string s)
    {
        input = s;
        Debug.Log(input);
    }
}
