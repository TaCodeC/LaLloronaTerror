using UnityEngine;
using System.Runtime.CompilerServices;

public class InputManager : MonoBehaviour
{ 
    [Header("Referencias principales")]
    public GameObject FlashLight;
    public InteractHold interactHold;
    public LloronaAI chillona;
    public Camera playerCamera;
    [Header("Flash HDR Emission")]
    public Material flashMat;             
    public Color flashColor = Color.white; 
    [ColorUsage(false, true)] public Color flashColorHDR = Color.white;
    public float baseIntensity = 1f;       
    public float peakIntensity = 12f;      
    public float riseSpeed = 40f;          
    public float fallSpeed = 2.5f;         
    private Color originalColor;       
    private float currentIntensity;
    private bool isFlashing;

    [Header("Conf Batería")]
    private float batteryCounter;
    [Min(3)] public int batterieDuration = 3;

    [Header("Stun CDs y duraciones")]
    [Min(3)] public float stunDuration = 3;
    [Min(3)] private float stunCounter;
    [Min(3)] public float stunColdownDuration = 3;
    private float stunColdownCounter;

    [Header("Efecto de Proximidad")]
    public float maxProximityDistance = 15f;
    public float minProximityDistance = 3f;
    
    [Header("Valores Scan Lines - Base")]
    public float baseScanLinesFreq = 200f;
    public float baseSpeedScanLines = -0.0011f;
    
    [Header("Valores Scan Lines - Máximo")]
    public float minScanLinesFreq = 50f;
    public float maxSpeedScanLines = 2.5f;
    
    private float currentScanLinesFreq;
    private float currentSpeedScanLines;

    [Header("Screamer Config")]
    public float distanciaScreamer = 2.5f;
    private bool screamerTriggered;

    void Awake()
    {
        batteryCounter = batterieDuration;
        FlashLight.SetActive(false);
        if (flashMat)
        {
            originalColor = flashMat.GetColor("_Color");
            currentIntensity = baseIntensity;
            currentScanLinesFreq = baseScanLinesFreq;
            currentSpeedScanLines = baseSpeedScanLines;
        }
    }

    void Update()
    {
        interactHold.checkInteraction();
        ManageFlashlight();
        FlashStun();
        HandleFlash();
        HandleProximityEffect();
        CheckScreamerDistance();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ManageFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && PlayerActions.Instance.playerInventory.HasItem(GameData.ItemType.Flashlight))
        {
            bool active = FlashLight.activeSelf;
            AudioManager.Instance.reproduceClip(AudioManager.Instance.FlashLight);
            if (!active && PlayerActions.Instance.playerInventory.cameraBatteryCount <= 0) return;
            if(GameCore.Instance.objective==GameData.Objective.FLASHLIGHT) 
                GameCore.Instance.setObjective(GameData.Objective.ENERGY);
            FlashLight.SetActive(!active);
        }

        if (FlashLight.activeSelf)
        {
            batteryCounter -= Time.deltaTime;
            if (batteryCounter <= 1f)
            {
                if (PlayerActions.Instance.playerInventory.cameraBatteryCount > 0)
                {
                    PlayerActions.Instance.playerInventory.minusBattery();
                    batteryCounter = batterieDuration;
                    PlayerActions.Instance.UpdateUI();
                }
                else
                {
                    PlayerActions.Instance.playerInventory.minusBattery();
                    batteryCounter = batterieDuration;
                    PlayerActions.Instance.UpdateUI();
                    FlashLight.SetActive(false);
                    AudioManager.Instance.reproduceClip(AudioManager.Instance.FlashLight);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void FlashStun()
    {
        if (Input.GetKeyDown(KeyCode.X) && stunColdownCounter <= 0)
        {
            if (PlayerActions.Instance.playerInventory.cameraBatteryCount >= 1)
            {
                PlayerActions.Instance.playerInventory.minusBattery();
                PlayerActions.Instance.UpdateUI();
                AudioManager.Instance.reproduceClip(AudioManager.Instance.Flashaso);
                batteryCounter = batterieDuration;
                currentIntensity = peakIntensity;
                isFlashing = true;
                if (chillona.IsTargetVisible())
                {
                    chillona.changeState(GameData.LloronaState.STUN);
                    stunColdownCounter = stunColdownDuration;
                    stunCounter = stunDuration;
                }
            }
        }

        if (stunColdownCounter > 0)
        {
            stunColdownCounter -= Time.deltaTime;
            if (stunColdownCounter <= 0f) stunColdownCounter = 0;
        }

        if (stunCounter > 0)
        {
            stunCounter -= Time.deltaTime;
            if (stunCounter <= 0f)
            {
                stunCounter = 0;
                chillona.changeState(GameData.LloronaState.STAY);
            }
        }
    }

    void HandleFlash()
    {
        if (!flashMat) return;
        if (isFlashing)
        {
            currentIntensity = Mathf.MoveTowards(currentIntensity, baseIntensity, Time.deltaTime * fallSpeed);
            Color hdrColor = flashColor * currentIntensity;
            flashMat.SetColor("_Color", hdrColor);
            if (Mathf.Approximately(currentIntensity, baseIntensity)) isFlashing = false;
        }
        else
        {
            Color baseColor = flashColor * baseIntensity;
            flashMat.SetColor("_Color", baseColor);
        }
    }

    void HandleProximityEffect()
    {
        if (!flashMat || !chillona) return;
        float distance = Vector3.Distance(transform.position, chillona.transform.position);
        if (distance <= maxProximityDistance)
        {
            float proximityFactor = 1f - Mathf.Clamp01((distance - minProximityDistance) / (maxProximityDistance - minProximityDistance));
            currentScanLinesFreq = Mathf.Lerp(baseScanLinesFreq, minScanLinesFreq, proximityFactor);
            currentSpeedScanLines = Mathf.Lerp(baseSpeedScanLines, maxSpeedScanLines, proximityFactor);
        }
        else
        {
            currentScanLinesFreq = baseScanLinesFreq;
            currentSpeedScanLines = baseSpeedScanLines;
        }
        flashMat.SetFloat("_scanLinesFreq", currentScanLinesFreq);
        flashMat.SetFloat("_SpeedScanLines", currentSpeedScanLines);
    }

    void CheckScreamerDistance()
    {
        if (!chillona || screamerTriggered) return;
        float distance = Vector3.Distance(transform.position, chillona.transform.position);
        if (distance < distanciaScreamer)
        {
            screamerTriggered = true;
            if (AudioManager.Instance.Chillonascream)
            {
              AudioManager.Instance.reproduceClip(AudioManager.Instance.Chillonascream);
            }

            Invoke(nameof(TriggerRespawn), 3f);
        }
    }

    void TriggerRespawn()
    {
        GameCore.Instance.orderRespawn();
        screamerTriggered = false;
    }
}
