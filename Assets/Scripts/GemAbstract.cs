using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class GemAbstract : MonoBehaviour
{
    [Header("Generic properties")]
    public int ScoreValue = 5;
    public bool isDragged = false;
    public InputAction InputActionClick;
    public InputAction InputActionMousePosition;
    public Vector2 mousePos = Vector2.zero;

    public Collider2D Collider;
    public Rigidbody2D Rigidbody;
    public float MouseFollowForce = 10;
    public float FollowStrengthMultiplier;
    public AnimationCurve MouseFollowFalloff;

    public virtual void Awake()
    {
        InputActionClick.Enable();
        InputActionMousePosition.Enable();
    }


    public void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(InputActionMousePosition.ReadValue<Vector2>());

        if (InputActionClick.WasPressedThisFrame() )
        {
            Debug.Log("click registered");
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.down);
            if(Collider.bounds.Contains(hit.point))
            {
                Debug.Log("click within bounds of "+name);
                isDragged = true;
            }
            return;
        }
        if (InputActionClick.WasCompletedThisFrame() && isDragged) 
        {
            Debug.Log("click ending");
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
