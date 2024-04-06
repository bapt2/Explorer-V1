using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveManager : MonoBehaviour
{
    [SerializeField] GameObject savePanel;
    //[SerializeField] GameObject savePrefabs;
    [SerializeField] GameObject maxSaveWarning;
    [SerializeField] GameObject newGamePanel;
    public ButtonManager buttonManager;

    public int maxSave = 9;
    public int actualNumberOfSave;

    public List<GameObject> saveList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject save in saveList)
        {
            InstantiateSave(save);
        }
    }

    public void OnNewGameClick()
    {
        DataPersistanceManager.instance.NewGame();
        DataPersistanceManager.instance.SaveGame();

        SceneManager.LoadSceneAsync("Hub");
    }

    public void OnContinueGameClick()
    {
        DataPersistanceManager.instance.SaveGame();
        //load the next scene which will turn load the game because of OnSceneloaded() in DataPersistenceManager
        SceneManager.LoadSceneAsync("Hub");
    }

    // Update is called once per frame
    public void CheckMaxSave()
    {
        if (actualNumberOfSave >= maxSave)
        {
            maxSaveWarning.SetActive(true);
            return;
        }
        else
        {
            buttonManager.WorldCreationButton();
        }
    }

    public void InstantiateSave(GameObject savePrefab)
    {
        saveList.Add(savePrefab);
        actualNumberOfSave += 1;
        buttonManager.BackButton();
        Instantiate(savePrefab, savePanel.transform);

    }

}
