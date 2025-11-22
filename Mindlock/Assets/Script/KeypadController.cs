using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadController : MonoBehaviour
{
    [Header("Reference")]
    public PcColider PcColiderScript;

    [Header("Display")]
    public TextMeshProUGUI displayTMP;

    [Header("Buttons")]
    public Button[] numberButtons;

    [Header("Sound")]
    public AudioSource KeypadAudio;
    public AudioClip[] ButtonPressClips;
    public AudioClip correctSound;    // nový zvuk pro správný kód
    public AudioClip incorrectSound;  // nový zvuk pro špatný kód
    public AudioClip buttonClickSound; // pøidej do inspektoru

    [Header("Settings")]
    public string correctCode = "8714";
    public int codeLength = 4;
    public float showResultSeconds = 1.0f;

    string current = "";

    int clipIndex;

    Color defaultTMPColor;

    bool closeScheduled = false;

    public event System.Action OnCorrectCode;

    void Start()
    {
        if (displayTMP != null) defaultTMPColor = displayTMP.color;
        UpdateDisplay();
    }

    void Update()
    {
        if (displayTMP != null && displayTMP.text == "Correct" && !closeScheduled)
        {
            closeScheduled = true;
            StartCoroutine(CloseParentAfterDelay(2.0f));
        }
    }

    public void PressDigit(int d)
    {
        // Pøehrát zvuk tlaèítka
        if (buttonClickSound != null)
            KeypadAudio.PlayOneShot(buttonClickSound);

        if (current.Length >= codeLength) return;
        current += d.ToString();
        UpdateDisplay();

        if (current.Length >= codeLength)
        {
            SetButtonsInteractable(false); // deaktivace tlaèítek
            StartCoroutine(EvaluateAndReset());
        }
    }

    IEnumerator EvaluateAndReset()
    {
        bool ok = current == correctCode;
        string result = ok ? "Correct" : "Incorrect";

        yield return new WaitForSeconds(0.5f);

        if (displayTMP != null)
        {
            displayTMP.text = result;
            displayTMP.color = ok ? defaultTMPColor : Color.red;
        }

        if (ok && correctSound != null)
            KeypadAudio.PlayOneShot(correctSound);
        else if (!ok && incorrectSound != null)
            KeypadAudio.PlayOneShot(incorrectSound);

        Debug.Log($"KeypadController: entered='{current}' => {result}");

        if (ok)
        {
            PcColiderScript.taskFinished = true;
            OnCorrectCode?.Invoke(); // Vyvolá event
            Room2ObjectiveUI.Instance.RemoveObjective();
            yield return new WaitForSeconds(2.0f);
            // Panel zavøe PcColider!
        }
        else
        {
            yield return new WaitForSeconds(showResultSeconds);
            current = "";
            UpdateDisplay();
            SetButtonsInteractable(true);
        }
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    void SetButtonsInteractable(bool interactable)
    {
        if (numberButtons != null)
        {
            foreach (var btn in numberButtons)
            {
                if (btn != null) btn.interactable = interactable;
            }
        }
    }

    void UpdateDisplay()
    {
        if (displayTMP != null)
        {
            displayTMP.text = current;
            displayTMP.color = defaultTMPColor;
        }
    }

    void RandomButtonSound()
    {
        clipIndex = Random.Range(0, 3);
    }

    IEnumerator CloseParentAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (transform.parent != null)
            transform.parent.gameObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}