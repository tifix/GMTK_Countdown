using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("balancing parameters")]
    [Range(0,1000),Tooltip("force applied when jumping")]
    public float JumpAcceleration = 20.0f;
    [Range(0, 1000), Tooltip("force applied when jumping")]
    public float MoveAcceleration = 20.0f;
    [Tooltip("Stub. To be used to prevent jump-stacking")]
    public bool IsGrounded = false;

    public InputActionAsset InputActions;
    private InputAction InputActionMove;
    private InputAction InputActionJump;
    [SerializeField, Tooltip("TBA")]
    private Vector2 RawMoveInput;
    public float RawJumpInput;


    [Header("cached components")]
    [SerializeField, Tooltip("Stub. To be used to prevent jump-stacking")]
    private Rigidbody2D Rigidbody;
    [SerializeField, Tooltip("Stub. To be used for collisionChecking")]
    private Collider2D BodyCollider;

    [SerializeField, Tooltip("Stub. To be used for collecting minerals")]
    private ProximitySensor GroundChecker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        //fetch new move sys input actions
        InputActionMove = InputSystem.actions.FindAction("Move");
        InputActionJump = InputSystem.actions.FindAction("Jump");
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    // Update is called once per frame
    void Update()//Fixed
    {
        InputToMovement();
    }


    //from key presses to moving the player character
    void InputToMovement()
    {
        RawMoveInput = InputActionMove.ReadValue<Vector2>();
        RawJumpInput = InputActionJump.ReadValue<float>();

        IsGrounded = GroundChecker.IsDetecting;

        //Jump handling
        if (InputActionJump.WasPressedThisDynamicUpdate() && IsGrounded) //RawJumpInput > 0  InputActionJump.WasPressedThisDynamicUpdate()
        {
            Debug.Log("Jumping now");
            Rigidbody.AddForce(Vector2.up * JumpAcceleration,ForceMode2D.Impulse);
        }

        //sideways move handling
        if (RawMoveInput.x < 0)
        {
            Rigidbody.AddForce(Vector2.left * MoveAcceleration* Time.deltaTime);
        }
        if (RawMoveInput.x > 0)
        {
            Rigidbody.AddForce(Vector2.right * MoveAcceleration * Time.deltaTime);
        }
    }
}
