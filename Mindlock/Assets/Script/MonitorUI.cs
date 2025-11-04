using System.Reflection;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class MonitorUI : MonoBehaviour
{
    public static MonitorUI Instance;

    [Header("References")]
    public MonoBehaviour movementComponent;      // pøiøaï movement komponentu v Inspectoru (preferované)
    public Animator monitorUIAnimator;

    [Header("Interaction")]
    public bool playerInRange = false;
    public bool isComputerAccesable = false;      // pokud je false, hráè nemùže interagovat (kontrolováno v triggeru)
    public bool monitorOpen = false;

    [Header("Game Objects")]
    public GameObject ObjectiveText;
    public GameObject InventorySlot;


    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isComputerAccesable && playerInRange && !monitorOpen)
        {
            ObjectiveText.SetActive(false);
            InventorySlot.SetActive(false);

            OpenMonitor();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isComputerAccesable == true)
        {
            UIManager.Instance.ShowText("Press E to interact with computer");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.HideText();
            playerInRange = false;
        }
    }

    public void OpenMonitor()
    {
        PauseMenu.Instance.canPause = false;

        monitorUIAnimator.SetBool("isOpen", true);
        monitorOpen = true;

        ComputerConsole.Instance.ActivateComputer();

        // zakázat pohyb hráèe
        PlayerMovement.Instance.canMove = false;
    }

    public void CloseMonitor()
    {
        PauseMenu.Instance.canPause = true;

        monitorUIAnimator.SetBool("isOpen", false);
        monitorOpen = false;

        // povolit pohyb hráèe
        PlayerMovement.Instance.canMove = true;
    }
}