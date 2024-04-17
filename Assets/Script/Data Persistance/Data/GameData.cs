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

    public GameData()
    {
        this.worldName = "New World";
        enciclopedie = new SerializableDictionary<string, bool>();

        maxHealth = 100;
        currentHealth = maxHealth;

        maxStamina = 150;
        currentStamina = maxStamina;

    }
}
