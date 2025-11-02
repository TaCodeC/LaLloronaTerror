using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class GameCore : MonoBehaviour
{
    public static GameCore Instance { get; private set; }
    public LloronaAI chillona;
    [Header("Mensajes UI")]
    public GameObject keyMessage; 
    public TMP_Text Instructions;
    public float messageDuration = 5f;
    [Header("UIPause")]
    public GameObject pauseMenu;
    [Header("ActualGameState")]
    [SerializeField] private GameData.GameState gameState;
    [SerializeField] private GameData.Objective objective;
    [SerializeField] private GameData.demiObjective demiObjective;
    
    private PlayerActions.Inventory playerInventoryToRespawn;
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
        playerInventoryToRespawn = PlayerActions.Instance.playerInventory;
        PlayerActions.Instance.setRespawnPoint();
    }

    public void orderRespawn()
    {
        PlayerActions.Instance.playerInventory = playerInventoryToRespawn;
        PlayerActions.Instance.reSpawnPoint();
        PlayerActions.Instance.UpdateUI();
        chillona.reSpawnPoint();
        chillona.changeState(GameData.LloronaState.STAY);
        demiObjective = GameData.demiObjective.SEARCH;
        UpdateUIInstructions();
        //TODO AGREGAR EL RESPAWN DE LOS ITEMS
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
        pauseMenu.SetActive(false);
        gameState = GameData.GameState.PLAYING;
        objective = GameData.Objective.ENERGY;
        demiObjective = GameData.demiObjective.SEARCH;
    }
    public bool confirmKey()
    {
        if (PlayerActions.Instance.HasItem(GameData.ItemType.Key))
        {
            PlayerActions.Instance.playerInventory.hasKey = false;
            PlayerActions.Instance.UpdateUI();
            return true;
        }
        ShowKeyMessage();
        return false;
    }

    public void ShowKeyMessage()
    {
        if (keyMessage == null) return;

        if (keyMessage.activeSelf) return; 

        StartCoroutine(ShowKeyMessageRoutine());
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
            case GameData.Objective.ENERGY:
                if (demiObjective == GameData.demiObjective.SEARCH)
                {
                    Instructions.text ="Busca la bateria de Emergencia";
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
                    Instructions.text = "Ve a la sala de control";
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

    IEnumerator ShowKeyMessageRoutine()
    {
        keyMessage.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        keyMessage.SetActive(false);
    }
}