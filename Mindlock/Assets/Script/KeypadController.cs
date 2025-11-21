using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadController : MonoBehaviour
{
    [Header("Display")]
    public TextMeshProUGUI displayTMP; // pøetáhni CodeDisplay (TMP) nebo
    public Text displayText;          // pøetáhni klasický Text, pokud nepoužíváš TMP

    [Header("Buttons")]
    public Button[] numberButtons; // pøetáhni sem 9 tlaèítek v Inspectoru

    [Header("Sound")]
    public AudioSource KeypadAudio;
    public AudioClip[] ButtonPressClips;

    [Header("Settings")]
    public string correctCode = "8714";
    public int codeLength = 4;
    public float showResultSeconds = 1.0f;

    string current = "";

    int clipIndex;

    void Start()
    {
        UpdateDisplay();
    }


    public void PressDigit(int d)
    {
        RandomButtonSound();
        KeypadAudio.PlayOneShot(ButtonPressClips[clipIndex]);

        if (current.Length >= codeLength) return;
        current += d.ToString();
        UpdateDisplay();

        if (current.Length >= codeLength)
            StartCoroutine(EvaluateAndReset());
    }

    IEnumerator EvaluateAndReset()
    {
        bool ok = current == correctCode;
        string result = ok ? "Correct" : "Incorrect";

        if (displayTMP != null) displayTMP.text = result;
        if (displayText != null) displayText.text = result;

        Debug.Log($"KeypadController: entered='{current}' => {result}");

        yield return new WaitForSeconds(showResultSeconds);

        current = "";
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (displayTMP != null) displayTMP.text = current;
        if (displayText != null) displayText.text = current;
    }

    void RandomButtonSound()
    {
        clipIndex = Random.Range(0, 3);
    }
}