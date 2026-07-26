using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class GemAbstract : MonoBehaviour
{
    [Header("Generic properties")]
    public bool isDragged = false;
    public InputAction InputActionClick;
    public InputAction InputActionMousePosition;
    public Vector2 mousePos = Vector2.zero;

    public Collider2D Collider;
    public Rigidbody2D Rigidbody;
    public float MouseFollowForce = 10;
    public float FollowStrengthMultiplier;
    public AnimationCurve MouseFollowFalloff;
    public CinemachineImpulseSource CamShake;

    public ParticleSystem ParticlesCollision;

    public PlayerController Player;

    public virtual void Awake()
    {
        InputActionClick.Enable();
        InputActionMousePosition.Enable();
        CamShake = GetComponent<CinemachineImpulseSource>();

        Player = FindFirstObjectByType<PlayerController>(); //not really performant but we don't have 1000+ objects to iterate over so should be chill
    }
    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player")) 
        {
            Debug.Log("hit player. Collecting");
            CamShake.StopAllCoroutines();
            OnGemCollected();
            Destroy(gameObject);
            return;
        }

        ParticlesCollision.Play();
    }
    //stub to be overriden by subclasses
    public virtual void OnGemCollected() 
    {
    }


    public void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(InputActionMousePosition.ReadValue<Vector2>());

        if (InputActionClick.WasPressedThisFrame() )
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.down);
            if(Collider.bounds.Contains(hit.point))
            {
                isDragged = true;
            }
            return;
        }
        if (InputActionClick.WasCompletedThisFrame() && isDragged) 
        {
            isDragged = false;
        }
        if (isDragged) 
        {
            FollowMouseThroughForce();
        }
    }

    public void FollowMouseThroughForce() 
    {
        //get the relative vector from where we currently are to where the gem should be. 
        Vector2 relVector = mousePos - (Vector2)transform.position;
        FollowStrengthMultiplier = MouseFollowFalloff.Evaluate(relVector.magnitude);
        Rigidbody.AddForce(relVector.normalized * MouseFollowForce * FollowStrengthMultiplier * Time.deltaTime);    // * Time.deltaTime


        //Apply relative force and counter-gravity
        Rigidbody.AddForce(Vector2.up * Rigidbody.gravityScale);
    }
}
