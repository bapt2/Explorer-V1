using System.Collections;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public bool isUnderWater = false;

    public static WaterScript instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of water script in the scene, destroy the newest one");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if (isUnderWater && !PlayerStatsManager.instance.die)
        {
            PlayerStatsManager.instance.breathBar.gameObject.SetActive(true);
            WaterBreathingDecrease();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            isUnderWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            isUnderWater = false;
        }
    }
    public void WaterBreathingDecrease()
    {
        if (PlayerStatsManager.instance.currentBreath <= 0)
        {
            StartCoroutine(TakeWaterBreathingDamageCoroutine());
        }

        if (isUnderWater)
        {
            if (PlayerStatsManager.instance.currentBreath == 0 && isUnderWater)
                return;
            StartCoroutine(WaterBreathingDecreaseCoroutine());
        }
    }

    public IEnumerator WaterBreathingDecreaseCoroutine()
    {
        if (PlayerStatsManager.instance.currentBreath > 0)
            PlayerStatsManager.instance.currentBreath -= 0.05f;

        else if (PlayerStatsManager.instance.currentBreath < 0)
            PlayerStatsManager.instance.currentBreath = 0;

        PlayerStatsManager.instance.breathBar.SetValue(PlayerStatsManager.instance.currentBreath);
        yield return new WaitForSeconds(2);

    }

    public IEnumerator TakeWaterBreathingDamageCoroutine()
    {
        if (PlayerStatsManager.instance.currentHealth > 0)
            PlayerStatsManager.instance.currentHealth -= 0.5f;

        else if (PlayerStatsManager.instance.currentHealth < 0)
            PlayerStatsManager.instance.currentHealth = 0;

        PlayerStatsManager.instance.healthBar.SetValue(PlayerStatsManager.instance.currentHealth);
        yield return new WaitForSeconds(2);
    }


}
