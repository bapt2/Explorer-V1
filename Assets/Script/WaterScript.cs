using System.Collections;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public bool isUnderWater = false;

   /* private void Update()
    {
        if (isUnderWater)
        {
            PlayerStatsManager.instance.breathBar.gameObject.SetActive(true);
            WaterBreathingDecrease();
        }
        else if (!isUnderWater && PlayerStatsManager.instance.currentBreath < PlayerStatsManager.instance.maxBreath)
            WaterBreathingRegen();

        else if (PlayerStatsManager.instance.currentBreath == PlayerStatsManager.instance.maxBreath && !isUnderWater)
            PlayerStatsManager.instance.breathBar.gameObject.SetActive(false);
    }*/

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
            PlayerStatsManager.instance.currentBreath -= 0.1f;

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

    public void WaterBreathingRegen()
    {
        if (PlayerStatsManager.instance.currentBreath >= 0 && PlayerStatsManager.instance.currentBreath < PlayerStatsManager.instance.maxBreath && !isUnderWater)
        {
            StartCoroutine(WaterBreathingRegenCoroutine());
        }
    }

    public IEnumerator WaterBreathingRegenCoroutine()
    {
        yield return new WaitForSeconds(2);
        if (PlayerStatsManager.instance.currentBreath < PlayerStatsManager.instance.maxBreath)
            PlayerStatsManager.instance.currentBreath += 1;
        else
            PlayerStatsManager.instance.currentBreath = PlayerStatsManager.instance.maxBreath;
        PlayerStatsManager.instance.breathBar.SetValue(PlayerStatsManager.instance.currentBreath);
    }
}
