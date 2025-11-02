using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Mov : MonoBehaviour
{
    public string speedParam = "Speed";
    public float smoothTime = 0.2f; // tiempo de suavizado
    public float moveSpeed = 4f; // velocidad máxima esperada

    private Animator anim;
    private int hashSpeed;

    private float currentSpeed;
    private float velocityRef;

    private Vector3 lastPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        hashSpeed = Animator.StringToHash(speedParam);
        lastPos = transform.position;
    }

    void Update()
    {
        //  desplazamiento entre frames
        Vector3 delta = transform.position - lastPos;
        float rawSpeed = delta.magnitude / Time.deltaTime;
        float targetSpeed = Mathf.Clamp01(rawSpeed / moveSpeed);

        // Suaviza
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocityRef, smoothTime);

        // Envía al Animator
        anim.SetFloat(hashSpeed, currentSpeed);
        lastPos = transform.position;

        //  Bloquear rot en x y z
        Vector3 rot = transform.eulerAngles;
        transform.localPosition = new Vector3(0, 0, -0.5f);
        transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
    }
}
