using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Space]
    public Transform cam;

    [Space]
    public float speed = 5;
    public float jumpHight = 3;
    public float rotationSpeed = 1;
    public float turnSmoothTime = 0.1f;

    private float turnSmoothVelocity;
    private CharacterController controller;
    private Animator animator;
    private Vector3 direction;
    private bool isGrounded;
    private Vector3 velocity;

    public GameObject ship;
    public GameObject shipCam;
    public GameObject playerCam;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ship = GameObject.FindGameObjectWithTag("Ship");
        shipCam = GameObject.FindGameObjectWithTag("ShipCam");
        playerCam = GameObject.FindGameObjectWithTag("PlayerCam");
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();


    }

    // Update is called once per frame
    void Update()
    {

        velocity.y += -9.81f * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        PlayerMovement();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2, LayerMask.GetMask("Ship"));

            if (colliders != null)
            {
                shipCam.SetActive(true);
                ship.GetComponent<HoverPlane>().enabled = true;

                playerCam.SetActive(false);
                this.gameObject.SetActive(false);

            }
        }
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHight * -2 * -9.81f);
        }
    }
}