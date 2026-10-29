using UnityEngine;

public enum Sfx
{
    ButtonHover,
    ButtonClick,
    CardDraw,
    CardPlay,
    TurnStart,
    Victory,
    Defeat,
    Hit,
    Heal
}

/// <summary>
/// Plays music and sound effects. It listens to GameEvents, so no other script needs to know about audio.
/// Every clip slot is optional: an empty slot is simply silent.
/// </summary>
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    [SerializeField] private int humanPlayerIndex = 0;

    [Header("Music")]
    [SerializeField] private AudioClip music;
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;

    [Header("Sound effects")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.8f;
    [SerializeField, Range(0f, 0.2f)] private float pitchVariation = 0.05f;
    [SerializeField] private AudioClip buttonHover;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip cardDraw;
    [SerializeField] private AudioClip cardPlay;
    [SerializeField] private AudioClip turnStart;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip heal;

    private const float MinGap = 0.05f;   // stops the same sound from machine-gunning
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private float[] lastPlayed;

    private void Awake()
    {
        instance = this;
        lastPlayed = new float[System.Enum.GetValues(typeof(Sfx)).Length];

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.clip = music;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        if (music != null) musicSource.Play();
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    private void OnEnable()
    {
        GameEvents.OnTurnStarted += HandleTurnStarted;
        GameEvents.OnCardDrawn += HandleCardDrawn;
        GameEvents.OnCardPlayStarted += HandleCardPlayStarted;
        GameEvents.OnGameOver += HandleGameOver;
        GameEvents.OnDamageDealt += HandleDamageDealt;
        GameEvents.OnHealed += HandleHealed;
    }

    private void OnDisable()
    {
        GameEvents.OnTurnStarted -= HandleTurnStarted;
        GameEvents.OnCardDrawn -= HandleCardDrawn;
        GameEvents.OnCardPlayStarted -= HandleCardPlayStarted;
        GameEvents.OnGameOver -= HandleGameOver;
        GameEvents.OnDamageDealt -= HandleDamageDealt;
        GameEvents.OnHealed -= HandleHealed;
    }

    private void HandleTurnStarted(int player)
    {
        if (player == humanPlayerIndex) Play(Sfx.TurnStart);
    }

    private void HandleCardDrawn(int player, CardData card)
    {
        if (player == humanPlayerIndex) Play(Sfx.CardDraw);
    }

    private void HandleCardPlayStarted(int player, CardData card) => Play(Sfx.CardPlay);

    private void HandleGameOver(int winner) =>
        Play(winner == humanPlayerIndex ? Sfx.Victory : Sfx.Defeat);

    private void HandleDamageDealt(ICombatant target, int amount)
    {
        if (amount > 0) Play(Sfx.Hit);
    }

    private void HandleHealed(ICombatant target, int amount)
    {
        if (amount > 0) Play(Sfx.Heal);
    }

    /// <summary>Call from anywhere: AudioManager.Play(Sfx.ButtonClick). Safe if no AudioManager exists.</summary>
    public static void Play(Sfx sfx)
    {
        if (instance != null) instance.PlayInternal(sfx);
    }

    private void PlayInternal(Sfx sfx)
    {
        AudioClip clip = ClipFor(sfx);
        if (clip == null) return;

        int index = (int)sfx;
        if (Time.unscaledTime - lastPlayed[index] < MinGap) return;
        lastPlayed[index] = Time.unscaledTime;

        sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private AudioClip ClipFor(Sfx sfx)
    {
        switch (sfx)
        {
            case Sfx.ButtonHover: return buttonHover;
            case Sfx.ButtonClick: return buttonClick;
            case Sfx.CardDraw: return cardDraw;
            case Sfx.CardPlay: return cardPlay;
            case Sfx.TurnStart: return turnStart;
            case Sfx.Victory: return victory;
            case Sfx.Defeat: return defeat;
            case Sfx.Hit: return hit;
            case Sfx.Heal: return heal;
            default: return null;
        }
    }
}
