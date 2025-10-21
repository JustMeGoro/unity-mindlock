using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RoomReturn : MonoBehaviour
{
    [Header("UI Settings")]
    public string interactionText = "Press E to enter";
    public Image fadeImage;

    [Header("Player Detection")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip doorOpen;
    public AudioClip doorClose;

    [Header("Animation")]
    public Animator doorAnimator;
    public string openTrigger = "Open";

    public Transform spawnPoint;

    private bool playerIsNear = false;
    private bool isTransitioning = false;

    private GameObject persistentFadeGO;
    private Image persistentFadeImage;
    private GameObject persistentAudioGO;
    private AudioSource persistentAudioSource;
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

        if (persistentAudioSource != null && doorOpen != null)
        {
            persistentAudioSource.PlayOneShot(doorOpen);
            yield return new WaitForSeconds(doorOpen.length);
        }
        else if (audioSource != null && doorOpen != null)
        {
            audioSource.PlayOneShot(doorOpen);
            yield return new WaitForSeconds(doorOpen.length);
        }

        if (persistentTransition != null)
        {
            persistentTransition.SetCloseClip(doorClose);
            persistentTransition.SetAudioSource(persistentAudioSource);
        }

        // Uložení jména dveří pro návrat (volitelně)
        PlayerPrefs.SetString("ReturnDoor", gameObject.name);
        // příznak pro to, že se vracíme do SampleScene
        PlayerPrefs.SetInt("ReturnToSampleScene", 1);

        var asyncOp = SceneManager.LoadSceneAsync("SampleScene"); // nebo index, pokud používáš index
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
        if (persistentAudioSource != null)
            return;

        persistentAudioGO = new GameObject("PersistentAudio");
        persistentAudioSource = persistentAudioGO.AddComponent<AudioSource>();
        persistentAudioSource.playOnAwake = false;
        persistentAudioSource.spatialBlend = 0f;
        DontDestroyOnLoad(persistentAudioGO);

        if (persistentTransition != null)
            persistentTransition.SetAudioSource(persistentAudioSource);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    private class PersistentTransition : MonoBehaviour
    {
        private Image fadeImage;
        private AudioSource persistentAudio;
        private AudioClip closeClip;
        private float fadeSpeed = 2f;
        private bool started = false;
        private bool closePlayed = false;

        public void Setup(Image img)
        {
            fadeImage = img;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void SetAudioSource(AudioSource src)
        {
            persistentAudio = src;
        }

        public void SetCloseClip(AudioClip clip)
        {
            closeClip = clip;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (started)
                return;
            started = true;
            StartCoroutine(PlayCloseThenFadeInAndCleanup());
        }

        private IEnumerator PlayCloseThenFadeInAndCleanup()
        {
            // Pokud se vracíme, najdi InteractionRange, jehož rodič je DoorsHitbox
            if (PlayerPrefs.GetInt("ReturnToSampleScene", 0) == 1)
            {
                GameObject spawnMark = null;

                // Nejprve přímo najít DoorsHitbox a potom jeho dítě InteractionRange
                GameObject doorsHitbox = GameObject.Find("DoorsHitbox");
                if (doorsHitbox != null)
                {
                    Transform child = doorsHitbox.transform.Find("InteractionRange");
                    if (child != null)
                        spawnMark = child.gameObject;
                }

                // Fallback: prohledat všechny Transformy a najít InteractionRange s rodičem DoorsHitbox
                if (spawnMark == null)
                {
                    var all = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
                    foreach (var t in all)
                    {
                        if (t.name == "InteractionRange" && t.parent != null && t.parent.name == "DoorsHitbox")
                        {
                            spawnMark = t.gameObject;
                            break;
                        }
                    }
                }

                if (spawnMark != null)
                {
                    GameObject player = GameObject.FindWithTag("Player");
                    if (player != null)
                    {
                        // Zastavit fyziku pokud existuje (opravena vlastnost)
                        var rb2 = player.GetComponent<Rigidbody2D>();
                        if (rb2 != null)
                        {
                            rb2.linearVelocity = Vector2.zero;
                            rb2.angularVelocity = 0f;
                        }
                        var rb3 = player.GetComponent<Rigidbody>();
                        if (rb3 != null)
                        {
                            rb3.linearVelocity = Vector3.zero;
                            rb3.angularVelocity = Vector3.zero;
                        }

                        player.transform.position = spawnMark.transform.position;
                    }
                }

                PlayerPrefs.DeleteKey("ReturnToSampleScene");
            }

            if (!closePlayed && persistentAudio != null && closeClip != null)
            {
                persistentAudio.PlayOneShot(closeClip);
                closePlayed = true;
                yield return new WaitForSeconds(closeClip.length);
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