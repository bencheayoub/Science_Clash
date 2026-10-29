using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tiny particle system made of UI images, so it works on a Screen Space - Overlay canvas.
// Put it on a full-screen object in the Canvas. Other scripts call Burst / Rise / Stream / Confetti.
public class UiParticles : MonoBehaviour
{
    private const int SpriteSize = 32;
    private static Sprite dotSprite;

    [SerializeField, Min(10)] private int maxParticles = 300;

    [Header("Ambient dust")]
    [SerializeField] private bool ambientDust = true;
    [SerializeField, Min(0f)] private float dustPerSecond = 5f;
    [SerializeField] private Color dustColor = new Color(0.6f, 0.95f, 1f, 0.35f);

    private class Particle
    {
        public Image image;
        public RectTransform rect;
        public Vector2 position;
        public Vector2 velocity;
        public float age;
        public float life;
        public float fadeIn;
        public float startSize;
        public float gravity;
        public Color color;
        public bool active;
    }

    private readonly List<Particle> particles = new List<Particle>();
    private float dustTimer;

    private void Start()
    {
        if (!ambientDust) return;

        // Fill the screen with dust right away so it doesn't start empty.
        for (int i = 0; i < 30; i++) SpawnDust(true);
    }

    // Sparks flying outward from a point (hits, impacts).
    public void Burst(Vector3 worldPosition, Color color, int count, float speed, float gravity, float size, float life)
    {
        Vector2 origin = ToLocal(worldPosition);
        for (int i = 0; i < count; i++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            float s = speed * Random.Range(0.35f, 1f);
            Spawn(origin, direction * s, color, size * Random.Range(0.6f, 1.2f), life * Random.Range(0.7f, 1.2f), gravity);
        }
    }

    // Sparkles drifting upward from inside a rectangle (healing).
    public void Rise(Vector3 worldCenter, Vector2 areaSize, Color color, int count, float speed, float size, float life)
    {
        Vector2 center = ToLocal(worldCenter);
        for (int i = 0; i < count; i++)
        {
            Vector2 start = center + new Vector2(
                Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
                Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.2f));
            Vector2 velocity = new Vector2(Random.Range(-20f, 20f), speed * Random.Range(0.5f, 1f));
            Spawn(start, velocity, color, size * Random.Range(0.6f, 1.2f), life * Random.Range(0.7f, 1.2f), 0f);
        }
    }

    // A stream of sparks travelling from one point to another (a card flying at its target).
    public void Stream(Vector3 fromWorld, Vector3 toWorld, Color color, int count, float size, float life)
    {
        Vector2 from = ToLocal(fromWorld);
        Vector2 to = ToLocal(toWorld);
        Vector2 velocity = (to - from) / life;

        for (int i = 0; i < count; i++)
        {
            Vector2 start = Vector2.Lerp(from, to, Random.Range(0f, 0.25f)) + Random.insideUnitCircle * 25f;
            Vector2 v = velocity * Random.Range(0.7f, 1.2f) + Random.insideUnitCircle * 40f;
            Spawn(start, v, color, size * Random.Range(0.6f, 1.2f), life * Random.Range(0.9f, 1.1f), 0f);
        }
    }

    // Colored pieces raining down from above the top of the screen.
    public void Confetti(Color[] colors, int count)
    {
        if (colors == null || colors.Length == 0) return;

        Rect area = ((RectTransform)transform).rect;
        for (int i = 0; i < count; i++)
        {
            Vector2 start = new Vector2(
                Random.Range(area.xMin, area.xMax),
                area.yMax + Random.Range(20f, 500f));
            Vector2 velocity = new Vector2(Random.Range(-90f, 90f), Random.Range(-260f, -120f));
            Color color = colors[Random.Range(0, colors.Length)];

            Spawn(start, velocity, color, Random.Range(9f, 18f), Random.Range(2.8f, 4f), 120f);
        }
    }

    private void SpawnDust(bool prewarm)
    {
        Rect area = ((RectTransform)transform).rect;
        Vector2 start = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax));
        Vector2 velocity = new Vector2(Random.Range(-12f, 12f), Random.Range(6f, 22f));
        float life = Random.Range(5f, 9f);

        Spawn(start, velocity, dustColor, Random.Range(3f, 8f), life, 0f,
            fadeIn: 1.5f, startAge: prewarm ? Random.Range(0f, life * 0.8f) : 0f);
    }

    private Vector2 ToLocal(Vector3 worldPosition) => transform.InverseTransformPoint(worldPosition);

    private void Spawn(Vector2 position, Vector2 velocity, Color color, float size, float life, float gravity,
        float fadeIn = 0f, float startAge = 0f)
    {
        Particle p = GetFreeParticle();
        if (p == null) return;   // pool is full, skip this particle

        p.position = position;
        p.velocity = velocity;
        p.age = startAge;
        p.life = life;
        p.fadeIn = fadeIn;
        p.startSize = size;
        p.gravity = gravity;
        p.color = color;
        p.active = true;
        p.image.enabled = true;
        Apply(p, startAge / life);
    }

    private Particle GetFreeParticle()
    {
        foreach (Particle p in particles)
            if (!p.active) return p;

        if (particles.Count >= maxParticles) return null;

        GameObject go = new GameObject("Particle", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        Image image = go.GetComponent<Image>();
        image.sprite = GetDotSprite();
        image.raycastTarget = false;   // never blocks clicks
        image.enabled = false;

        Particle created = new Particle { image = image, rect = (RectTransform)go.transform };
        particles.Add(created);
        return created;
    }

    private void Update()
    {
        // Unscaled time so effects still finish while the game is paused.
        float dt = Time.unscaledDeltaTime;

        if (ambientDust)
        {
            dustTimer += dt * dustPerSecond;
            while (dustTimer >= 1f)
            {
                dustTimer -= 1f;
                SpawnDust(false);
            }
        }

        foreach (Particle p in particles)
        {
            if (!p.active) continue;

            p.age += dt;
            if (p.age >= p.life)
            {
                p.active = false;
                p.image.enabled = false;
                continue;
            }

            p.velocity.y -= p.gravity * dt;
            p.position += p.velocity * dt;
            Apply(p, p.age / p.life);
        }
    }

    private static void Apply(Particle p, float t)
    {
        float size = p.startSize * (1f - t * 0.7f);
        p.rect.localPosition = p.position;
        p.rect.sizeDelta = new Vector2(size, size);

        Color c = p.color;
        c.a *= 1f - t * t;   // stays bright, then fades quickly at the end
        if (p.fadeIn > 0f) c.a *= Mathf.Clamp01(p.age / p.fadeIn);
        p.image.color = c;
    }

    // A soft round dot, built once in code.
    private static Sprite GetDotSprite()
    {
        if (dotSprite != null) return dotSprite;

        Texture2D tex = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        float radius = SpriteSize * 0.5f;
        for (int y = 0; y < SpriteSize; y++)
        {
            for (int x = 0; x < SpriteSize; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(radius, radius)) / radius;
                float alpha = Mathf.Pow(Mathf.Clamp01(1f - d), 1.5f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        tex.Apply();

        dotSprite = Sprite.Create(tex, new Rect(0, 0, SpriteSize, SpriteSize), new Vector2(0.5f, 0.5f), 100f);
        return dotSprite;
    }
}
