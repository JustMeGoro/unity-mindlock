using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public Slider loadingBar;
    public TextMeshProUGUI progressText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerPrefs.SetFloat("SavedVolume", AudioListener.volume);
        AudioListener.volume = 0;

        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        string sceneToLoad = "SampleScene";
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float targetProgress = 0f;

        while (operation.progress < 0.9f)
        {
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = targetProgress;
            progressText.text = (targetProgress * 100f).ToString("F0") + "%";
            yield return null;
        }

        // Pomalé doplnění z 90% na 100%
        float fakeProgress = targetProgress;
        float fillSpeed = 0.15f; // Čím menší, tím pomalejší (např. 0.1f je ještě pomalejší)
        while (fakeProgress < 1f)
        {
            fakeProgress += Time.deltaTime * fillSpeed;
            loadingBar.value = fakeProgress;
            progressText.text = (fakeProgress * 100f).ToString("F0") + "%";
            yield return null;
        }

        operation.allowSceneActivation = true;
    }
}
