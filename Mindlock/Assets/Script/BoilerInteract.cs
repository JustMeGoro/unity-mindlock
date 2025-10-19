using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class BoilerInteract : MonoBehaviour
{
    public TMP_Text interactionText;
    public string playerTag = "Player";
    public GameObject cogwheelOnBoiler;
    public AudioSource metalClank;
    private bool playerInRange = false;
    private bool cogwheelInPlace = false;
    public VideoPlayer videoPlayer;

    void Start()
    {
        cogwheelOnBoiler.SetActive(false);
        if (interactionText != null)
            interactionText.enabled = false;
    }

    void Update()
    {
        bool allPipesReady = AreAllPipesReady();

        if (playerInRange && PlayerInteractor.Instance.isHoldingItem && !cogwheelInPlace)
        {
            if (!allPipesReady)
            {
                interactionText.text = "The pipes are missing cogwheels!";
                interactionText.enabled = true;
                return;
            }

            if (interactionText != null)
            {
                interactionText.text = "Press E to place final cogwheel";
                interactionText.enabled = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                cogwheelOnBoiler.SetActive(true);
                cogwheelInPlace = true;
                PlayerInteractor.Instance.isHoldingItem = false;
                interactionText.enabled = false;
                metalClank.Play();
                Debug.Log("Final cogwheel placed!");

                TypewriterEffect.Instance.RemoveObjective();

                videoPlayer.Play();

            }
        }
        else if (interactionText != null)
        {
            interactionText.enabled = false;
        }
    }

    bool AreAllPipesReady()
    {
        foreach (var pipe in PipeInteract.allPipes)
        {
            if (!pipe.cogwheelInPlace)
                return false;
        }
        return true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }
}
