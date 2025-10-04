using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public int cogwheelCount = 0;
    public int totalCogwheels = 3;
    public TMP_Text cogwheelObjectiveText;

    public float interactRadius = 2f;    //v jaké vzalenosti bude objekt s hráèem reagovat
    public TMP_Text promptText;

    private IInteractable currentInteractable;

    private void Start()
    {

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

        if (cogwheelCount == totalCogwheels)
        {
            cogwheelObjectiveText.text = "All cogwheels found," + "\n" +
                   "doors are unlocked!";
                                
        }
        else
        {
            cogwheelObjectiveText.text = "Find cogwheels " + cogwheelCount + "/" + totalCogwheels;
        }
    }

}

