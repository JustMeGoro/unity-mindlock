using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorInteractionRoom2 : MonoBehaviour
{
    [Header("UI Settings")]
    public string interactionText = "Press E to enter";
    public Image fadeImage;

    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioSource openAudioSource;
    public AudioSource closeAudioSource;

    [Header("Animation")]
    public Animator doorAnimator;
    public string openTrigger = "Open";

    private bool playerIsNear = false;
    private bool isTransitioning = false;

    private GameObject persistentFadeGO;
    private Image persistentFadeImage;
    private GameObject persistentAudioGO;
    private AudioSource persistentOpenAudioSource;
    private AudioSource persistentCloseAudioSource;
    private PersistentTransition persistentTransition;

    private void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    void Update()
    {
        if (playerIsNear && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EnterNextRoom());
        }
    }

    IEnumerator EnterNextRoom()
    {
        isTransitioning = true;

        if (UIManager.Instance != null)
            UIManager.Instance.HideText();

        if (doorAnimator != null && !string.IsNullOrEmpty(openTrigger))
            doorAnimator.SetTrigger(openTrigger);

        EnsurePersistentFadeExists();
        yield return StartCoroutine(FadeScreenTo(1f));

        PreparePersistentAudioForClose();

        if (persistentOpenAudioSource != null && openAudioSource != null && openAudioSource.clip != null)
        {
            persistentOpenAudioSource.Play();
            yield return new WaitForSeconds(openAudioSource.clip.length);
        }
        else if (openAudioSource != null && openAudioSource.clip != null)
        {
            openAudioSource.Play();
            yield return new WaitForSeconds(openAudioSource.clip.length);
        }

        if (persistentTransition != null)
        {
            persistentTransition.SetCloseAudioSource(persistentCloseAudioSource);
        }

        var asyncOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        while (!asyncOp.isDone)
            yield return null;
    }

    IEnumerator FadeScreenTo(float targetAlpha)
    {
        EnsurePersistentFadeExists();
        Image img = persistentFadeImage != null ? persistentFadeImage : fadeImage;
        if (img == null)
            yield break;

        float speed = 2f;
        float startAlpha = img.color.a;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time);
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, newAlpha);
            yield return null;
        }

        Color final = img.color;
        img.color = new Color(final.r, final.g, final.b, targetAlpha);
    }

    void EnsurePersistentFadeExists()
    {
        if (persistentFadeImage != null && persistentFadeGO != null)
            return;

        persistentFadeGO = new GameObject("PersistentFadeCanvas");
        var canvas = persistentFadeGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;
        persistentFadeGO.AddComponent<CanvasScaler>();
        persistentFadeGO.AddComponent<GraphicRaycaster>();

        GameObject imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(persistentFadeGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0f);

        var rt = imgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        persistentFadeImage = img;
        persistentTransition = persistentFadeGO.AddComponent<PersistentTransition>();
        persistentTransition.Setup(persistentFadeImage);

        DontDestroyOnLoad(persistentFadeGO);
    }

    void PreparePersistentAudioForClose()
    {
        if (persistentOpenAudioSource != null && persistentCloseAudioSource != null)
            return;

        persistentAudioGO = new GameObject("PersistentAudio");
        persistentOpenAudioSource = persistentAudioGO.AddComponent<AudioSource>();
        persistentCloseAudioSource = persistentAudioGO.AddComponent<AudioSource>();

        persistentOpenAudioSource.playOnAwake = false;
        persistentOpenAudioSource.spatialBlend = 0f;
        persistentCloseAudioSource.playOnAwake = false;
        persistentCloseAudioSource.spatialBlend = 0f;

        if (openAudioSource != null && openAudioSource.clip != null)
            persistentOpenAudioSource.clip = openAudioSource.clip;
        if (closeAudioSource != null && closeAudioSource.clip != null)
            persistentCloseAudioSource.clip = closeAudioSource.clip;

        float sfxSaved = PlayerPrefs.GetFloat(SoundManager.SfxKey, 1f);
        persistentOpenAudioSource.volume = sfxSaved * (openAudioSource != null ? openAudioSource.volume : 1f);
        persistentCloseAudioSource.volume = sfxSaved * (closeAudioSource != null ? closeAudioSource.volume : 1f);

        DontDestroyOnLoad(persistentAudioGO);

        if (persistentTransition != null)
            persistentTransition.SetCloseAudioSource(persistentCloseAudioSource);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = true;
            if (UIManager.Instance != null)
                UIManager.Instance.ShowText(interactionText);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerIsNear = false;
            if (UIManager.Instance != null)
                UIManager.Instance.HideText();
        }
    }

    // 🟢 Inner class - fade a zvuk po načtení scény
    private class PersistentTransition : MonoBehaviour
    {
        private Image fadeImage;
        private AudioSource persistentCloseAudio;
        private float fadeSpeed = 2f;
        private bool started = false;
        private bool closePlayed = false;

        public void Setup(Image img)
        {
            fadeImage = img;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void SetCloseAudioSource(AudioSource src)
        {
            persistentCloseAudio = src;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (started) return;
            started = true;
            StartCoroutine(PlayCloseAndFadeIn());
        }

        private IEnumerator PlayCloseAndFadeIn()
        {
            if (!closePlayed && persistentCloseAudio != null && persistentCloseAudio.clip != null)
            {
                persistentCloseAudio.PlayOneShot(persistentCloseAudio.clip);
                closePlayed = true;
                yield return new WaitForSeconds(persistentCloseAudio.clip.length);
            }

            if (fadeImage != null)
            {
                float startAlpha = fadeImage.color.a;
                float time = 0f;
                while (time < 1f)
                {
                    time += Time.deltaTime * fadeSpeed;
                    float newAlpha = Mathf.Lerp(startAlpha, 0f, time);
                    Color c = fadeImage.color;
                    fadeImage.color = new Color(c.r, c.g, c.b, newAlpha);
                    yield return null;
                }
                Color final = fadeImage.color;
                fadeImage.color = new Color(final.r, final.g, final.b, 0f);
            }

            Destroy(gameObject);
        }
    }
}
