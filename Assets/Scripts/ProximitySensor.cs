using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProximitySensor : MonoBehaviour
{
    public enum TriggerType { UpdateConstantly, UpdateOnce};

    public TriggerType triggerType = TriggerType.UpdateConstantly;
    public bool IsDetecting = false;
    public LayerMask ColisionMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((ColisionMask.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            IsDetecting = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(triggerType == TriggerType.UpdateConstantly) 
        {
            if ((ColisionMask.value & (1 << collision.transform.gameObject.layer)) > 0)
            {
                IsDetecting = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((ColisionMask.value & (1 << collision.transform.gameObject.layer)) > 0)
        {
            IsDetecting = false;
        }
    }

}
