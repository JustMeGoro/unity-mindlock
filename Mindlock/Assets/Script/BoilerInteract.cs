using TMPro;
using UnityEngine;

public class BoilerInteract : MonoBehaviour
{
    public string playerTag = "Player";
    [Space(1)]
    public string interactionText;
    public GameObject cogwheelOnBoiler;
    public AudioSource metalClank;
    private bool playerInRange = false;
    private bool cogwheelInPlace = false;

    void Start()
    {
        cogwheelOnBoiler.SetActive(false);
    }

    void Update()
    {
        bool allPipesReady = AreAllPipesReady();

        if (playerInRange && InventoryUI.Instance.isHoldingItem == true && !cogwheelInPlace)
        {
            if (!allPipesReady)
            {
                UIManager.Instance.ShowText("The pipes are missing cogwheels!");
                return;
            }

            if (interactionText != null)
            {
                UIManager.Instance.ShowText(interactionText);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                cogwheelOnBoiler.SetActive(true);
                cogwheelInPlace = true;
                InventoryUI.Instance.isHoldingItem = false;
                UIManager.Instance.HideText();
                metalClank.Play();
                Debug.Log("Final cogwheel placed!");

                TypewriterEffect.Instance.RemoveObjective();

            }
        }
        else if (interactionText != null)
        {

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
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            UIManager.Instance.HideText();
        }
    }
}
