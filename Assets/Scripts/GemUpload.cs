using UnityEngine;

public class GemUpload : GemAbstract
{
    public override void OnGemCollected()
    {
        base.OnGemCollected();
        Player.GemCountUpload++;

        //handle tutorialisation
        GameController GC = Camera.main.GetComponent<GameController>();
        if (!GC.HasEverCollectedGemD)
        {
            GC.TutorialShowGemD();
        }
    }

}
