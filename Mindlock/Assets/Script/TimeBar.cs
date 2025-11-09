using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TimeBar : MonoBehaviour
{
    [Header("Objects")]
    public FadeInStart fadeInStart;
    public CanvasGroup TimeBarCanvasGorup;
    public Image bar;

    [Header("Settings")]
    public float maximumTime = 100;
    public float currentTime;
    public float delay;
    public float barFadeDuration = 1f;

    [Header("Helping bools")]
    public bool canTimerStart = false;
    public bool isInfinite = false;

    private bool stopTimer = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canTimerStart && !stopTimer)
        {
            GetCurrentFill();
            if (!isInfinite)
                currentTime -= Time.deltaTime;
        }

        if (currentTime <= 0)
        {
            currentTime = 0;
            canTimerStart = false;
            Debug.Log("Time's up!");
            StartCoroutine(fadeInStart.FadeToBlack(delay));
        }
    }

    void GetCurrentFill()
    {
        float fillAmount = (float)currentTime / (float)maximumTime;
        bar.fillAmount = fillAmount;
    }

    public IEnumerator HideBar()
    {
        stopTimer = true;

        float time = 0;

        while (time < barFadeDuration)
        {
            float t = time / barFadeDuration;

            TimeBarCanvasGorup.alpha = Mathf.Lerp(1, 0, t);


            time += Time.deltaTime;
            yield return null;
        }
    }
}
