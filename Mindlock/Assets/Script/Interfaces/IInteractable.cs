 using UnityEngine;

public interface IInteractable
{
    string GetPromptMessage(); //vrací text , který se zobrazí
    void Interact(); //co se stane pøi interakci
}
