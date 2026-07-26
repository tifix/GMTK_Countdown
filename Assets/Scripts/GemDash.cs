using UnityEngine;

public class GemDash : GemAbstract
{
    public override void OnGemCollected()
    {
        base.OnGemCollected();
        Player.GemCountDash++;

        //handle tutorialisation
        GameController GC = Camera.main.GetComponent<GameController>();
        if (!GC.HasEverCollectedGemB)
        {
            GC.TutorialShowGemB();
        }
    }
}
