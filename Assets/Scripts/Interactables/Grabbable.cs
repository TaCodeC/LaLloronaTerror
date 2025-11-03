using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour
{
    private GameData.ItemType ITEM_ID;
    [SerializeField] private GameData.ItemType itemType;
    [Header("Si interactable, tipo , duracion y ang (puerta), y ref de la puerta")]
    [SerializeField] private GameData.Interactables interactable = GameData.Interactables.None;

    [SerializeField] private GameObject batteryToActivate;
    [SerializeField] private GameObject gateToActivate;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float openAngle = 90f;
    private bool isOpen = false;
    private bool hasBeenOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private Coroutine currentCoroutine;

    [Header("Si es cassete, que N. de casette es")]
    [SerializeField] private int CassetN;

    void Start()
    {
        ITEM_ID = itemType;
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f));
        if(batteryToActivate != null) batteryToActivate.SetActive(false);
        if(gateToActivate != null) gateToActivate.SetActive(false);
    }

    public GameData.ItemType onGrab()
    {
        if (ITEM_ID != GameData.ItemType.Interaction)
        {
            gameObject.SetActive(false);
            return ITEM_ID;
        }

        manageInteraction();
        return ITEM_ID;
    }

    private void manageInteraction()
    {
        switch (interactable)
        {

            case GameData.Interactables.LockedDoor:
                if (hasBeenOpen)
                {
                    ToggleDoor();
                    break;
                }

                if (GameCore.Instance.confirmKey())
                {
                    ToggleDoor();
                    PlayerActions.Instance.playerInventory.hasKey = false;
                    PlayerActions.Instance.UpdateUI();
                }
                break;
            case GameData.Interactables.LockedDoorControl:
                if (hasBeenOpen)
                {
                    ToggleDoor();
                    break;
                }

                if (GameCore.Instance.confirmKey() && GameCore.Instance.objective == GameData.Objective.CONTROL)
                {
                    ToggleDoor();
                    GameCore.Instance.setObjective(GameData.Objective.ESCAPE);
                    PlayerActions.Instance.playerInventory.hasKey = false;
                    PlayerActions.Instance.UpdateUI();
                }
                break;
            case GameData.Interactables.InstallBattery:
                if (GameCore.Instance.confirmBattery())
                {
                    if (batteryToActivate != null)
                        batteryToActivate.SetActive(true);
                    GameCore.Instance.setObjective(GameData.Objective.DOOR);
                    AudioManager.Instance.insertBattery();
                    LightsManager.Instance.StopEmergencyBlinkAndBoost();
                }
                break;
            case GameData.Interactables.Gate:
                if (GameCore.Instance.confirmGate())
                {
                    if (gateToActivate != null)
                        gateToActivate.SetActive(true);
                    gameObject.SetActive(false);
                    AudioManager.Instance.reproduceClip(AudioManager.Instance.gate);
                    GameCore.Instance.setObjective(GameData.Objective.CONTROL);
                }

                break;
                case GameData.Interactables.Cassete:
                    AudioManager.Instance.reproduceCassette(CassetN);
                    gameObject.SetActive(false);
                    if(PlayerActions.Instance.addCassete()) GameCore.Instance.setDemiObjective(GameData.demiObjective.USE);
                    break;
                case GameData.Interactables.ControlButtons:
                    if (PlayerActions.Instance.pressControlBtn())
                    {
                        AudioManager.Instance.reproduceClip(AudioManager.Instance.escapeAudio);
                    }
                    break;
                case GameData.Interactables.escape :
                        if (GameCore.Instance.objective == GameData.Objective.ESCAPE &&
                            GameCore.Instance.demiObjective == GameData.demiObjective.USE)
                        {
                            GameCore.Instance.changetoMenu();
                        }
                    break;
        }
    }

    private IEnumerator RotateDoor(Quaternion from, Quaternion to)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
            yield return null;
        }
        transform.rotation = to;
        currentCoroutine = null;
    }

    public void ToggleDoor()
    {
        if (currentCoroutine != null) return;
        Quaternion fromRot = isOpen ? openRot : closedRot;
        Quaternion toRot = isOpen ? closedRot : openRot;
        currentCoroutine = StartCoroutine(RotateDoor(fromRot, toRot));
        isOpen = !isOpen;
        hasBeenOpen = true;
        AudioManager.Instance.reproduceClip(AudioManager.Instance.openDoor);
    }
}