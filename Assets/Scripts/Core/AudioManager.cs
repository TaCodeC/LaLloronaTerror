using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Referencias de ruido")]
    public float lastNoiseLevel;
    public Vector3 LastNoisePos;
    private LloronaAI lloronaAI;
    public Image Volume;

    [Header("Reproductor de Casettes")]
    public AudioSource audioSource;           
    public AudioClip cassetteSFX;
    public AudioClip bateriaSFX;
    public AudioClip[] cassetteClips;
    
    public AudioClip Chillonascream;
    public AudioClip FlashLight;
    public AudioClip Flashaso;
    public AudioClip takeKey;
    public AudioClip openDoor;
    public AudioClip escapeAudio;
    public AudioClip gate;
    [Tooltip("Delay entre el SFX y la reproducción del cassette (segundos)")]
    public float cassetteDelay = 1.5f;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        lloronaAI = FindObjectOfType<LloronaAI>();
    }

    public void scream()
    {
        audioSource.clip = Chillonascream;
        audioSource.Play();
    }

    public void reproduceClip(AudioClip clip)
    {
        //no se poq nio lo hice asi antes
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void RegisterNoise(Vector3 position, float noiseLevel)
    {
        lastNoiseLevel = noiseLevel;
        LastNoisePos = position;

        if (lloronaAI)
            lloronaAI.OnNoiseHeard(LastNoisePos, lastNoiseLevel);

        if (Volume)
            Volume.fillAmount = noiseLevel;
    }

    public void reproduceCassette(int cassetteN)
    {
        // Validar entrada
        if (cassetteN < 0 || cassetteN >= cassetteClips.Length)
        {
            return;
        }

        if (audioSource == null)
        {
            return;
        }

        StopAllCoroutines();

        StartCoroutine(PlayCassetteSequence(cassetteN));
    }

    public void insertBattery()
    {
        audioSource.clip = bateriaSFX;
        audioSource.Play();
    }

    private IEnumerator PlayCassetteSequence(int cassetteN)
    {
        // 1️⃣ Reproduce sonido de inserción
        if (cassetteSFX)
        {
            audioSource.clip = cassetteSFX;
            audioSource.Play();
            yield return new WaitForSeconds(cassetteSFX.length + cassetteDelay);
        }

        if (cassetteClips[cassetteN])
        {
            audioSource.clip = cassetteClips[cassetteN];
            audioSource.Play();
        }
    }
}
