using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI progressText;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerPrefs.SetFloat("SavedVolume", AudioListener.volume);
        AudioListener.volume = 0;

        StartCoroutine(AnimateLoadingText());
        StartCoroutine(FakeLoadingProgress());
    }

    IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4; // 0,1,2,3 → reset
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator FakeLoadingProgress()
    {
        float progress = 0f;
        float elapsedTime = 0f;

        //Získáme scénu, kterou chceme načíst
        string sceneToLoad = "SampleScene"; //výchozí hodnota
        Debug.Log("Načítám scénu: " + sceneToLoad);

        //Spustíme async načítání, ale nedovolíme aktivaci hned
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;
        Debug.Log("async načítání scény spuštěno.");

        //Simulace postupu načítání
        while (progress < 100f)
        {
            elapsedTime += Time.deltaTime;
            progress += Random.Range(1f, 5f);
            progressText.text = Mathf.Clamp(progress, 0, 100).ToString("F0") + "%";
            yield return new WaitForSeconds(0.2f);
        }

        //povolíme načtení scény
        operation.allowSceneActivation = true;
    }
}
