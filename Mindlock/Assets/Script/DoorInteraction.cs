using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // pokud používáš UI Text (Canvas)

public class DoorInteraction : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject interactionText; // objekt s Textem nebo TMP_Textem

    [Header("Player Detection")]
    public string playerTag = "Player"; // tag hráèe

    private bool playerIsNear = false;

    void Start()
    {
        if (interactionText != null)
            interactionText.SetActive(false); // text je skrytý na zaèátku
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Entering next room...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = true;
            if (interactionText != null)
                interactionText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = false;
            if (interactionText != null)
                interactionText.SetActive(false);
        }
    }
}
