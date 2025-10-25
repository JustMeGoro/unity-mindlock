using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject PauseMenuCanvas;

    void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                ResumeInstant();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void ResumeInstant()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void ResumeWithDelay()
    {
        StartCoroutine(ResumeCoroutine());
    }

    IEnumerator ResumeCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
        PauseMenuCanvas.SetActive(false);
        paused = false;
    }

    public void MainMenuButton()
    {
        StartCoroutine(MainMenuCoroutine());
    }

    IEnumerator MainMenuCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // 💾 Uloží aktuální scénu, aby ji šlo načíst zpět
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastScene", currentScene);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
