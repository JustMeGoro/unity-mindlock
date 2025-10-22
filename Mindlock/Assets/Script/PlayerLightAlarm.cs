using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightAlarmController : MonoBehaviour
{
    public static PlayerLightAlarmController Instance;

    [Header("Reference")]
    public Light2D playerLight;
    public AudioSource alarmSound;

    [Header("Alarm – barvy a efekty")]
    public Color alarmBrightColor = new Color(1f, 0f, 0f, 1f);  // jasná červená
    public Color alarmDimColor = new Color(0.4f, 0f, 0f, 1f);   // tmavší červená
    public float alarmPulseSpeed = 2f;
    public float alarmMinRadius = 3f;
    public float alarmMaxRadius = 5f;
    public float alarmMinIntensity = 0.8f;
    public float alarmMaxIntensity = 1.3f;

    [Header("Normální světlo")]
    public float transitionSpeed = 2f; // rychlost přepínání

    // uložené výchozí hodnoty světla
    private Color defaultColor;
    private float defaultRadius;
    private float defaultIntensity;

    private bool alarmActive = false;
    private float pulseTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (playerLight == null)
            playerLight = GetComponent<Light2D>();

        // 🔹 Uložení původních hodnot
        defaultColor = playerLight.color;
        defaultRadius = playerLight.pointLightOuterRadius;
        defaultIntensity = playerLight.intensity;

        // 🔹 Hned aktivuj alarm na startu
        ActivateAlarm();
    }

    void Update()
    {
        if (alarmActive)
        {
            // ALARM mód — pulzování
            pulseTimer += Time.deltaTime * alarmPulseSpeed;
            float t = (Mathf.Sin(pulseTimer) + 1f) / 2f;

            playerLight.color = Color.Lerp(alarmDimColor, alarmBrightColor, t);
            playerLight.pointLightOuterRadius = Mathf.Lerp(alarmMinRadius, alarmMaxRadius, t);
            playerLight.intensity = Mathf.Lerp(alarmMinIntensity, alarmMaxIntensity, t);
        }
        else
        {
            // NORMÁLNÍ mód — návrat k uloženým hodnotám
            playerLight.color = Color.Lerp(playerLight.color, defaultColor, Time.deltaTime * transitionSpeed);
            playerLight.pointLightOuterRadius = Mathf.Lerp(playerLight.pointLightOuterRadius, defaultRadius, Time.deltaTime * transitionSpeed);
            playerLight.intensity = Mathf.Lerp(playerLight.intensity, defaultIntensity, Time.deltaTime * transitionSpeed);
        }

        // 🔸 Testovací přepínač (můžeš odstranit)
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (alarmActive) DeactivateAlarm();
            else ActivateAlarm();
        }
    }

    public void ActivateAlarm()
    {
        alarmActive = true;
        pulseTimer = 0f;
    }

    public void DeactivateAlarm()
    {
        alarmActive = false;
        alarmSound.Stop();
    }
}

