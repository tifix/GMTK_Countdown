using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool isShowingTutorialMovement = true;
    public bool isShowingGameOver = false;

    public int score = 0;
    public int GemScoreExplosive = 2;
    public int GemScoreDash = 7;
    public int GemScoreReveal = 200;
    public int GemScoreUpload = 50;
    public bool isGemScoringNow = false;
    public float TimeUploadHeld = 0;
    public float GemScoreTickInterval = 0.2f;
    public AnimationCurve GemScoreTickIntervalMultiplier;

    public float GemScoreScaleMultiplier = 1.05f;
    public float CounterMaxSize = 2.5f;

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


    public void SkipIntro() 
    {
        animator.Play("TutorialMoveIn", 0);
    }

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
        else if(!isShowingGameOver)
        {
            EndGameOnTimeOut();
        }

        UpdateCounters();
    }


    public void EndGameOnTimeOut() 
    {
        isShowingGameOver = true;
        Debug.Log("GAME OVER");
        animator.SetTrigger("BadEnding");
        Invoke("RestartGame", 30);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void GemsToScore()
    {
        StartCoroutine(ProcessGemScoring());
    }

    public IEnumerator ProcessGemScoring()
    {
        isGemScoringNow = true;
        int cachedNumberA = Player.GemCountExplosive;
        int cachedNumberB = Player.GemCountDash;
        int cachedNumberC = Player.GemCountReveal;
        int cachedNumberD = Player.GemCountUpload-1;
        Vector3 CounterBaseSize = CounterScore.rectTransform.localScale;
        float GemTickCompoundInterval;
        for (int i = 0; i < cachedNumberA; i++)
        {
            Player.GemCountExplosive--;
            score += GemScoreExplosive;
            CounterScore.text = score.ToString();
            CounterScore.rectTransform.localScale += Vector3.one * GemScoreScaleMultiplier;
            CounterScore.rectTransform.localScale = Vector3.ClampMagnitude(CounterScore.rectTransform.localScale, CounterMaxSize);
            GemTickCompoundInterval = GemScoreTickInterval * GemScoreTickIntervalMultiplier.Evaluate(TimeUploadHeld);
            yield return new WaitForSeconds(GemTickCompoundInterval);
            TimeUploadHeld += GemTickCompoundInterval;
            if (!isGemScoringNow)
            {
                break;
            }
        }
        for (int i = 0; i < cachedNumberB; i++)
        {
            Player.GemCountDash--;
            score += GemScoreDash;
            CounterScore.text = score.ToString();
            CounterScore.rectTransform.localScale += Vector3.one * GemScoreScaleMultiplier;
            CounterScore.rectTransform.localScale = Vector3.ClampMagnitude(CounterScore.rectTransform.localScale, CounterMaxSize);
            GemTickCompoundInterval = GemScoreTickInterval * GemScoreTickIntervalMultiplier.Evaluate(TimeUploadHeld);
            yield return new WaitForSeconds(GemTickCompoundInterval);
            TimeUploadHeld += GemTickCompoundInterval;
            if (!isGemScoringNow)
            {
                break;
            }
        }
        for (int i = 0; i < cachedNumberC; i++)
        {
            Player.GemCountReveal--;
            score += GemScoreReveal;
            CounterScore.text = score.ToString();
            CounterScore.rectTransform.localScale += Vector3.one * GemScoreScaleMultiplier;
            CounterScore.rectTransform.localScale = Vector3.ClampMagnitude(CounterScore.rectTransform.localScale, CounterMaxSize);
            GemTickCompoundInterval = GemScoreTickInterval * GemScoreTickIntervalMultiplier.Evaluate(TimeUploadHeld);
            yield return new WaitForSeconds(GemTickCompoundInterval);
            TimeUploadHeld += GemTickCompoundInterval;
            if (!isGemScoringNow)
            {
                break;
            }
        }
        for (int i = 0; i < cachedNumberD; i++)
        {
            Player.GemCountUpload--;
            score += GemScoreUpload;
            CounterScore.text = score.ToString();
            CounterScore.rectTransform.localScale += Vector3.one * GemScoreScaleMultiplier;
            CounterScore.rectTransform.localScale = Vector3.ClampMagnitude(CounterScore.rectTransform.localScale, CounterMaxSize);
            GemTickCompoundInterval = GemScoreTickInterval * GemScoreTickIntervalMultiplier.Evaluate(TimeUploadHeld);
            yield return new WaitForSeconds(GemTickCompoundInterval);
            TimeUploadHeld += GemTickCompoundInterval;
            if (!isGemScoringNow) 
            {
                break;
            }
        }
        isGemScoringNow = false;
        CounterScore.rectTransform.localScale = CounterBaseSize;
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
