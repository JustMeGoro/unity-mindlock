using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolaroidInteraction : MonoBehaviour
{
    public enum PhotoKind
    {
        Frogs,
        Stars,
        Potions,
        Tear,
    }
    [Header("PhotoSelection")]
    public PhotoKind photo;
    public Image[] images;

    [Header("Interaction text settings")]
    public string interactionText = "Press E to look at photo";

    [Header("Help text")]
    public TMP_Text HelpText;


    bool isPlayerInside = false;
    bool lookingAtPhoto = false;

    void Start()
    {
        foreach (var item in images)
        {
            item.enabled = false;
        }
        HelpText.enabled = false;
    }


    void Update()
    {
        if (isPlayerInside && !lookingAtPhoto && Input.GetKeyDown(KeyCode.E))
        {
            LookAtPhoto();
        }
        else if (isPlayerInside && lookingAtPhoto && Input.GetKeyDown(KeyCode.E))
        {
            PutPhotoDown();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        isPlayerInside = true;

        UIManager.Instance.ShowText(interactionText);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isPlayerInside = false;

        UIManager.Instance.HideText();
    }

    void LookAtPhoto()
    {
        lookingAtPhoto = true;
        PauseMenu.Instance.canPause = false;
        PlayerMovement.Instance.canMove = false;
        UIManager.Instance.HideText();
        PlayerMovement.Instance.rb.linearVelocity = new Vector2(0, 0);
        HelpText.enabled = true;

        switch (photo)
        {
            case PhotoKind.Potions:
                images[0].enabled = true;
                break;
            case PhotoKind.Frogs:
                images[1].enabled = true;
                break;
            case PhotoKind.Tear:
                images[2].enabled = true;
                break;
            case PhotoKind.Stars:
                images[3].enabled = true;
                break;
        }
    }

    void PutPhotoDown()
    {
        lookingAtPhoto = false;
        PauseMenu.Instance.canPause = true;
        PlayerMovement.Instance.canMove = true;
        UIManager.Instance.ShowText(interactionText);
        HelpText.enabled = false;

        switch (photo)
        {
            case PhotoKind.Potions:
                images[0].enabled = false;
                break;
            case PhotoKind.Frogs:
                images[1].enabled = false;
                break;
            case PhotoKind.Tear:
                images[2].enabled = false;
                break;
            case PhotoKind.Stars:
                images[3].enabled = false;
                break;
        }
    }
}
