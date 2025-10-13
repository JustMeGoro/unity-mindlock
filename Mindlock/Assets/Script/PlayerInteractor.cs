using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public int cogwheelCount = 0;
    public int totalCogwheels = 3;

    public GameObject objectivePanel;
    public TMP_Text objectiveTitleText;
    public TMP_Text objectiveText;
    public TMP_Text promptText;

    public float interactRadius = 2f;

    private IInteractable currentInteractable;
    private bool objectiveCompleted = false;
 

    private void Start()
    {
        // Kontrola, že reference jsou pøiøazené a GameObjecty aktivní
        if (objectiveTitleText != null && objectiveTitleText.gameObject.activeInHierarchy)
        {
            objectiveTitleText.text = "Objectives";
            objectiveTitleText.color = Color.black;
        }
        if (objectiveText != null && objectiveText.gameObject.activeInHierarchy)
        {
            objectiveText.text = "1. Stop the gas";
            objectiveText.color = Color.black;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && objectivePanel != null)
        {
            bool isActive = objectivePanel.activeSelf;
            objectivePanel.SetActive(!isActive);
        }

        if (objectivePanel != null && !objectivePanel.activeSelf)
            return;

        CheckForInteractable();

        if (currentInteractable != null && promptText != null && promptText.gameObject.activeInHierarchy)
        {

            promptText.text = currentInteractable.GetPromptMessage();
            promptText.enabled = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractable.Interact();
                promptText.enabled = false;
            }
        }
        else if (promptText != null && promptText.gameObject.activeInHierarchy)
        {
            promptText.enabled = false;
        }

        // Nastav text a barvu podle stavu objectiveCompleted
        if (objectivePanel != null && objectivePanel.activeSelf && objectiveText != null)
        {
            if (objectiveCompleted)
            {
                objectiveText.text = "<s>1. Stop the gas</s>";
                objectiveText.color = Color.white;
            }
            else
            {
                objectiveText.text = "1. Stop the gas";
                objectiveText.color = Color.black;
            }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

    public void AddCogwheel()
    {
        cogwheelCount++;
        Debug.Log($"Cogwheels: {cogwheelCount}/{totalCogwheels}");

        if (cogwheelCount == totalCogwheels)
        {
            objectiveCompleted = true;
            Debug.Log("Objective splnìn, barva zmìnìna na bílou a text pøeškrtnut.");
        }
    }
}

