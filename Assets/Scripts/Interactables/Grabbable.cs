using UnityEngine;
using System.Collections;

public class Grabbable : MonoBehaviour
{
    private GameData.ItemType ITEM_ID;
    [SerializeField] private GameData.ItemType itemType;
    [Header("Si interactable, tipo , duracion y ang (puerta)")]
    [SerializeField] private GameData.Interactables interactable = GameData.Interactables.None;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float openAngle = 90f;
    private bool isOpen = false;
    private bool hasBeenOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;
    private Coroutine currentCoroutine;

    void Start()
    {
        ITEM_ID = itemType;
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f));
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
            case GameData.Interactables.Door:
                ToggleDoor();
                break;
            case GameData.Interactables.LockedDoor:
                if (hasBeenOpen)
                {
                    ToggleDoor();
                    break;
                }

                if (GameCore.Instance.confirmKey())
                {
                    ToggleDoor();
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
    }
}