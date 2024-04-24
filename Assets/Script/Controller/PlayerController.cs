using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public float jumpForce;
    private bool isJumping = false;
    public bool isGrounded = true;

    public static PlayerController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("more than one instance of PlayerController in the scene, newest Destroy");
            Destroy(gameObject);
            return;
        }

        instance = this;

    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);

        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        float horizontaleInput = Input.GetAxis("Horizontal") * speed;
        float depthInput = Input.GetAxis("Vertical") * speed;

        PlayerMovement(horizontaleInput, depthInput);
    }

    void PlayerMovement(float _horizontal, float _depth)
    {

        // camera direction
        Vector3 camForward = CameraController.instance.transform.forward;
        Vector3 camRight = CameraController.instance.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = _depth * camForward;
        Vector3 rightRelative = _horizontal * camRight;

        Vector3 moveDir = forwardRelative + rightRelative;
        
        if (rb.velocity != Vector3.zero)
        {
            transform.forward = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        
        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);

        if (isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = false;
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }


}
