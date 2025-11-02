using UnityEngine;
using System.Runtime.CompilerServices;

//basicamente para centralizar todo lo que refiere a efectos del jugador
public class InputManager : MonoBehaviour
{ 
    [Header("Referencias principales")]
    public GameObject FlashLight;
    public InteractHold interactHold;
    public LloronaAI chillona;

    [Header("Flash HDR Emission")]
    public Material flashMat;             
    public Color flashColor = Color.white; 
    [ColorUsage(false, true)] public Color flashColorHDR = Color.white; // Para ver el HDR en el inspector
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
    [Tooltip("Distancia máxima para activar el efecto de proximidad")]
    public float maxProximityDistance = 15f;
    [Tooltip("Distancia mínima donde el efecto es máximo")]
    public float minProximityDistance = 3f;
    
    [Header("Valores Scan Lines - Base")]
    public float baseScanLinesFreq = 200f;
    public float baseSpeedScanLines = -0.0011f;
    
    [Header("Valores Scan Lines - Máximo")]
    public float minScanLinesFreq = 50f;
    public float maxSpeedScanLines = 2.5f;
    
    private float currentScanLinesFreq;
    private float currentSpeedScanLines;

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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ManageFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && PlayerActions.Instance.playerInventory.HasItem(GameData.ItemType.Flashlight))
        {
            bool active = FlashLight.activeSelf;
            FlashLight.SetActive(!active);
        } 

        if (FlashLight.activeSelf)
        {
            batteryCounter -= Time.deltaTime;
            if (batteryCounter <= 0f)
            {
                if (PlayerActions.Instance.playerInventory.cameraBatteryCount > 1)
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
            // Disminuir la intensidad gradualmente
            currentIntensity = Mathf.MoveTowards(currentIntensity, baseIntensity, Time.deltaTime * fallSpeed);
            
            Color hdrColor = flashColor * currentIntensity;
            flashMat.SetColor("_Color", hdrColor);

            // detener el flash
            if (Mathf.Approximately(currentIntensity, baseIntensity))
            {
                isFlashing = false;
            }
        }
        else
        {
            // Mantener el color base cuando no está flasheando
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
            // (0 = lejos, 1 = muy cerca)
            float proximityFactor = 1f - Mathf.Clamp01((distance - minProximityDistance) / (maxProximityDistance - minProximityDistance));

            // Interpolar frecuencia 
            currentScanLinesFreq = Mathf.Lerp(baseScanLinesFreq, minScanLinesFreq, proximityFactor);

            // Interpolar velocidad 
            currentSpeedScanLines = Mathf.Lerp(baseSpeedScanLines, maxSpeedScanLines, proximityFactor);
        }
        else
        {
            // Volver a valores base si está lejos
            currentScanLinesFreq = baseScanLinesFreq;
            currentSpeedScanLines = baseSpeedScanLines;
        }

        // Aplicar valores al shader
        flashMat.SetFloat("_scanLinesFreq", currentScanLinesFreq);
        flashMat.SetFloat("_SpeedScanLines", currentSpeedScanLines);
    }
}