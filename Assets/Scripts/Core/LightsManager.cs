using UnityEngine;
using System.Collections;

public class LightsManager : MonoBehaviour
{
    // Singleton
    public static LightsManager Instance { get; private set; }

    [Header("Luces de Emergencia (titilan al inicio)")]
    public Light[] emergencyLights;
    [Range(0f, 100f)] public float emergencyIntensity = 2f;
    [Range(0f, 20f)] public float emergencyRange = 10f;
    [Tooltip("Velocidad del titileo de las luces de emergencia")]
    public float emergencyBlinkSpeed = 2f;

    [Header("Luces Generales (encendidas desde el inicio)")]
    public Light[] generalLights;
    [Range(0f, 100f)] public float generalIntensity = 3f;
    [Range(0f, 20f)] public float generalRange = 15f;

    [Header("Luces de Sala de Control (4 luces)")]
    public Light[] controlRoomLights; // deben ser 4
    [Range(0f, 100f)] public float controlLightIntensity = 4f;
    [Range(0f, 20f)] public float controlLightRange = 12f;
    [Tooltip("Tiempo entre apagados secuenciales en shutDownControlLight")]
    public float controlLightOffDelay = 1.5f;

    private bool emergencyBlinking = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InitializeEmergencyLights();
        InitializeGeneralLights();
        InitializeControlLights();

        StartCoroutine(EmergencyLightBlink());
    }

    void InitializeEmergencyLights()
    {
        foreach (Light l in emergencyLights)
        {
            if (l == null) continue;
            l.intensity = emergencyIntensity;
            l.range = emergencyRange;
            l.color = Color.red; // luces de emergencia rojas
            l.enabled = true;
        }
    }

    void InitializeGeneralLights()
    {
        foreach (Light l in generalLights)
        {
            if (l == null) continue;
            l.intensity = generalIntensity;
            l.range = generalRange;
            l.color = Color.white;
            l.enabled = true;
        }
    }

    void InitializeControlLights()
    {
        foreach (Light l in controlRoomLights)
        {
            if (l == null) continue;
            l.intensity = controlLightIntensity;
            l.range = controlLightRange;
            l.color = Color.yellow;
            l.enabled = true;
        }
    }

    IEnumerator EmergencyLightBlink()
    {
        while (emergencyBlinking)
        {
            float blink = Mathf.Abs(Mathf.Sin(Time.time * emergencyBlinkSpeed));
            foreach (Light l in emergencyLights)
            {
                if (l == null) continue;
                l.intensity = Mathf.Lerp(0f, emergencyIntensity, blink);
            }
            yield return null;
        }
    }

    // Detiene el titileo de las luces de emergencia y aumenta su intensidad.
    public void StopEmergencyBlinkAndBoost()
    {
        emergencyBlinking = false;
        StopCoroutine(EmergencyLightBlink());

        foreach (Light l in emergencyLights)
        {
            if (l == null) continue;
            l.intensity = emergencyIntensity * 2f;
            l.color = Color.red;
            l.enabled = true;
        }
    }

    public void TurnOffEmergencyLights()
    {
        emergencyBlinking = false;
        StopAllCoroutines();

        foreach (Light l in emergencyLights)
        {
            if (l == null) continue;
            l.enabled = false;
        }

        foreach (Light l in generalLights)
        {
            if (l == null) continue;
            l.enabled = false;
        }
    }

    public void ShutDownControlLight()
    {
        StartCoroutine(SequentialControlLightOff());
    }

    IEnumerator SequentialControlLightOff()
    {
        for (int i = 0; i < controlRoomLights.Length; i++)
        {
            Light l = controlRoomLights[i];
            if (l == null) continue;

            float startIntensity = l.intensity;
            float t = 0f;
            while (t < 1f)
            {
                l.intensity = Mathf.Lerp(startIntensity, 0f, t);
                t += Time.deltaTime / controlLightOffDelay;
                yield return null;
            }

            l.intensity = 0f;
            l.enabled = false;

            yield return new WaitForSeconds(controlLightOffDelay);
        }
    }

    public void RestoreAllLights()
    {
        emergencyBlinking = false;
        StopAllCoroutines();

        // Luces de emergencia
        foreach (Light l in emergencyLights)
        {
            if (l == null) continue;
            l.enabled = true;
            l.intensity = emergencyIntensity;
            l.range = emergencyRange;
            l.color = Color.white;
        }

        // Luces generales
        foreach (Light l in generalLights)
        {
            if (l == null) continue;
            l.enabled = true;
            l.intensity = generalIntensity;
            l.range = generalRange;
            l.color = Color.white;
        }

        // Luces de control
        foreach (Light l in controlRoomLights)
        {
            if (l == null) continue;
            l.enabled = true;
            l.intensity = controlLightIntensity;
            l.range = controlLightRange;
            l.color = Color.white;
        }

    }
}
