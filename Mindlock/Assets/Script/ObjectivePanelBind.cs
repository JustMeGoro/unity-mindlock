using UnityEngine;

public class ObjectivePanelBind : MonoBehaviour
{
    public GameObject objectivePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && objectivePanel != null)
        {
            bool isActive = objectivePanel.activeSelf;
            objectivePanel.SetActive(!isActive);
        }
    }
}