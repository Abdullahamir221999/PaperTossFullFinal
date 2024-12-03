
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpController : MonoBehaviour
{
    [Header("Power-Up Buttons")]
    public Button windBlockerButton;
    public Button paperGliderButton;
    public Button basketMagnetButton;
    public Button scoreMultiplierButton;
    public Button extraLifeButton;
    public Button freezeTimeButton;

    [Header("Power-Up Durations")]
    public float windBlockerDuration = 5f;
    public float paperGliderDuration = 5f;
    public float basketMagnetDuration = 5f;
    public float scoreMultiplierDuration = 5f;
    public float freezeTimeDuration = 5f;

    private bool windBlocked = false;
    private bool paperGliderActive = false;
    private bool basketMagnetActive = false;
    public bool scoreMultiplierActive = false; // Made public for GameManager access

    private int additionalLives = 0;

    public PaperThrow3D paperThrowScript;

    void Start()
    {
        windBlockerButton.onClick.AddListener(() => StartCoroutine(WindBlocker()));
        paperGliderButton.onClick.AddListener(() => StartCoroutine(PaperGlider()));
        basketMagnetButton.onClick.AddListener(() => StartCoroutine(BasketMagnet()));
        scoreMultiplierButton.onClick.AddListener(() => StartCoroutine(ScoreMultiplier()));
        extraLifeButton.onClick.AddListener(ExtraLife);
        freezeTimeButton.onClick.AddListener(() => StartCoroutine(FreezeTime()));
    }

    IEnumerator WindBlocker()
    {
        Debug.Log("WindBlocker Activated");
        FindObjectOfType<FanZone>().windBlocked = true;
        yield return new WaitForSeconds(windBlockerDuration);
        Debug.Log("WindBlocker Deactivated");
        FindObjectOfType<FanZone>().windBlocked = false;
    }

    IEnumerator PaperGlider()
    {
        paperGliderButton.interactable = false;
        paperGliderActive = true;
        if (paperThrowScript != null)
        {
            paperThrowScript.ActivatePaperGlider(paperGliderDuration);
            Debug.Log("Paper Glider activated for " + paperGliderDuration + " seconds.");
        }

        yield return new WaitForSeconds(paperGliderDuration);

        paperGliderActive = false;
        paperGliderButton.interactable = true;
        Debug.Log("Paper Glider Deactivated");
    }

    IEnumerator BasketMagnet()
    {
        basketMagnetButton.interactable = false;
        paperThrowScript.basketMagnetActive = true;
        Debug.Log("Basket Magnet Activated");

        yield return new WaitForSeconds(basketMagnetDuration);

        paperThrowScript.basketMagnetActive = false;
        basketMagnetButton.interactable = true;
        Debug.Log("Basket Magnet Deactivated");
    }

    IEnumerator ScoreMultiplier()
    {
        scoreMultiplierActive = true;
        Debug.Log("Score Multiplier Activated");

        yield return new WaitForSeconds(scoreMultiplierDuration);

        scoreMultiplierActive = false;
        Debug.Log("Score Multiplier Deactivated");
    }

    void ExtraLife()
    {
        GameManager.Instance.GrantExtraLife();  // Increase maxAttempts when extra life is activated
        Debug.Log("Extra Life activated, increasing max attempts.");
    }


    IEnumerator FreezeTime()
    {
        Time.timeScale = 0.5f;
        Debug.Log("Time Freeze Activated");

        yield return new WaitForSecondsRealtime(freezeTimeDuration);

        Time.timeScale = 1;
        Debug.Log("Time Freeze Deactivated");
    }
}
