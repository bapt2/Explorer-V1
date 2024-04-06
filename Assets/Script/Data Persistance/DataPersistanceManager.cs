using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
public class DataPersistanceManager : MonoBehaviour
{
    [Header("Debugging")]
    public bool disableDataPersistence = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;

    private List<IDataPersistence> dataPersistencesObject;

    private FileDataHandler dataHandler;

    private string selectedProfileId = "test2";

    public static DataPersistanceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Data Persistance Manager in the scene. Destroying the newest one");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disable");
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneloaded called");
        this.dataPersistencesObject = FindAllDataPersistanteObject();
        LoadGame();
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        
        this.selectedProfileId = newProfileId;
        //load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }

    //temporally function
    public void StartGameButton()
    {
        // create a new game which will initialize our data to a clean slate

        LoadGame();

        SaveGame();

        //load the scene which will in turn save the game because of OnSceneUnloaded() in the DataPersistenceManeger
        SceneManager.LoadSceneAsync("Hub");
    }

    public void NewGame()
    {
        this.gameData = new GameData();
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
        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null)
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
