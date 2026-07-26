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

        if (ImpactVelocity > explosionThreshhold)
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
            //Break blocks - TODO refactor this from gem onto terrain itself
            if(ObjectHit is TilemapCollider2D) 
            {
                Tilemap TileMap = ObjectHit.GetComponent<Tilemap>();
                TilemapGen MapData = ObjectHit.transform.parent.GetComponent<TilemapGen>();
                GridLayout gridLayout = ObjectHit.GetComponentInParent<GridLayout>();

                //create a bounds struct to check within - TODO: update to circle not square checker
                Vector3 BoundsMiddle = transform.position - Vector3Int.one * Mathf.FloorToInt(explosionRadius / 2);
                var cellBounds = new BoundsInt(
                gridLayout.WorldToCell(BoundsMiddle),Vector3Int.one * Mathf.FloorToInt(explosionRadius));

                //check all tiles within the bounds
                foreach (var cell in cellBounds.allPositionsWithin)
                {
                    if (TileMap.HasTile(cell))
                    {
                        //get tile data, check if it's special, if it is, spawn a gem from prefab
                        TileBase tileDestroyed = TileMap.GetTile(cell);
                        Debug.Log("cell of type "+ tileDestroyed.name+ " exploded!");
                        foreach(var TilePrefab in MapData.TileDictionary) 
                        {
                            if(tileDestroyed == TilePrefab.TileData && TilePrefab.SpawnOnDestroyed != null) 
                            {
                                Instantiate(TilePrefab.SpawnOnDestroyed, gridLayout.CellToWorld(cell), Quaternion.identity);
                            }
                        }

                        TileMap.SetTile(cell, null);
                    }
                }                
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

        //handle tutorialisation
        if (!Camera.main.GetComponent<GameController>().HasUsedGemA)
        {
            Camera.main.GetComponent<GameController>().HasUsedGemA = true;
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
