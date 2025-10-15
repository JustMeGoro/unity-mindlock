using UnityEngine;
using UnityEngine.Rendering.Universal; // pro ShadowCaster2D

public class WallShadowSwitch : MonoBehaviour
{
    public Transform player;
    public SpriteRenderer wallFront;
    public SpriteRenderer wallBack;

    public float fadeSpeed = 5f; // rychlost přechodu
    public float threshold = 0.95f; // kdy se ShadowCaster aktivuje

    private ShadowCaster2D frontShadow;
    private ShadowCaster2D backShadow;

    void Start()
    {
        frontShadow = wallFront.GetComponent<ShadowCaster2D>();
        backShadow = wallBack.GetComponent<ShadowCaster2D>();
    }

    void Update()
    {
        bool playerBehind = player.position.y > transform.position.y;

        // Fade sprite
        float frontTarget = playerBehind ? 0f : 1f;
        float backTarget = playerBehind ? 1f : 0f;

        wallFront.color = new Color(wallFront.color.r, wallFront.color.g, wallFront.color.b,
                                    Mathf.MoveTowards(wallFront.color.a, frontTarget, fadeSpeed * Time.deltaTime));

        wallBack.color = new Color(wallBack.color.r, wallBack.color.g, wallBack.color.b,
                                    Mathf.MoveTowards(wallBack.color.a, backTarget, fadeSpeed * Time.deltaTime));


        // Aktivace/deaktivace Shadow Casteru
        if (frontShadow != null)
            frontShadow.enabled = wallFront.color.a > (1 - threshold);
        if (backShadow != null)
            backShadow.enabled = wallBack.color.a > threshold;

    }
}
