using UnityEngine;

public class GemUpload : GemAbstract
{
    public override void OnGemCollected()
    {
        base.OnGemCollected();
        Player.GemCountUpload++;

        //handle tutorialisation
        if (!Camera.main.GetComponent<GameController>().HasEverCollectedGemD)
        {
            Camera.main.GetComponent<GameController>().TutorialShowGemD();
        }
    }
}
