using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Load scene
    public void Play() 
    {
        StartCoroutine(PlayDelayed(0.5f)); // wait 1 second before loading the scene
    }

    private IEnumerator PlayDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Quit game
    public void Quit() 
    {
        Application.Quit();
        Debug.Log("Player has quit the game");
    }
}
