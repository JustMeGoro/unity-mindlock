using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class Room1ObjectiveUI : MonoBehaviour
{
    public static Room1ObjectiveUI Instance;

    [Header("Object and delay selector")]
    // Zvolení objektù
    public TMP_Text objectiveTitle;   // Text pro "OBJECTIVE"
    public TMP_Text objectiveText;    // Text pro samotný úkol
    public CanvasGroup objectiveUnderline; // Podtrhnutí

    // Urèení delayù
    public float letterDelay = 0.1f;
    public float startDelay = 1f; // èekání pøed zaèátkem

    [Header("Text options")]
    // Co se kde napíše
    public string title = "OBJECTIVE"; 
    public string task = " ";

    [Header("Audio options")]
    // Volba audiosource
    public AudioSource audioSource;
    // AudioClips
    public AudioClip typingSound;
    public AudioClip backspaceSound;

    private bool objectiveShown = false;

    void Awake()
    {
        Instance = this;   
    }

    void Start()
    {
        objectiveTitle.text = "";
        objectiveText.text = "";
        objectiveUnderline.alpha = 0;

        if (!FadeInStart.Instance.fadingOut)
        {
            StartCoroutine(ShowObjective());
        }
    }

    private void Update()
    {
        if (!FadeInStart.Instance.fadingOut && !objectiveShown)
        {
            objectiveShown = true;
            StartCoroutine(ShowObjective());
        }
    }

    public IEnumerator ShowNextObjective(string ObjectiveText)
    {
        task = ObjectiveText;
        yield return new WaitForSeconds(0.5f);

        // Spustíme zvuk
        audioSource.loop = true; // aby se opakoval, pokud je krátký
        audioSource.clip = typingSound;
        audioSource.Play();


        ObjectiveNotification.Instance.Show();
        // Efekt psaní pro úkol
        foreach (char c in task)
        {
            objectiveText.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        audioSource.Stop();
        audioSource.loop = false;
    }

    IEnumerator ShowObjective()
    {

        yield return new WaitForSeconds(startDelay);

        // Spustíme zvuk
        audioSource.loop = true; // aby se opakoval, pokud je krátký
        audioSource.clip = typingSound;
        audioSource.Play();

        // Efekt psaní pro title
        foreach (char c in title)
        {
            objectiveTitle.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        audioSource.Stop();
        yield return StartCoroutine(FadeIn(objectiveUnderline, 1f));
        audioSource.Play();

        ObjectiveNotification.Instance.Show();
        // Efekt psaní pro úkol
        foreach (char c in task)
        {
            objectiveText.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        // Po dokonèení zastav zvuk
        audioSource.Stop();
        audioSource.loop = false;
    }


    public void RemoveObjective(float letterDelayOverride = -1f)
    {
        // pokud je -1 použij defaultní delay
        float delay = letterDelayOverride > 0 ? letterDelayOverride : letterDelay;
        StartCoroutine(RemoveTaskCoroutine(delay));
    }

    private IEnumerator RemoveTaskCoroutine(float delay)
    {
        Color flashColor = new Color(1f, 0.9f, 0.1f); // jemnì žlutá (RGB)
        objectiveText.color = flashColor;

        yield return new WaitForSeconds(0.7f);

        // Spustíme zvuk
        audioSource.loop = true; // aby se opakoval, pokud je krátký
        audioSource.clip = backspaceSound;
        audioSource.Play();

        // Odmazávání textu úkolu po písmenech
        while (objectiveText.text.Length > 0)
        {
            objectiveText.text = objectiveText.text.Substring(0, objectiveText.text.Length - 1);
            yield return new WaitForSeconds(delay);
        }

        // Po dokonèení zastav zvuk
        audioSource.Stop();
        audioSource.loop = false;
    }

    IEnumerator FadeIn(CanvasGroup group, float duration)
    {
        float time = 0;
        group.alpha = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }
        group.alpha = 1;
    }
}
