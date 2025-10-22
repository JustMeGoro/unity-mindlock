using UnityEngine;

public class MonitorAlarmBlink : MonoBehaviour
{
    [Header("Nastavení barvy a rychlosti")]
    public Color brightColor = new Color(1f, 0f, 0f, 1f);  // plná červená
    public Color dimColor = new Color(0.2f, 0f, 0f, 1f);   // tlumená červená
    public float blinkSpeed = 2f;                          // rychlost blikání
    public bool smoothBlink = true;                        // true = plynulé pulzování, false = ostré blikání

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name} nemá SpriteRenderer! Přidej ho nebo použij Light2D verzi.");
            enabled = false;
            return;
        }

        // uloží původní barvu (např. bílá nebo zelená obrazovka)
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        timer += Time.deltaTime * blinkSpeed;

        if (smoothBlink)
        {
            // plynulé přecházení mezi dim a bright barvou
            float t = (Mathf.Sin(timer) + 1f) / 2f;
            spriteRenderer.color = Color.Lerp(dimColor, brightColor, t);
        }
        else
        {
            // ostré blikání zap/vyp
            float blink = Mathf.PingPong(timer, 1f);
            spriteRenderer.color = blink > 0.5f ? brightColor : dimColor;
        }
    }

    // 🔹 volitelné metody pro vypnutí / zapnutí alarmu
    public void ActivateAlarm()
    {
        timer = 0f;
        enabled = true;
    }

    public void DeactivateAlarm()
    {
        // po vypnutí vrátí původní barvu
        spriteRenderer.color = originalColor;
        enabled = false;
    }
}
