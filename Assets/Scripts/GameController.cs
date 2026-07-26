using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool isShowingTutorialMovement = true;

    public int score = 0;

    [Range(0,1500)]
    public float timeLeft = 900;  //900 => 15mins in seconds
    //Player Data
    public PlayerController Player;


    //Parent to all debug buttons, disabling it will hide them all
    public GameObject DebugWindowParent;

    //for UI effects and animations
    public Animator animator;

    public Text CounterTimer;
    public Text CounterScore;
    public Text CounterGemsA;
    public Text CounterGemsB;
    public Text CounterGemsC;
    public Text CounterGemsD;

    public bool HasEverUsedMove = false;
        public bool HasEverUsedJump = false;

    public bool HasEverCollectedGemA = false;
        public bool HasEverCollectedGemB = false;
            public bool HasEverCollectedGemC = false;
                public bool HasEverCollectedGemD = false;

    public bool HasUsedGemA = false;
        public bool HasUsedGemB = false;
            public bool HasUsedGemC = false;
                public bool HasUsedGemD = false;


    private void Start()
    {
        StartCoroutine(AwaitInputForTutorialMovement());
    }

    void Update()
    {
        if(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timeLeft = Mathf.Clamp(timeLeft, 0, 6039);   //99min:99sec
        }
        else
        {
            EndGameOnTimeOut();
        }

        UpdateCounters();
    }

    public void EndGameOnTimeOut() 
    {
        Debug.Log("GAME OVER");
    }

    public IEnumerator AwaitInputForTutorialMovement() 
    {
        //continue showing until both actions completed
        while (!HasEverUsedMove && !HasEverUsedJump) 
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetTrigger("TutMoveHide");
    }
    public IEnumerator AwaitInputForTutorialGemA()
    {
        HasEverCollectedGemA = true;
        animator.SetTrigger("TutExplosiveShow");

        while (!HasUsedGemA)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetTrigger("TutExplosiveHide");
    }
    public IEnumerator AwaitInputForTutorialGemB()
    {
        HasEverCollectedGemB = true;
        animator.SetTrigger("TutDashShow");

        while (!HasUsedGemB)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetTrigger("TutDashHide");
    }
    public IEnumerator AwaitInputForTutorialGemC()
    {
        HasEverCollectedGemC = true;
        animator.SetTrigger("TutRevealShow");

        while (!HasUsedGemC)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetTrigger("TutRevealHide");
    }
    public IEnumerator AwaitInputForTutorialGemD()
    {
        HasEverCollectedGemD = true;
        animator.SetTrigger("TutRecallShow");

        while (!HasUsedGemD)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetTrigger("TutRecallHide");
    }

    public void UpdateCounters() 
    {
        CounterGemsA.text = Player.GemCountExplosive.ToString();
        CounterGemsB.text = Player.GemCountDash.ToString();
        CounterGemsC.text = Player.GemCountReveal.ToString();
        CounterGemsD.text = Player.GemCountUpload.ToString();
        CounterTimer.text = ParseTimeLeftToString(timeLeft);
    }

    public string ParseTimeLeftToString(float time)
    {
        string timeMinutes = Mathf.FloorToInt(time / 60).ToString();
        int Seconds = Mathf.FloorToInt(time % 60);
        string timeSeconds = Seconds.ToString();
        if(Seconds < 10) 
        {
            timeSeconds = "0" + Seconds.ToString();
        }
        string output = timeMinutes + ":" + timeSeconds;

        return output;
    }


    public void FX_GemCollectedExplosive() => animator.SetTrigger("GemCollectedA");
    public void FX_GemCollectedDash() => animator.SetTrigger("GemCollectedB");
    public void FX_GemCollectedReveal() => animator.SetTrigger("GemCollectedC");
    public void FX_GemCollectedUpload() => animator.SetTrigger("GemCollectedD");

    public void TutorialShowGemA() => StartCoroutine(AwaitInputForTutorialGemA());
    public void TutorialShowGemB() => StartCoroutine(AwaitInputForTutorialGemB());
    public void TutorialShowGemC() => StartCoroutine(AwaitInputForTutorialGemC());
    public void TutorialShowGemD() => StartCoroutine(AwaitInputForTutorialGemD());
}
