using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    // Singleton
    public static PlayerActions Instance { get; private set; }

    [Header("Iconos de inventario")]
    public GameObject uiInventoryBattery;
    public GameObject uiInventoryPliers;
    public GameObject uiInventoryFlashlight;
    public GameObject uiInventoryKey;
    public GameObject uiInventoryCameraBattery;
    [Header("Límite de baterías de cámara")]
    [Min(0)] public int maxCameraBatteries = 4;

    private Vector3 respawnPoint;


    //  struct holder 
    public struct Inventory
    {
        public bool hasBattery;
        public bool hasPliers;
        public bool hasFlashlight;
        public bool hasKey;
        public int cameraBatteryCount;
        
        public void AddItem(GameData.ItemType itemType, int maxCamBats)
        {
            switch (itemType)
            {
                case GameData.ItemType.Battery:      hasBattery = true;      break;
                case GameData.ItemType.Pliers:       hasPliers = true;       break;
                case GameData.ItemType.Flashlight:   hasFlashlight = true;   break;
                case GameData.ItemType.Key:          hasKey = true;          break;
                case GameData.ItemType.CameraBattery:
                    cameraBatteryCount = Mathf.Min(cameraBatteryCount + 1, maxCamBats);
                    break;
            }
        }


        public void minusBattery()
        {
            if(cameraBatteryCount>0) cameraBatteryCount--;
            
        }


        public bool HasItem(GameData.ItemType itemType)
        {
            return itemType switch
            {
                GameData.ItemType.Battery       => hasBattery,
                GameData.ItemType.Pliers        => hasPliers,
                GameData.ItemType.Flashlight    => hasFlashlight,
                GameData.ItemType.Key           => hasKey,
                GameData.ItemType.CameraBattery => cameraBatteryCount > 0,
                _ => false
            };
        }
    }

    public Inventory playerInventory;
    public void setRespawnPoint()
    {
        respawnPoint = transform.position;
    }

    public void reSpawnPoint()
    {
        transform.position = respawnPoint;
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerInventory = new Inventory();
        UpdateUI();
    }
/*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerInventory.HasItem(GameData.ItemType.Flashlight))
        {
            bool active  = FlashLight.activeSelf;
            FlashLight.SetActive(!active);
        }

        if (FlashLight.activeSelf)
        {
            batteryCounter -= Time.deltaTime;
            if (batteryCounter <= 0f)
            {
                if (playerInventory.cameraBatteryCount > 1)
                {
                    playerInventory.minusBattery();
                    batteryCounter = batterieDuration;
                    UpdateUI();
                }
                else
                {
                    FlashLight.SetActive(false);
                    UpdateUI();
                }
            }
        }
        
    }
*/
    // Api Func
    public void AddItem(GameData.ItemType itemType)
    {
        if (itemType == GameData.ItemType.Interaction) return;
        playerInventory.AddItem(itemType, maxCameraBatteries);
        UpdateUI();
    }

    public bool HasItem(GameData.ItemType itemType) => playerInventory.HasItem(itemType);

    public void UpdateUI()
    {
        static void SetAlpha(GameObject go, bool enabled)
        {
            if (!go) return;
            var img = go.GetComponent<Image>();
            if (!img) return;
            var c = img.color;
            c.a = enabled ? 1f : 0.49f;
            img.color = c;
            Debug.Log($"SetAlpha {go.name} to {(enabled ? "enabled" : "disabled")}");
        }

        void setBatteries(GameObject go, bool enabled)
        {
            if (!go) return;
            var img = go.GetComponent<Image>();
            img.fillAmount = enabled ? Mathf.Clamp((float)playerInventory.cameraBatteryCount / maxCameraBatteries, 0f, 1f) : 0f;
        }

        SetAlpha(uiInventoryBattery,       playerInventory.hasBattery);
        SetAlpha(uiInventoryPliers,        playerInventory.hasPliers);
        SetAlpha(uiInventoryFlashlight,    playerInventory.hasFlashlight);
        SetAlpha(uiInventoryKey,           playerInventory.hasKey);
        setBatteries(uiInventoryCameraBattery, playerInventory.cameraBatteryCount >= 0);
    }
}
