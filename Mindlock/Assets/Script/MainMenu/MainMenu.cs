using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        StartCoroutine(PlayDelayed(0.5f));
    }

    private IEnumerator PlayDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        //Ověří, zda je uložená poslední scéna
        string lastScene = PlayerPrefs.GetString("LastScene", "");

        if (!string.IsNullOrEmpty(lastScene))
        {
            // Uloží ji znovu pro jistotu (aby LoadingScreen věděl, co má načíst)
            PlayerPrefs.SetString("SceneToLoad", lastScene);
        }
        else
        {
            // Fallback – třeba první level
            PlayerPrefs.SetString("SceneToLoad", "Level1");
        }

        PlayerPrefs.Save();

        //Přepni na loading screen
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has quit the game");
    }
}

