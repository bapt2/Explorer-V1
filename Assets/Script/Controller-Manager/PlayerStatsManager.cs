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

    public bool die = false;

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
        SetSliderValue();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            IsDie();
        }

        WaterBreathingRegen();
        if (currentBreath == maxBreath)
        {
            breathBar.gameObject.SetActive(false);
            breathBar.SetMaxValue(maxBreath);
        }

    }

    public void ResetStats()
    {
        currentHealth = maxHealth;
        currentBreath = maxBreath;
        currentStamina = maxStamina;
    }

    public void IsDie()
    {
        die = true;

        if (InventoryManager.instance.inventoryIsOpen)
        {
            InventoryManager.instance.inventoryPanel.SetActive(false);
            InventoryManager.instance.inventoryIsOpen = false;
            InventoryManager.instance.enabled = false;
        }
        else if (!InventoryManager.instance.inventoryIsOpen)
        {
            PlayerController.instance.enabled = false;
            CameraController.instance.enabled = false;
        }
        GameOverManager.instance.gameOverMenu.SetActive(true);
    }

    public void IsAlive()
    {
        GameOverManager.instance.gameOverMenu.SetActive(false);

        PlayerController.instance.enabled = true;
        CameraController.instance.enabled = true;
        InventoryManager.instance.enabled = true;
        ResetStats();
        SetSliderValue();
        die = false;
    }


    public void WaterBreathingRegen()
    {
        if (currentBreath >= 0 && currentBreath < maxBreath && (!WaterScript.instance.isUnderWater || WaterScript.instance == null) && !PlayerStatsManager.instance.die)
            StartCoroutine(WaterBreathingRegenCoroutine());
    }

    public IEnumerator WaterBreathingRegenCoroutine()
    {
        yield return new WaitForSeconds(2);
        if (currentBreath < maxBreath)
            currentBreath += 1;
        else
        {
            currentBreath = maxBreath;
        }
        breathBar.SetValue(currentBreath);
    }

    public void SetSliderValue()
    {
        healthBar.SetMaxValue(maxHealth);
        breathBar.SetMaxValue(maxBreath);
        staminaBar.SetMaxValue(maxStamina);
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
