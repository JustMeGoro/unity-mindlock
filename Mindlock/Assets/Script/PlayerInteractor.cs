using NUnit.Framework.Interfaces;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public static PlayerInteractor Instance; // vytvoøí promìnnou pro odkaz na tento script

    public bool isHoldingItem = false; // promìnná bool co urèuje jestli postava nìco drží

    public TMP_Text promptText;

    public float interactRadius = 2f; // radius ve kterém postava mùže používat vìci

    private IInteractable currentInteractable; // momentální interactable
 

    private void Start()
    {
        Instance = this; // do promìnné instance uloží odkaz na tento script
    }

    void Update()
    {

        CheckForInteractable(); //pøejde na void CheckForInteractables

        if (currentInteractable != null && promptText != null && promptText.gameObject.activeInHierarchy) // jestli se CheckForInteractables vrátilo s itemem tak...
        {
            promptText.text = currentInteractable.GetPromptMessage(); // napíše na obrazovku jaký item lze vzít
            promptText.enabled = true; // zobrazí text na obrazovce

            if (Input.GetKeyDown(KeyCode.E) && isHoldingItem == false) // pokud se zmáèkne klávesa E a zároveò postava nic nedrží tak...
            {
                currentInteractable.Interact(); // item vezme do rukou a zobrazí se v inventáøi
                promptText.enabled = false; // odstraní text pro sebrání pomocí klávesy E z obrazovky
                isHoldingItem = true;
            }
        }
        else if (promptText != null && promptText.gameObject.activeInHierarchy) // Jinak...
        {
            promptText.enabled = false; // text pro sebrání pomocí klávesy E se na obrazovce nezobrazí
        }
    }

    void CheckForInteractable() // hledá v okolí postavy itemy se kterýma mùže interaktovat
    {
        // hledání itemù v okolí a vytvoøí list s danýma itemama
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        var interactables = hits
            .Select(h => h.GetComponent<IInteractable>())
            .Where(i => i != null)
            .ToList();

        if (interactables.Count > 0) // pokud je v okolí alespoò jeden item tak..
        {
            currentInteractable = interactables[0]; // první item v seznamu uloží do promenné
        }
        else
        {
            currentInteractable = null; // jinak bude promenná prázdná
        }
    }

}

