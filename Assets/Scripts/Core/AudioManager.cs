using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance  { get; private set; }
    public float lastNoiseLevel;
    public Vector3 LastNoisePos;
    LloronaAI lloronaAI;
    public Image Volume;

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

    public void RegisterNoise(Vector3 position, float noiseLevel)
    {
        lastNoiseLevel = noiseLevel;
        LastNoisePos  = position;
        
        if(lloronaAI)
            lloronaAI.OnNoiseHeard(LastNoisePos,  lastNoiseLevel);
        if(Volume)
            Volume.fillAmount = noiseLevel;
            
    }

    
}
