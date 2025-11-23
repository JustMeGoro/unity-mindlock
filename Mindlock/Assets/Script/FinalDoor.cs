using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FinalDoor : MonoBehaviour
{
    public PcColider pcColider;
    public AudioSource DoorAudioSource;
    public AudioClip FinalMusic;
    public Image whiteScreenImage;

    bool playerIsNear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color tempColor = whiteScreenImage.color;
        tempColor.a = 0f; // Alpha 0 = prùhledné
        whiteScreenImage.color = tempColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsNear && pcColider.taskFinished && Input.GetKeyDown(KeyCode.E))
        {
            PlayerMovement.Instance.canMove = false;
            PauseMenu.Instance.canPause = false;
            StartCoroutine(OpenDoor()); 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        playerIsNear = true;

        if (!pcColider.taskFinished)
        {
            UIManager.Instance.ShowText("Doors are locked");
        }
        else
        {
            UIManager.Instance.ShowText("Press E to open the door");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        playerIsNear = false;

        UIManager.Instance.HideText();
    }

    IEnumerator OpenDoor()
    {
        DoorAudioSource.PlayOneShot(FinalMusic);
        StartCoroutine(FadeToWhite());
        yield return new WaitWhile(() => DoorAudioSource.isPlaying);
        //zapnevideo
    }

    private IEnumerator FadeToWhite()
    {
        float timer = 0f;
        float duration = 10.0f; // Jak dlouho to trvá (sekundy)
        Color startColor = whiteScreenImage.color;

        // Cílová barva (stejná jako pùvodní, ale Alpha je 1 = neprùhledná)
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Lerp vypoèítá barvu v daném èase
            whiteScreenImage.color = Color.Lerp(startColor, targetColor, timer / duration);
            yield return null; // Èeká na další snímek
        }

        // Na konci pro jistotu nastavíme èistì bílou
        whiteScreenImage.color = targetColor;
    }
}
