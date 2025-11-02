using UnityEngine;
using UnityEngine.UI;

public class InteractHold : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 5f;
    public LayerMask interactableLayer;

    // UI References
    public GameObject uiElementToActivate;
    public Image uiFillImage;

    public float holdDuration = 2f;

    private float holdTimer = 0f;
    private bool isLookingAtInteractable = false;
    private Grabbable currentGrabbable = null;

    public void checkInteraction()
    {
        CheckRaycast();

        if (isLookingAtInteractable)
        {
            uiElementToActivate.SetActive(true);
            HandleHoldInput();
        }
        else
        {
            uiElementToActivate.SetActive(false);
            ResetHold();
        }
    }

    private void CheckRaycast()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.yellow);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            currentGrabbable = hit.collider.GetComponent<Grabbable>();
            isLookingAtInteractable = (currentGrabbable);
        }
        else
        {
            currentGrabbable = null;
            isLookingAtInteractable = false;
        }
    }

    private void HandleHoldInput()
    {
        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;
            float normalized = Mathf.Clamp01(holdTimer / holdDuration);
            uiFillImage.fillAmount = normalized;

            if (holdTimer >= holdDuration)
            {
                // Se completó la acción
                PlayerActions.Instance.AddItem(currentGrabbable.onGrab());
                Debug.Log("Item: " + currentGrabbable.onGrab());
                ResetHold();
            }
        }
        else
        {
            // No se está manteniendo la tecla: resetear
            if (holdTimer > 0f)
            {
                ResetHold();
            }
        }
    }

    private void ResetHold()
    {
        holdTimer = 0f;
        uiFillImage.fillAmount = 0f;
    }
}
