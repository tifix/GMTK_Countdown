using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Current status")]
    [SerializeField, Tooltip("Uused to prevent jump-stacking")]
    private bool IsGrounded = false;

    [Header("Balancing parameters")]
    [Range(0,100),Tooltip("force applied when jumping")]
    public float JumpAcceleration = 20.0f;
    [Range(500, 2000), Tooltip("force applied when jumping")]
    public float MoveAcceleration = 20.0f;


    //Input handling//////////////////////////
    public InputActionAsset InputActions;
    private InputAction InputActionMove;
    private InputAction InputActionJump;
    private InputAction InputActionSelectGemExplosive;  //1st
    private InputAction InputActionSelectGemDash;       //2nd
    private InputAction InputActionSelectGemReveal;     //3rd
    private InputAction InputActionSelectGemUpload;     //4th 
    [SerializeField, Tooltip("TBA")]
    private Vector2 RawMoveInput;
    public float RawJumpInput;


    [Header("Cached components")]
    [SerializeField, Tooltip("Uused to prevent jump-stacking")]
    private Rigidbody2D Rigidbody;
    [SerializeField, Tooltip("Used for ensuring player doesn't jump mid-air")]
    private ProximitySensor GroundChecker;
    [SerializeField, Tooltip("Used for animating player sprite")]
    private Animator Anim;
    public GameController Controller;

    public int GemCountExplosive = 0;
    public int GemCountDash = 0;
    public int GemCountReveal = 0;
    public int GemCountUpload = 0;

    [Header("Prefabs")]
    [SerializeField, Tooltip("used for summoning a gem if present in inventory")]
    private GameObject PrefabGemExplosive;
    [SerializeField, Tooltip("used for summoning a gem if present in inventory")]
    private GameObject PrefabGemDash;
    [SerializeField, Tooltip("used for summoning a gem if present in inventory")]
    private GameObject PrefabGemReveal;
    [SerializeField, Tooltip("used for summoning a gem if present in inventory")]
    private GameObject PrefabGemUpload;
    [SerializeField, Tooltip("DEBUG; used for spawning for testing")]
    private Transform GemSpawnLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        if(Controller  == null) 
        {
            Controller = Camera.main.GetComponent<GameController>(); 
        }

        //fetch new move sys input actions
        InputActionMove = InputSystem.actions.FindAction("Move");
        InputActionJump = InputSystem.actions.FindAction("Jump");
        InputActionSelectGemExplosive = InputSystem.actions.FindAction("SelectGem1");
        InputActionSelectGemDash = InputSystem.actions.FindAction("SelectGem2");
        InputActionSelectGemReveal = InputSystem.actions.FindAction("SelectGem3");
        InputActionSelectGemUpload = InputSystem.actions.FindAction("SelectGem4");
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
        InputToGemSelect();
    }


    //from key presses to moving the player character
    void InputToMovement()
    {
        RawMoveInput = InputActionMove.ReadValue<Vector2>();
        RawJumpInput = InputActionJump.ReadValue<float>();

        IsGrounded = GroundChecker.IsDetecting;

        //Jump handling
        if (InputActionJump.WasPressedThisDynamicUpdate() && IsGrounded)
        {
            Debug.Log("Jumping now");
            Rigidbody.AddForce(Vector2.up * JumpAcceleration,ForceMode2D.Impulse);

            //handle tutorialisation
            if (!Controller.HasEverUsedJump) 
            {
                Controller.HasEverUsedJump = true;
            }
        }

        //sideways move handling
        if (RawMoveInput.x < 0)
        {
            Rigidbody.AddForce(Vector2.left * MoveAcceleration* Time.deltaTime);
            Anim.SetFloat("Movement", RawMoveInput.x);
            Anim.SetFloat("MovementLast", RawMoveInput.x);

            //handle tutorialisation
            if (!Controller.HasEverUsedMove)
            {
                Controller.HasEverUsedMove = true;
            }
            return;
        }
        if (RawMoveInput.x > 0)
        {
            Rigidbody.AddForce(Vector2.right * MoveAcceleration * Time.deltaTime);
            Anim.SetFloat("Movement", RawMoveInput.x);
            Anim.SetFloat("MovementLast", RawMoveInput.x);

            //handle tutorialisation
            if (!Controller.HasEverUsedMove)
            {
                Controller.HasEverUsedMove = true;
            }
            return;
        }
        Anim.SetFloat("Movement", 0);

    }

    public void Debug_GrantGems() 
    {
        GemCountExplosive += 3;
        GemCountDash += 3;
        GemCountReveal+= 3;
        GemCountUpload += 3;
    }
    void InputToGemSelect() 
    {
        if (InputActionSelectGemExplosive.WasPressedThisDynamicUpdate() && GemCountExplosive > 0) 
        {
            Instantiate(PrefabGemExplosive, GemSpawnLocation.position, Quaternion.identity);
            GemCountExplosive--;
            return;
        }

        if (InputActionSelectGemDash.WasPressedThisDynamicUpdate() && GemCountDash > 0)
        {
            Instantiate(PrefabGemDash, GemSpawnLocation.position, Quaternion.identity);
            GemCountDash--;
            return;
        }

        if (InputActionSelectGemReveal.WasPressedThisDynamicUpdate() && GemCountReveal > 0)
        {
            Instantiate(PrefabGemReveal, GemSpawnLocation.position, Quaternion.identity);
            GemCountReveal--;
            return;
        }

        if (InputActionSelectGemUpload.WasPressedThisDynamicUpdate() && GemCountUpload > 0)
        {
            Instantiate(PrefabGemUpload, GemSpawnLocation.position, Quaternion.identity);
            GemCountUpload--;
            return;
        }

    }
}
