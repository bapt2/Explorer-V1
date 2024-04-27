using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    public bool hasRespawn = false;

    public static GameOverManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of game over manager, destroyin the newest one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }


    public void RetryButton()
    {
        hasRespawn = true;
        SceneManager.LoadScene("Hub");

        PlayerStatsManager.instance.IsAlive();
    }

    public void MainMenuButton()
    {
        PlayerStatsManager.instance.IsAlive();
        PlayerStatsManager.instance.gameObject.SetActive(false);

        SceneManager.LoadScene("Main Menu");
    }

    public void RageQuitButton()
    {

        SceneManager.LoadScene("Hub");
        PlayerStatsManager.instance.IsAlive();

        Application.Quit();
    }
}
