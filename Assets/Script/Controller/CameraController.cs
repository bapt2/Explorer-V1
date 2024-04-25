using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity;
    public float orbitDamping;
    Vector3 localRot;

    public static CameraController instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of Camera controller in the scene, destroy the newest one");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        //if (DataPersistanceManager.instance.currentScene != "Main Menu")
        //{
            transform.position = PlayerController.instance.transform.position;
            localRot.x += Input.GetAxis("Mouse X") * sensitivity;

            // inverse this if you want to inverse the camera control
            localRot.y -= Input.GetAxis("Mouse Y") * sensitivity;

            localRot.y = Mathf.Clamp(localRot.y, -30f, 80f);

            Quaternion qt = Quaternion.Euler(localRot.y, localRot.x, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.deltaTime * orbitDamping);
        //}
    }
}
