using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public int cogwheelCount = 0;
    public int totalCogwheels = 3;

    public TMP_Text objectiveTitleText; // nový odkaz na nadpis
    public TMP_Text objectiveText;      // nový odkaz na text úkolu
    public TMP_Text promptText;

    public float interactRadius = 2f;    //v jaké vzalenosti bude objekt s hráèem reagovat

    private IInteractable currentInteractable;

    private void Start()
    {
        objectiveTitleText.text = "Objectives";
        objectiveTitleText.color = Color.yellow;
        objectiveText.text = "1. Stop the gas";
        objectiveText.color = Color.yellow; // zmìna na žlutou hned na zaèátku
    }

    void Update()
    {
        CheckForInteractable();

        if (currentInteractable != null)
        {

            promptText.text = currentInteractable.GetPromptMessage();
            promptText.enabled = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractable.Interact();
                promptText.enabled = false;

            }
        }
        else
        {
            promptText.enabled = false;
        }
    }

    void CheckForInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius); //vyhledá pøedmìty v radiusu
        var interactables = hits
            .Select(h => h.GetComponent<IInteractable>()) //zkontroluje jestli mají komponentu Interactable
            .Where(i => i != null)  //vezme jen ty který komponentu opradu mají
            .ToList();  //pøidá je do listu interactables

        if (interactables.Count > 0 ) //zkontroluje zda se našli nìjaké objekty
        {
            currentInteractable = interactables[0]; //vybere první objekt v blízkosti
        }
        else
        {
            currentInteractable = null; //nevybere žádný pokud se žádný neneašel
        }
    }


    private void OnDrawGizmosSelected() // Pomocný vizuální kruh pro ladìní
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
            objectiveText.text = "<s>1. Stop the gas</s>";
            objectiveText.color = Color.white; // Zmìní barvu na bílou
            Debug.Log("Objective splnìn, barva zmìnìna na bílou a text pøeškrtnut.");
        }
    }
}

