using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    [Header("UI Settings")]
    public string interactionText = "Press E to enter"; // text u dveøí
    public Image fadeImage; // UI Image (èerný fade panel) - nepovinné, fallback se vytvoøí automaticky

    [Header("Player Detection")]
    public string playerTag = "Player"; // tag hráèe

    [Header("Audio")]
    // místo AudioClipù: pøiøadíš dva AudioSource (každý má svùj clip nastavený v Inspectoru)
    public AudioSource openAudioSource;
    public AudioSource closeAudioSource;

    [Header("Animation")]
    public Animator doorAnimator; // Animator dveøí (volitelný)
    public string openTrigger = "Open";

    private bool playerIsNear = false;
    private bool isTransitioning = false;

    // runtime persistent objects
    private GameObject persistentFadeGO;
    private Image persistentFadeImage;
    private GameObject persistentAudioGO;
    private AudioSource persistentOpenAudioSource;
    private AudioSource persistentCloseAudioSource;

    // persistent helper on the persistent fade GO (survives scene load)
    private PersistentTransition persistentTransition;

    private void Start()
    {
        // pokud už máš pøiøazený fadeImage nastav jeho prùhlednost
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    void Update()
    {
        if (playerIsNear && !isTransitioning && Input.GetKeyDown(KeyCode.E) && BoilerInteract.Instance.objectiveCompleted == true)
        {
            StartCoroutine(EnterNextRoom());
        }
        else if (playerIsNear && !isTransitioning && BoilerInteract.Instance.objectiveCompleted == false)
        {
            UIManager.Instance.ShowText("Doors are locked");
        }
    }

    IEnumerator EnterNextRoom()
    {
        isTransitioning = true;

        if (UIManager.Instance != null)
            UIManager.Instance.HideText();

        // animace dveøí
        if (doorAnimator != null && !string.IsNullOrEmpty(openTrigger))
            doorAnimator.SetTrigger(openTrigger);

        // zajistíme persistentní fade image (fallback) a okamžitì ztmavíme obrazovku
        EnsurePersistentFadeExists();
        yield return StartCoroutine(FadeScreenTo(1f)); // FADE TO BLACK pøed open zvukem

        // pøiprav persistentní audio pøed pøehráním open, aby open dohrálo i pøes naèítání scény
        PreparePersistentAudioForClose();

        // pøehraj open na persistentním source (fallback na lokální pokud persistent chybí)
        if (persistentOpenAudioSource != null && openAudioSource != null && openAudioSource.clip != null)
        {
            // persistentOpenAudioSource.clip už nastaven v PreparePersistentAudioForClose
            // persistentní source má nastavenou hlasitost podle uloženého SFX
            persistentOpenAudioSource.Play();
            yield return new WaitForSeconds(openAudioSource.clip.length);
        }
        else if (openAudioSource != null && openAudioSource.clip != null)
        {
            openAudioSource.Play();
            yield return new WaitForSeconds(openAudioSource.clip.length);
        }
        else
        {
            // žádný clip / source -> pokraèujeme bez èekání
            yield return null;
        }

        // pøipravíme persistentTransition, aby po naètení scény pøehrál close a pak roztmavil
        if (persistentTransition != null)
        {
            persistentTransition.SetCloseAudioSource(persistentCloseAudioSource);
        }

        // naèti další scénu asynchronnì až po dohrání open
        var asyncOp = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        while (!asyncOp.isDone)
            yield return null;

        // po naètení: persistentTransition zpracuje pøehrání close + fade-in
    }

    IEnumerator FadeScreenTo(float targetAlpha)
    {
        // vybereme image: preferuj persistent, pak pøiøazený fadeImage
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

        // ensure final alpha
        Color final = img.color;
        img.color = new Color(final.r, final.g, final.b, targetAlpha);
    }

    void EnsurePersistentFadeExists()
    {
        if (persistentFadeImage != null && persistentFadeGO != null)
            return;

        // vytvoøíme Canvas + Image, pokud neexistuje
        persistentFadeGO = new GameObject("PersistentFadeCanvas");
        var canvas = persistentFadeGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; // hodnì nahoøe
        persistentFadeGO.AddComponent<CanvasScaler>();
        persistentFadeGO.AddComponent<GraphicRaycaster>();

        // child image
        GameObject imgGO = new GameObject("FadeImage");
        imgGO.transform.SetParent(persistentFadeGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0f);

        // roztažení na celou obrazovku
        var rt = imgGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;

        persistentFadeImage = img;

        // pøidáme komponentu, která pøežije naètení scény a vykoná fade-in + pøehrání zavíracího zvuku
        persistentTransition = persistentFadeGO.AddComponent<PersistentTransition>();
        persistentTransition.Setup(persistentFadeImage);

        DontDestroyOnLoad(persistentFadeGO);
    }

    void PreparePersistentAudioForClose()
    {
        // pokud už máme persistent audio, použijeme ho
        if (persistentOpenAudioSource != null && persistentCloseAudioSource != null)
            return;

        // vytvoøíme persistentní GameObject s dvìma AudioSource
        persistentAudioGO = new GameObject("PersistentAudio");
        persistentOpenAudioSource = persistentAudioGO.AddComponent<AudioSource>();
        persistentCloseAudioSource = persistentAudioGO.AddComponent<AudioSource>();

        persistentOpenAudioSource.playOnAwake = false;
        persistentOpenAudioSource.spatialBlend = 0f; // 2D
        persistentCloseAudioSource.playOnAwake = false;
        persistentCloseAudioSource.spatialBlend = 0f; // 2D

        // Zkopíruj clipy z lokálních zdrojù (pokud existují), aby persistentní zdroje mìly clip i po naèítání scény
        if (openAudioSource != null && openAudioSource.clip != null)
            persistentOpenAudioSource.clip = openAudioSource.clip;
        if (closeAudioSource != null && closeAudioSource.clip != null)
            persistentCloseAudioSource.clip = closeAudioSource.clip;

        // nastav poèáteèní hlasitost podle uloženého SFX v SoundManager a lokální relativní hlasitosti
        float sfxSaved = PlayerPrefs.GetFloat(SoundManager.SfxKey, 1f);
        persistentOpenAudioSource.volume = sfxSaved * (openAudioSource != null ? openAudioSource.volume : 1f);
        persistentCloseAudioSource.volume = sfxSaved * (closeAudioSource != null ? closeAudioSource.volume : 1f);

        // pokusíme se oznaèit persistentní GO tagem "SFX", aby tvùj SoundManager ho nalezl pøi dalším Apply
        try
        {
            persistentAudioGO.tag = "SFX";
        }
        catch
        {
            // pokud tag "SFX" neexistuje v projektu, ignore
        }

        DontDestroyOnLoad(persistentAudioGO);

        // pøedáme persistentní audio komponentì fade GO (pokud existuje) pouze close source (ta se pøehraje po naètení)
        if (persistentTransition != null)
            persistentTransition.SetCloseAudioSource(persistentCloseAudioSource);
    }

    void CleanupPersistentAudio()
    {
        if (persistentAudioGO != null)
            Destroy(persistentAudioGO);
        persistentAudioGO = null;
        persistentOpenAudioSource = null;
        persistentCloseAudioSource = null;
    }

    void CleanupPersistentFade()
    {
        if (persistentFadeGO != null)
            Destroy(persistentFadeGO);
        persistentFadeGO = null;
        persistentFadeImage = null;
        persistentTransition = null;
    }

    // Odhlášení handleru pro pøípad, že objekt bude znièen døíve, než pøijde event
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // bezpeèné no-op pokud nebylo pøidáno
    }

    // Tento OnSceneLoaded zùstává jako fallback (nespoléháme na nìj; persistentTransition dìlá práci)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // fallback - nic nedìláme, persistentTransition spravuje fade-in
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

    // persistent helper class: žije na persistentFadeGO a obslouží fade-in po naètení scény
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
            // Pøihlásíme se na sceneLoaded pøímo z tohoto persistentního objektu
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void SetCloseAudioSource(AudioSource src)
        {
            persistentCloseAudio = src;
        }

        private IEnumerator PlayCloseDelayCoroutine(float delay)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            if (!closePlayed && persistentCloseAudio != null && persistentCloseAudio.clip != null)
            {
                persistentCloseAudio.PlayOneShot(persistentCloseAudio.clip);
                closePlayed = true;
                yield return new WaitForSeconds(persistentCloseAudio.clip.length);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // spuštìní pouze jednou
            if (started)
                return;
            started = true;

            // spustíme coroutine pro pøípad, že close ještì nebìžel, a pro fade-in
            StartCoroutine(PlayCloseIfNeededThenFadeInAndCleanup());
        }

        private IEnumerator PlayCloseIfNeededThenFadeInAndCleanup()
        {
            if (!closePlayed && persistentCloseAudio != null && persistentCloseAudio.clip != null)
            {
                persistentCloseAudio.PlayOneShot(persistentCloseAudio.clip);
                closePlayed = true;
                yield return new WaitForSeconds(persistentCloseAudio.clip.length);
            }

            // fade in: z 1 -> 0
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
                // ensure final alpha
                Color final = fadeImage.color;
                fadeImage.color = new Color(final.r, final.g, final.b, 0f);
            }

            // po dokonèení fade-inu smažeme persistentní objekty
            Destroy(gameObject);
        }
    }
}
