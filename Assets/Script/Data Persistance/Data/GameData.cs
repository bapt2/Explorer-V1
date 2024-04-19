using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public string worldName;

    public SerializableDictionary<string, bool> enciclopedie;

    public int maxHealth;
    public int currentHealth;

    public int maxStamina;
    public int currentStamina;

    public int itemPlace;
    public int insectPlace;
    public int fishPlace;
    public int plantPlace;

    public List<BaseItem> itemList;
    public List<InsectItem> insectItemList;
    public List<FishItem> fishItemList;
    public List<PlantItem> plantItemList;

    public GameData()
    {
        this.worldName = "New World";
        enciclopedie = new SerializableDictionary<string, bool>();

        maxHealth = 100;
        currentHealth = maxHealth;

        maxStamina = 150;
        currentStamina = maxStamina;

        itemPlace = 20;
        insectPlace = 10;
        fishPlace = 10;
        plantPlace = 20;

        itemList = new();
        insectItemList = new();
        fishItemList = new();
        plantItemList = new();
    }
}
