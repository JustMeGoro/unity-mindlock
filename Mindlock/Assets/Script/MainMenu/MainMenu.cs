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

        //Načti uloženou scénu
        string lastScene = PlayerPrefs.GetString("LastScene", "");
        Debug.Log("Poslední scéna uložená v PlayerPrefs: " + lastScene);

        if (!string.IsNullOrEmpty(lastScene))
        {
            PlayerPrefs.SetString("SceneToLoad", lastScene);
            Debug.Log("SceneToLoad nastaveno na: " + lastScene);
        }
        else
        {
            PlayerPrefs.SetString("SceneToLoad", "Level1");
            Debug.Log("Žádná scéna nebyla nalezena, načítám Level1");
        }

        PlayerPrefs.Save();

        Debug.Log("Přepínám na LoadingScreen...");
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Hráč ukončil hru");
    }
}
