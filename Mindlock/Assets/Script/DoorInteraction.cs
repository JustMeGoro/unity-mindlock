using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // pokud používáš UI Text (Canvas)

public class DoorInteraction : MonoBehaviour
{
    public static DoorInteraction Instance;

    [Header("UI Settings")]
    public string interactionText; // obsah interaction textu

    [Header("Player Detection")]
    public string playerTag = "Player"; // tag hráèe

    private bool playerIsNear = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerIsNear = false;
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
            UIManager.Instance.ShowText(interactionText);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = false;
            UIManager.Instance.HideText();
        }
    }
}
