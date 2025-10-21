using UnityEngine;

[ExecuteAlways]
public class PipeSteamEffect : MonoBehaviour
{
    private const string SteamObjectName = "PipeSteam";

    [Header("Steam Placement")]
    [SerializeField]
    private Vector3 steamLocalOffset = new Vector3(0f, 1.05f, 0f);

    [Header("Emission")]
    [SerializeField]
    private float emissionRate = 18f;

    [Header("Lifetime")]
    [SerializeField]
    private Vector2 lifetimeRange = new Vector2(1.2f, 1.8f);

    [Header("Start Size")]
    [SerializeField]
    private Vector2 sizeRange = new Vector2(0.35f, 0.6f);

    private Gradient steamGradient;

    private void OnEnable()
    {
        CreateOrUpdateSteam();
    }

    private void OnValidate()
    {
        CreateOrUpdateSteam();
    }

    private void Reset()
    {
        CreateOrUpdateSteam();
    }

    private Gradient SteamGradient
    {
        get
        {
            if (steamGradient == null || steamGradient.colorKeys == null || steamGradient.colorKeys.Length == 0)
            {
                steamGradient = new Gradient();
                steamGradient.SetKeys(
                    new[]
                    {
                        new GradientColorKey(new Color(0.9f, 0.9f, 1f), 0f),
                        new GradientColorKey(Color.white, 1f)
                    },
                    new[]
                    {
                        new GradientAlphaKey(0.65f, 0f),
                        new GradientAlphaKey(0.28f, 0.55f),
                        new GradientAlphaKey(0f, 1f)
                    });
            }

            return steamGradient;
        }
    }

    private void CreateOrUpdateSteam()
    {
        var steamTransform = transform.Find(SteamObjectName);
        if (steamTransform == null)
        {
            var steamObject = new GameObject(SteamObjectName);
            steamTransform = steamObject.transform;
            steamTransform.SetParent(transform, false);
        }

        steamTransform.localPosition = steamLocalOffset;
        steamTransform.localRotation = Quaternion.identity;
        steamTransform.localScale = Vector3.one;

        var particleSystem = steamTransform.GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            particleSystem = steamTransform.gameObject.AddComponent<ParticleSystem>();
        }

        ConfigureParticleSystem(particleSystem);
    }

    private void ConfigureParticleSystem(ParticleSystem particleSystem)
    {
        var main = particleSystem.main;
        main.loop = true;
        main.duration = 4f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetimeRange.x, lifetimeRange.y);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);
        main.startSize = new ParticleSystem.MinMaxCurve(sizeRange.x, sizeRange.y);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, 0.35f));
        main.gravityModifier = 0f;
        main.playOnAwake = true;
        main.maxParticles = 512;

        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRate);

        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.08f;
        shape.radiusThickness = 0.5f;
        shape.arcMode = ParticleSystemShapeMultiModeValue.Loop;

        var velocity = particleSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.08f, 0.08f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.25f, 0.45f);

        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(SteamGradient);

        var sizeOverLifetime = particleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        var sizeCurve = new AnimationCurve(
            new Keyframe(0f, 0.3f),
            new Keyframe(0.5f, 0.95f),
            new Keyframe(1f, 1.2f));
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var noise = particleSystem.noise;
        noise.enabled = true;
        noise.strength = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
        noise.frequency = 0.35f;
        noise.scrollSpeed = new ParticleSystem.MinMaxCurve(0.15f, 0.25f);

        var rotation = particleSystem.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-8f, 8f);

        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.View;
        renderer.sortingOrder = 5;

        if (!Application.isPlaying)
        {
            particleSystem.Simulate(0f, true, true, true);
        }

        if (!particleSystem.isPlaying && Application.isPlaying)
        {
            particleSystem.Play();
        }
    }
}
