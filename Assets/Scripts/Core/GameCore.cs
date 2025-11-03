using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance { get; private set; }
    public LloronaAI chillona;
    [Header("Mensajes UI")]
    public GameObject keyMessage;
    public GameObject batteryMessage;
    public TMP_Text Instructions;
    public float messageDuration = 5f;
    [Header("UIPause")]
    public GameObject pauseMenu;
    [Header("ActualGameState")]
    [SerializeField] private GameData.GameState gameState;

    [SerializeField] public GameData.Objective objective;
    [SerializeField] public GameData.demiObjective demiObjective;

    private PlayerActions.Inventory playerInventoryToRespawn;

    [Header("Ultima Mision")]
    public TMP_Text countdownText;
    public float countdownDuration = 10f;
    private float countdownTimer;

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

    public void orderSetSpawnPoint()
    {
        PlayerActions.Instance.setRespawnPoint();
    }

    public void orderRespawn()
    {
        PlayerActions.Instance.reSpawnPoint();
        PlayerActions.Instance.UpdateUI();
        chillona.reSpawnPoint();
        chillona.changeState(GameData.LloronaState.STAY);
    }

    public void changetoMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if (pauseMenu.activeSelf)
            {
                Time.timeScale = 0;
                gameState = GameData.GameState.PAUSE;
            }
            else
            {
                Time.timeScale = 1;
                gameState = GameData.GameState.PLAYING;
            }
        }

    }

    void Start()
    {
        keyMessage.SetActive(false);
        batteryMessage.SetActive(false);
        pauseMenu.SetActive(false);
        gameState = GameData.GameState.PLAYING;
        objective = GameData.Objective.FLASHLIGHT;
        demiObjective = GameData.demiObjective.SEARCH;
        UpdateUIInstructions();
        countdownTimer = countdownDuration;
    }

    public bool confirmKey()
    {
        if (PlayerActions.Instance.HasItem(GameData.ItemType.Key))
        {
            PlayerActions.Instance.playerInventory.hasKey = false;
            PlayerActions.Instance.UpdateUI();
            return true;
        }
        ShowMessage(keyMessage);
        return false;
    }

    public bool confirmBattery()
    {
        if (PlayerActions.Instance.HasItem(GameData.ItemType.Battery))
        {
            return true;
        }
        ShowMessage(batteryMessage);
        return false;
    }

    public bool confirmGate()
    {
        if (objective == GameData.Objective.DOOR && PlayerActions.Instance.HasItem(GameData.ItemType.Pliers))
        {
            return true;
        }
        return false;
    }

    public void ShowMessage(GameObject message)
    {
        if (message == null) return;
        if (message.activeSelf) return;
        StartCoroutine(ShowMessageRoutine(message));
    }

    public void setDemiObjective(GameData.demiObjective _demiObjective)
    {
        demiObjective = _demiObjective;
        UpdateUIInstructions();
    }

    public void setObjective(GameData.Objective _objective)
    {
        objective = _objective;
        setDemiObjective(GameData.demiObjective.SEARCH);
        UpdateUIInstructions();
    }

    private void UpdateUIInstructions()
    {
        switch (objective)
        {
            case GameData.Objective.FLASHLIGHT:
                if (demiObjective == GameData.demiObjective.SEARCH)
                {
                    Instructions.text = "Busca la linterna y una bateria";
                }
                else
                {
                    Instructions.text = "Usa la linterna con F";
                }
                break;
            case GameData.Objective.ENERGY:
                if (demiObjective == GameData.demiObjective.SEARCH)
                {
                    Instructions.text = "Busca la bateria de Emergencia";
                }
                else
                {
                    Instructions.text = "Instala la  bateria de Emergencia";
                }
                break;
            case GameData.Objective.DOOR:
                if (demiObjective == GameData.demiObjective.SEARCH)
                {
                    Instructions.text = "Busca las pinzas de corte";
                }
                else
                {
                    Instructions.text = "Corta las cadenas de la puerta";
                }
                break;
            case GameData.Objective.CONTROL:
                if (demiObjective == GameData.demiObjective.SEARCH)
                {
                    Instructions.text = "Busca las llaves de la sala de Control";
                }
                else
                {
                    Instructions.text = "Toma las laves y ve a la sala de control";
                }
                break;
            case GameData.Objective.ESCAPE:
                if (demiObjective == GameData.demiObjective.SEARCH)
                {
                    Instructions.text = "Activa la terminal de control";
                }
                else
                {
                    Instructions.text = "Escapa en el tren";
                }
                break;
        }
    }

    IEnumerator ShowMessageRoutine(GameObject message)
    {
        message.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        message.SetActive(false);
    }
}
