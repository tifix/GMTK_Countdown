using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GemExplosive : GemAbstract
{
    [Header("Explosion properties")]
    public float explosionForce = 500;
    public float explosionRadius = 50;
    [Range(1,100),Tooltip("to explode, the gem needs to hit SOMETHING with this much force")]
    public float explosionThreshhold = 10;

    public bool isExploding = false;
    public float delayBeforeDespawn = 2f;
    public ParticleSystem ParticlesCollision;
    public CinemachineImpulseSource CamShake;

    public override void Awake()
    {
        base.Awake();
        ParticlesCollision = GetComponentInChildren<ParticleSystem>();
        CamShake = GetComponent<CinemachineImpulseSource>();
        CamShake.enabled = false;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        ParticlesCollision.Play();

        //if hitting with enough energy to explode - explode
        float ImpactVelocity = (collision.relativeVelocity).magnitude;
        Debug.Log(ImpactVelocity);

        if (ImpactVelocity > explosionThreshhold)
        {
            TriggerExplosion();
        }
    }

    public void TriggerExplosion() 
    {
        StartCoroutine(ExplosionCoroutine());
    }

    public IEnumerator ExplosionCoroutine() 
    {
        isExploding = true;
        CamShake.enabled = true;

        GetComponent<SpriteRenderer>().color = Color.white;

        Collider2D[] HitsInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D ObjectHit in HitsInRadius)
        {
            //Break blocks
            //TODO

            //trigger other gems
            GemExplosive OutOtherGem;
            ObjectHit.TryGetComponent<GemExplosive>(out OutOtherGem);
            if(OutOtherGem != null && !OutOtherGem.isExploding)
            {
                OutOtherGem.TriggerExplosion();
            }

            //Apply explosion force to entities -  TODO doing this via tags is error prone, revise to class inheritance
            if (ObjectHit.CompareTag("Player") || ObjectHit.CompareTag("Enemy")) 
            {
                //get relative location and check distance
                Vector2 explosionDirection = ObjectHit.transform.position - transform.position;
                
                //reduce force applied LINEARLY based on distance from explosion location
                float forceAfterFalloff = Mathf.Lerp(0, explosionForce, (explosionRadius - explosionDirection.magnitude));
                
                //apply the force
                ObjectHit.attachedRigidbody.AddForce(explosionDirection * forceAfterFalloff);
                Debug.Log("pushing " + ObjectHit.name + " with a force of " + forceAfterFalloff);
                CamShake.GenerateImpulse(0.5f);
            }
            yield return null;
        }
        //Show explosion VFX
        yield return new WaitForSeconds(delayBeforeDespawn);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
