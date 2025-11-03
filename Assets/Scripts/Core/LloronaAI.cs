using UnityEngine;
using UnityEngine.AI;

public class LloronaAI : MonoBehaviour
{
    public Camera Camtarget; // El objetivo a seguir
    public float stoppingDistance = 1f; // Distancia para detenerse
    public float followSpeed = 3.5f; // Velocidad de seguimiento
    public Vector3 target;
    private NavMeshAgent agent;
    private Plane[] planes;
    private GameData.LloronaState state;
    public Vector3 spawnPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        state = GameData.LloronaState.STAY;
        spawnPoint = transform.position;
    }

    public void setSpawnPoint(Vector3 pos)
    {
        spawnPoint = pos;
    }

    public void reSpawnPoint()
    {
        transform.position = spawnPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if ((state ==  GameData.LloronaState.ALERT ||  state ==  GameData.LloronaState.RUNNING) && !IsTargetVisible())
        {
                FollowTarget();
        }
        else
        {
            // detener al agente si vuelve a ser visible
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
    public bool IsTargetVisible()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camtarget); //obtener el arreglo de planos del frutum
        if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds))
            //GetComponent<Collider>().bounds es el bounding de la chillona, si no intersecta con los planos del frustum entonces no lo ve
            return false;
        Vector3 origin = Camtarget.transform.position; //origen del rayo es la posicion de la camara
        Vector3 final = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z); //posicion de la chillona
        Vector3 direction = (final - origin).normalized; //direccion del rayo es desde la camara hacia el objetivo
        float distance = Vector3.Distance(origin, transform.position); //distancia entre la camara y el objetivo
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            Debug.DrawLine(origin, hit.point, Color.yellow, 3f); // línea hasta donde choca el rayo 

            if (hit.collider.gameObject != gameObject)
                // el disparo se lanza del player a la chillone
                // en este sentido, si no choca con la chillona, es que hay un obstáculo en medio
                return false;
        }
        else
        {
            Debug.DrawLine(origin, transform.position, Color.green, 3f); // sin obstáculo
        }
        return true;

    }

    public void changeState(GameData.LloronaState newState)
    {
        state = newState;
    }
    public void OnNoiseHeard(Vector3 pos, float level)
    {
        if (state == GameData.LloronaState.STUN) return;
        if (level > 0.75f)
        {
            state = GameData.LloronaState.RUNNING;
            followSpeed = 3.5f;
            target = Camtarget.transform.position;
            return;
        }else  if (level > 0.4f)
        {
            state = GameData.LloronaState.ALERT;
            target = pos;
            followSpeed = 1.5f;
            return;
        }
        state = GameData.LloronaState.STAY;
        return;
    }

    void FollowTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target);
        if (distanceToTarget > stoppingDistance)
        {
            agent.isStopped = false;
            agent.speed = followSpeed;
            agent.SetDestination(target);
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
