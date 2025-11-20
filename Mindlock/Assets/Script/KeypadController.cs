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

    [Header("Settings")]
    public string correctCode = "9714";
    public int codeLength = 4;
    public float showResultSeconds = 1.0f;

    string current = "";

    void Start()
    {
        if (numberButtons != null)
        {
            foreach (var btn in numberButtons)
                btn.onClick.RemoveAllListeners();

            for (int i = 0; i < numberButtons.Length; i++)
            {
                var btn = numberButtons[i];
                if (btn == null) continue;

                // zkuseme pøeèíst štítek tlaèítka (TMP nebo Text)
                string label = GetButtonLabel(btn);
                if (int.TryParse(label, out int digit))
                {
                    int captured = digit; // nutné pro správné uzavøení
                    btn.onClick.AddListener(() => OnNumberPressed(captured));
                }
                else
                {
                    // fallback: pokud nemá label, zapíšeme debug
                    btn.onClick.AddListener(() => Debug.Log($"KeypadController: Button '{btn.name}' clicked (no numeric label)"));
                }
            }
        }

        UpdateDisplay();
    }

    string GetButtonLabel(Button btn)
    {
        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) return tmp.text.Trim();
        var txt = btn.GetComponentInChildren<Text>();
        if (txt != null) return txt.text.Trim();
        return "";
    }

    void OnNumberPressed(int d)
    {
        Debug.Log($"KeypadController: Button clicked -> {d}");
        PressDigit(d);
    }

    public void PressDigit(int d)
    {
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
}