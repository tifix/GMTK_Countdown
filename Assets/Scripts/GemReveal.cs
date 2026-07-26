using System.Collections;
using UnityEngine;

public class GemReveal : GemAbstract
{

    [Header("Reveal properties")]
    public float Radius = 100;
    public float Duration = 3.0f;

    public bool EffectActive = false;
/*    public void StartReveal() 
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
    }*/

    public override void OnGemCollected()
    {
        base.OnGemCollected();
        Player.GemCountReveal++;
        Player.ShowRevealTutorial();
    }


}
