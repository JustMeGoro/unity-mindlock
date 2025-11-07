using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class ComputerConsole : MonoBehaviour
{
    public static ComputerConsole Instance;

    [Header("Scene References")]
    public VideoPlayer bootVideo;
    public GameObject consoleUI;
    public TMP_Text consoleText;
    public TMP_InputField inputField;

    [Header("Sounds")]
    public AudioSource computerAudioSource;
    public AudioClip beep;         // zvuk alarmu
    public AudioClip startup;    // zvuk pøi "Rebooting..."

    private bool busy = true; // aby nešlo psát bìhem animace
    private bool rebooted = false;

    // odemèení dveøí 
    public bool doorsUnlocked = false;

    void Start()
    {
        Instance = this;

        consoleUI.SetActive(false);
        bootVideo.loopPointReached += OnBootEnd;
    }

    public void ActivateComputer()
    {
        bootVideo.Play();

        Room1ObjectiveUI.Instance.objectiveText.text = "";
    }

    void OnBootEnd(VideoPlayer vp)
    {

        bootVideo.gameObject.SetActive(false);
        consoleUI.SetActive(true);
        inputField.gameObject.SetActive(true);

        computerAudioSource.PlayOneShot(beep);

        consoleText.text =
            "GURTCORP SYSTEM TERMINAL v2.4\n" +
            "-----------------------------------\n" +
            "> ALERT: Gas leak detected in Sector B.\n" +
            "> Automatic lockdown engaged.\n" +
            "> All doors sealed.\n" +
            "> Command for reboot: power on systems\n" +
            "> Type command: ";

        inputField.ActivateInputField();

        busy = false;
    }

    void Update()
    {
        if (consoleUI.activeSelf && busy == false && Input.GetKeyDown(KeyCode.Return) && !rebooted)
        {
            string command = inputField.text.ToLower().Trim();
            inputField.text = "";
            inputField.ActivateInputField();

            ExecuteCommand(command);
        }

        if (rebooted == true && Input.GetKeyDown(KeyCode.Return))
        {
            MonitorUI.Instance.CloseMonitor();

            MonitorUI.Instance.ObjectiveText.SetActive(true);
            MonitorUI.Instance.InventorySlot.SetActive(true);


            StartCoroutine(Room1ObjectiveUI.Instance.ShowNextObjective("- Exit the room"));
        }
    }

    void ExecuteCommand(string command)
    {
        if (command == "power on systems")
        {
            inputField.DeactivateInputField();
            busy = true;
            inputField.gameObject.SetActive(false);
            StartCoroutine(RebootSequence(command));
        }
        else if (command != "")
        {
            computerAudioSource.PlayOneShot(beep);

            consoleText.text = "GURTCORP SYSTEM TERMINAL v2.4\n" +
            "-----------------------------------\n" +
            "> ALERT: Gas leak detected in Sector B.\n" +
            "> Automatic lockdown engaged.\n" +
            "> All doors sealed.\n" +
            "> Command for reboot: power on systems\n" +
            "> Type command:\n" +
            "> " + command + " is unknown command.\n ";
        }

        Canvas.ForceUpdateCanvases();
    }

    IEnumerator RebootSequence(string command)
    {
        busy = true;
        inputField.DeactivateInputField();

        consoleText.text = "GURTCORP SYSTEM TERMINAL v2.4\n" +
                    "-----------------------------------\n";

        computerAudioSource.PlayOneShot(beep);
        consoleText.text += "> Rebooting";

        // teèky (loading efekt)
        for (int i = 0; i < 3; i++)
        {
            inputField.DeactivateInputField();
            consoleText.text += ".";

            computerAudioSource.PlayOneShot(beep);

            Canvas.ForceUpdateCanvases();
            inputField.DeactivateInputField();

            yield return new WaitForSeconds(1.0f);
        }

        consoleText.text = "GURTCORP SYSTEM TERMINAL v2.4\n" +
                            "-----------------------------------\n";


        yield return new WaitForSeconds(1.5f);

        consoleText.text += "> Lockdown disengaged.\n";
        computerAudioSource.PlayOneShot(beep);

        yield return new WaitForSeconds(1.2f);

        consoleText.text += "> Redirecting power\n";
        computerAudioSource.PlayOneShot(beep);

        yield return new WaitForSeconds(1.4f);

        consoleText.text += "> Systems online\n";
        computerAudioSource.PlayOneShot(beep);


        Canvas.ForceUpdateCanvases();
        yield return new WaitForSeconds(1.0f);

        // dramatické zobrazení "ALL DOORS UNLOCKED"
        computerAudioSource.PlayOneShot(startup);
        consoleText.text += "<color=#00FF00><b>> ALL DOORS UNLOCKED</b></color>\n ";
        consoleText.text += "<color=#00260f>> (press enter to exit console)</color>\n ";
        Canvas.ForceUpdateCanvases();
        
        doorsUnlocked = true;
        rebooted = true;
        PlayerLightAlarmController.Instance.systemsOnline = true;
        MonitorUI.Instance.isComputerAccesable = false;
    }
}
