using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DataPersistanceManager : MonoBehaviour
{
    [Header("Debugging")]
    public bool disableDataPersistence = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public string currentScene;
    public bool isProcGenScene = false;
    public bool dataExist = false;

    GameData gameData;

    List<IDataPersistence> dataPersistencesObject;

    FileDataHandler dataHandler;

    string selectedProfileId = "test2";

    public static DataPersistanceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Data Persistance Manager in the scene. Destroying the newest one");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disable");
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        if (dataHandler.Load(selectedProfileId) != null)
            dataExist = true;
        else
            dataExist = false;
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        if (scene.name != "Main Menu")
        {
            PlayerController.instance.gameObject.SetActive(true);
            SaveGame();
        }
        else if (scene.name == "Main Menu" && PlayerController.instance != null)
            PlayerController.instance.gameObject.SetActive(false);

        if (scene.name == ("Procedural Generation_Forest") || scene.name == ("Procedural Generation_Desert") || scene.name == ("Procedural Generation_Island"))
            isProcGenScene = true;
        else if (scene.name != ("Procedural Generation_Forest") && scene.name != ("Procedural Generation_Desert") && scene.name != ("Procedural Generation_Island"))
            isProcGenScene = false;

        dataPersistencesObject = FindAllDataPersistanteObject();
        LoadGame();
    }

 /*   public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading

        selectedProfileId = newProfileId;
        //load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }*/

    //temporally function
    public void StartGameButton()
    {
        // create a new game which will initialize our data to a clean slate

        SaveGame();

        LoadGame();

        SceneManager.LoadSceneAsync("Hub");
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void DeleteProfileData(string profileId)
    {
        dataHandler.Delete(profileId);
        LoadGame();

    }

    public void LoadGame()
    {
        if (disableDataPersistence)
        {
            return;
        }


        // load any save data from a file using the data handler
        gameData = dataHandler.Load(selectedProfileId);

        if (gameData == null)
        {
            Debug.Log("No data whas found, initialise data to default");
            NewGame();
        }


        // push the loaded data to all other script that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObject)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log(gameData.worldName);
    }

    public void SaveGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObject)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        // save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistanteObject()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
