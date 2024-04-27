using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    public float jumpForce;
    public bool isGrounded = true;

    float horizontaleInput;
    float depthInput;
    public float workingSpeed;
    
    bool isJumping = false;

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
        if (SceneManager.GetActiveScene().name == "Hub")
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);

        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody>();

        workingSpeed = speed;
    }

    private void Update()
    {

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        horizontaleInput = Input.GetAxis("Horizontal") * workingSpeed;
        depthInput = Input.GetAxis("Vertical") * workingSpeed;

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

    public bool IsMoving()
    {
        if (rb.velocity.x != 0 || rb.velocity.z != 0)
            return true;
        return false;
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
