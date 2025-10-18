using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AITargeting : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform wayPoints;
    private int currentWaypoint;

    public Transform Target;
    public float AttackDistance;

    private NavMeshAgent m_agent;
    private Animator m_animator;
    private float m_Distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        //m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Distance = Vector3.Distance(m_agent.transform.position, Target.position);

        if (m_Distance > AttackDistance)
        {
            //m_agent.isStopped = true;
            if (m_agent.remainingDistance <= 0.2f)
            {
                currentWaypoint++;
                if (currentWaypoint >= wayPoints.childCount)
                {
                    currentWaypoint = 0;
                }

                m_agent.SetDestination(wayPoints.GetChild(currentWaypoint).position);
            }
        }
        else
        {
            m_agent.isStopped = false;
            m_agent.destination = Target.position;
        }
    }
}
