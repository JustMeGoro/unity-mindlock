using TMPro;
using UnityEngine;
using System.Collections;

public class Room2ObjectiveUI : MonoBehaviour
{
    public static Room2ObjectiveUI Instance;

    [Header("Object and delay selector")]
    // Zvolení objektù
    public TMP_Text objectiveTitle;   // Text pro "OBJECTIVE"
    public TMP_Text objectiveText;    // Text pro samotný úkol
    public CanvasGroup objectiveUnderline; // Podtrhnutí
    [Space(1)]
    // Urèení delayù
    public float letterDelay = 0.1f;
    public float startDelay = 1f; // èekání pøed zaèátkem

    [Header("Text options")]
    // Co se kde napíše
    public string title = "OBJECTIVE";
    public string task = "Stop the gas";

    [Header("Audio options")]
    // Volba audiosource
    public AudioSource audioSource;
    // AudioClips
    public AudioClip typingSound;
    public AudioClip backspaceSound;

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        objectiveText.text = "";
        StartCoroutine(ShowObjective());
    }

    IEnumerator ShowObjective()
    {
        yield return new WaitForSeconds(startDelay);

        // Spustíme zvuk
        audioSource.loop = true; // aby se opakoval, pokud je krátký
        audioSource.clip = typingSound;
        audioSource.Play();


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
}
