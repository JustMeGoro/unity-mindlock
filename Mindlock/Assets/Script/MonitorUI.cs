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
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isComputerAccesable && playerInRange)
        {
            OpenMonitor();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && monitorOpen)
        {
            CloseMonitor();
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
        monitorUIAnimator.SetBool("isOpen", true);
        monitorOpen = true;
    }

    public void CloseMonitor()
    {
        monitorUIAnimator.SetBool("isOpen", false);
        monitorOpen = false;
    }
}