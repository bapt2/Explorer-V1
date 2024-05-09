using UnityEngine;

public class StaminaControler : MonoBehaviour
{

    public float speedAdd;
    public float speedSub;
    public bool reachedZero = false;
    public bool speedAddAplied = false;
    public bool speedSubAplied = false;

    // Update is called once per frame
    void Update()
    {
        if (PlayerStatsManager.instance.currentStamina >= PlayerStatsManager.instance.maxStamina)
        {
            PlayerStatsManager.instance.currentStamina = PlayerStatsManager.instance.maxStamina;
            speedAddAplied = false;
            speedSubAplied = false;
            reachedZero = false;
        }
        if (Input.GetKey(KeyCode.LeftShift) && !reachedZero && PlayerController.instance.IsMoving())
        {
            DecreaseStamina();
        }
        if (!Input.GetKey(KeyCode.LeftShift))
            RegenStamina();

        if (!speedAddAplied && !speedSubAplied)
            PlayerController.instance.workingSpeed = PlayerController.instance.speed;


    }

    public void DecreaseStamina()
    {
        if (PlayerStatsManager.instance.currentStamina > 0)
        {
            if (!speedAddAplied)
            {
                speedAddAplied = true;
                PlayerController.instance.workingSpeed += speedAdd;
            }
            PlayerStatsManager.instance.currentStamina -= 0.5f;

            PlayerStatsManager.instance.staminaBar.SetValue(PlayerStatsManager.instance.currentStamina);
        }

        if (PlayerStatsManager.instance.currentStamina <= 0)
        {
            PlayerStatsManager.instance.currentStamina = 0;
            reachedZero = true;
            RegenStamina();
            return;
        }
    }


public void RegenStamina()
{
    if (reachedZero)
    {
        if (PlayerStatsManager.instance.currentStamina < PlayerStatsManager.instance.maxStamina)
        {
            if (!speedSubAplied && speedAddAplied)
            {
                PlayerController.instance.workingSpeed = PlayerController.instance.speed;
                speedAddAplied = false;
                speedSubAplied = true;
                PlayerController.instance.workingSpeed -= speedSub;
            }
            PlayerStatsManager.instance.currentStamina += 0.1f;
        }

        PlayerStatsManager.instance.staminaBar.SetValue(PlayerStatsManager.instance.currentStamina);
    }
    else
    {
        if (PlayerStatsManager.instance.currentStamina < PlayerStatsManager.instance.maxStamina)
            PlayerStatsManager.instance.currentStamina += 0.5f;

        PlayerStatsManager.instance.staminaBar.SetValue(PlayerStatsManager.instance.currentStamina);
    }
}
}
