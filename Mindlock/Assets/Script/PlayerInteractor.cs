using NUnit.Framework.Interfaces;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public static PlayerInteractor Instance;

    public bool isHoldingItem = false;

    public TMP_Text promptText;

    public float interactRadius = 2f;

    private IInteractable currentInteractable;
    private bool objectiveCompleted = false;
 

    private void Start()
    {
        Instance = this;
    }

    void Update()
    {

        CheckForInteractable();

        if (currentInteractable != null && promptText != null && promptText.gameObject.activeInHierarchy)
        {
            promptText.text = currentInteractable.GetPromptMessage();
            promptText.enabled = true;

            if (Input.GetKeyDown(KeyCode.E) && isHoldingItem == false)
            {
                currentInteractable.Interact();
                promptText.enabled = false;
            }
        }
        else if (promptText != null && promptText.gameObject.activeInHierarchy)
        {
            promptText.enabled = false;
        }
    }

    void CheckForInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        var interactables = hits
            .Select(h => h.GetComponent<IInteractable>())
            .Where(i => i != null)
            .ToList();

        if (interactables.Count > 0)
        {
            currentInteractable = interactables[0];
        }
        else
        {
            currentInteractable = null;
        }
    }

}

