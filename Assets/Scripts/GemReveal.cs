using System.Collections;
using UnityEngine;

public class GemReveal : GemAbstract
{

    [Header("Reveal properties")]
    public float Radius = 100;
    public float Duration = 60;

    public bool EffectActive = false;
    public void StartReveal() 
    {
        StartCoroutine(ProcessReveal());
    }


    public IEnumerator ProcessReveal() 
    {
        if(EffectActive) 
        {
            yield break;    // quit early if active to prevent effect stacking
        }
        EffectActive = true;
        yield return new WaitForSeconds(Duration);
    }
    public override void OnGemCollected()
    {
        base.OnGemCollected();
        Player.GemCountReveal++;

        //handle tutorialisation
        GameController GC = Camera.main.GetComponent<GameController>();
        if (!GC.HasEverCollectedGemC)
        {
            GC.TutorialShowGemC();
        }
    }
}
