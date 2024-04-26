using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour, IDataPersistence
{
    public ValueBar healthBar;
    public ValueBar staminaBar;
    public ValueBar breathBar;

    public float maxHealth = 100;
    public float currentHealth;

    public float maxBreath = 100;
    public float currentBreath;

    public float maxStamina = 150;
    public float currentStamina;

    public static PlayerStatsManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of player stats manager in the scene, destroying the newest one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        healthBar.SetValue(currentHealth);
        staminaBar.SetValue(currentStamina);
        breathBar.SetValue(currentBreath);
    }

    public void LoadData(GameData data)
    {
        maxHealth = data.maxHealth;
        currentHealth = data.currentHealth;
        
        maxBreath = data.maxBreath;
        currentBreath = data.currentBreath;

        maxStamina = data.maxStamina;
        currentStamina = data.currentStamina;
    }
    
    public void SaveData(GameData data)
    {
        data.maxHealth = maxHealth;
        data.currentHealth = currentHealth;
        
        data.maxBreath = maxBreath;
        data.currentBreath = currentBreath;

        data.maxStamina = maxStamina;
        data.currentStamina = currentStamina;
    }
}
