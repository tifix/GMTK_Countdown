using UnityEngine;

public class GemDash : GemAbstract
{
    public override void OnGemCollected()
    {
        base.OnGemCollected();
        Player.GemCountDash++;

        //handle tutorialisation
        if (!Camera.main.GetComponent<GameController>().HasEverCollectedGemA)
        {
            Camera.main.GetComponent<GameController>().TutorialShowGemB();
        }
    }
}
