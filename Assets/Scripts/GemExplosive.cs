using Assets;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GemExplosive : GemAbstract
{
    [Header("Explosion properties")]
    public float explosionForce = 500;
    public float explosionRadius = 50;
    [Range(1,100),Tooltip("to explode, the gem needs to hit SOMETHING with this much force")]
    public float explosionThreshhold = 10;

    public bool isExploding = false;
    public float delayBeforeDespawn = 2f;
    public ParticleSystem ExplosionFX;



    public override void Awake()
    {
        base.Awake();
        if(ParticlesCollision == null)
        {
            ParticlesCollision = GetComponentInChildren<ParticleSystem>();
        }

        CamShake.enabled = false;
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        //if hitting with enough energy to explode - explode
        float ImpactVelocity = (collision.relativeVelocity).magnitude;
        Debug.Log(ImpactVelocity);

        if (ImpactVelocity > explosionThreshhold && !collision.gameObject.CompareTag("Player"))
        {
            TriggerExplosion();
        }
        else
        {
            base.OnCollisionEnter2D(collision);
        }
    }
    public override void OnGemCollected()
    {
        Player.GemCountExplosive++;
        base.OnGemCollected();

        //handle tutorialisation
        if (!Camera.main.GetComponent<GameController>().HasEverCollectedGemA)
        {
            Camera.main.GetComponent<GameController>().TutorialShowGemA();
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
        ExplosionFX.Play();
        GetComponent<SpriteRenderer>().enabled = false;

        Collider2D[] HitsInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D ObjectHit in HitsInRadius)
        {
            //if the object has been destroyed already
            if (!ObjectHit.gameObject) 
            {
                continue;
            }
            if(ObjectHit is TilemapCollider2D)
            {
                TilemapGen MapData = ObjectHit.transform.parent.GetComponent<TilemapGen>();
                MapData.BreakTiles((TilemapCollider2D)ObjectHit,transform.position,explosionRadius);                         
            }

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

        //handle tutorialisation - complete
        GameController GC = Camera.main.GetComponent<GameController>();
        if (!GC.HasUsedGemA && GC.HasEverCollectedGemA)
        {
            GC.HasUsedGemA = true;
        }

        yield return new WaitForSeconds(delayBeforeDespawn);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
