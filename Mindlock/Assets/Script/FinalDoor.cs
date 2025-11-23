using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class FinalDoor : MonoBehaviour
{
    public PcColider pcColider;

    [Header("Door audio settings")]
    public AudioSource doorAudioSource;
    public AudioClip doorOpenClose;
    public AudioSource ambientSound;
    public AudioSource FinalAudioSource;
    public AudioClip FinalMusic;
    public GameObject VideoSetActive;

    public Image whiteScreenImage;

    [Header("Video settings")]
    public VideoPlayer finalVideo;

    bool playerIsNear;
    bool alreadyPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalVideo.targetTexture.Release();
        whiteScreenImage.enabled = false;
        VideoSetActive.SetActive(false);
        finalVideo.time= 0;
        Color tempColor = whiteScreenImage.color;
        tempColor.a = 0f; // Alpha 0 = prùhledné
        whiteScreenImage.color = tempColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsNear && pcColider.taskFinished && Input.GetKeyDown(KeyCode.E) && !alreadyPlaying)
        {
            alreadyPlaying = true;
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
        ambientSound.volume = 0;
        doorAudioSource.PlayOneShot(doorOpenClose);
        FinalAudioSource.PlayOneShot(FinalMusic);
        StartCoroutine(FadeToWhite());
        yield return new WaitForSeconds(16);
        VideoSetActive.SetActive(true);
        finalVideo.Play();
        finalVideo.loopPointReached += Prepnout;
    }

    private IEnumerator FadeToWhite()
    {
        float timer = 0f;
        float duration = 10.0f; // Jak dlouho to trvá (sekundy)
        Color startColor = whiteScreenImage.color;
        whiteScreenImage.enabled = true;

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

    void Prepnout(VideoPlayer vp)
    {
        // Tady natvrdo pøepneme na scénu MainMenu
        SceneManager.LoadScene("MainMenu");
    }
}
