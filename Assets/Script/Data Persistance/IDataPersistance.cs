using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence 
{
    //read Data
    void LoadData(GameData data);

    //Write Data
    void SaveData(GameData data);
}
