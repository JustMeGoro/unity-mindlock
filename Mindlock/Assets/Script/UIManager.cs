using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    public TMP_Text interactionText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowText(string inputText)
    {
        interactionText.text = inputText;
        interactionText.gameObject.SetActive(true);
    }

    public void HideText()
    {
        interactionText.gameObject.SetActive(false);
    }
}
