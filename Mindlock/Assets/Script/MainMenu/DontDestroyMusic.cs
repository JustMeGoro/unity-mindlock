using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    private static MenuMusic instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Když zjistíme, že jsme v jiné scéně (ne menu ani loading), hudbu vypni
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (currentScene != "MainMenu" && currentScene != "LoadingScreen")
        {
            Destroy(gameObject);
        }
    }
}
