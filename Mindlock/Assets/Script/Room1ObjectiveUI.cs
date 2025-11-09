using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class Room1ObjectiveUI : MonoBehaviour
{
    public static Room1ObjectiveUI Instance;
    public TimeBar timeBar;


    [Header("Object and delay selector")]
    // Zvolení objektů
    public TMP_Text objectiveTitle;   // Text pro "OBJECTIVE"
    public TMP_Text objectiveText;    // Text pro samotný úkol
    public CanvasGroup objectiveTextAplha; // Pro fade in/out textu úkolu
    public CanvasGroup objectiveUnderline; // Podtrhnutí

    [Space(10)]
    // Určení časů
    public float letterDelay = 0.1f;
    public float startDelay = 1f; // čekání před začátkem
    public float removeDuration = 1f; // 

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
    private Color mainColor;

    void Awake()
    {
        Instance = this;   

        mainColor = objectiveText.color;
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
        objectiveText.color = mainColor;
        objectiveTextAplha.alpha = 1;

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
        objectiveTextAplha.alpha = 1;

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

        ObjectiveNotification.Instance.Show();

        audioSource.Stop();
        yield return StartCoroutine(FadeIn(objectiveUnderline, 1f));
        audioSource.Play();

        // Efekt psaní pro úkol
        foreach (char c in task)
        {
            objectiveText.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        // Po dokončení zastav zvuk
        audioSource.Stop();
        audioSource.loop = false;

        timeBar.canTimerStart = true;
    }


    public void RemoveObjective(float letterDelayOverride = -1f)
    {

        StartCoroutine(RemoveTaskCoroutine());


    }

    private IEnumerator RemoveTaskCoroutine()
    {
        // Barva bliknutí při dokončení
        Color flashColor = new Color(1f, 0.9f, 0.1f);
        objectiveText.color = flashColor;

        // pauza před zmizením
        yield return new WaitForSeconds(2f);

        float time = 0f;

        Debug.Log("Removing objective text...");

        // ✅ PLYNULÝ FADE
        while (time < removeDuration)
        {
            time += Time.deltaTime;
            float t = time / removeDuration;

            objectiveTextAplha.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        // ✅ Úplné odstranění po fade-outu
        objectiveTextAplha.alpha = 0f;
        objectiveText.text = "";
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

    public void SceneRestart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
