using System.Collections;
using Cysharp.Threading.Tasks.Triggers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : NetworkBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    [SerializeField] private float wanderRadius = 50f;
    private bool isDead = false;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 애니메이션 재생 속도 <- 사용자 입력으로 동작하는 상태가 아니기에 수동으로 입력
        anim.SetFloat("MotionSpeed", 1);

        StartCoroutine(MovingRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
            return;
        
        var hitbox = other.GetComponent<Hitbox>();

        if (hitbox != null)
        {
            NetworkObject otherUser = other.transform.root.GetComponent<NetworkObject>();
            string otherID = otherUser.OwnerClientId.ToString();
            
                LogManager.Instance.SetLogMessage(otherID, "NPC", false);
            
            GetHit();
        }
    }
    
    IEnumerator MovingRoutine()
    {
        while (true)
        {
            SetRandomDestination();
            int randMove = Random.Range(0, 2); // 0 : Walk, 1: Run
            agent.speed = randMove == 0 ? 2f : 6f; // 이동 속도 적용
            anim.SetFloat("Speed", agent.speed); // 애니메이션 적용
            
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
            
            anim.SetFloat("Speed",0);
            float waitTime = Random.Range(2f, 5f); // 이동 후 잠시 멈추는 시간
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SetRandomDestination()
    {
        var randDir = Random.insideUnitSphere * wanderRadius;
        randDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
    
    private void GetHit()
    {
        isDead = true;
        anim.SetTrigger("Death");
        agent.isStopped = true;
        agent.ResetPath();
        agent.speed = 0;
        StopAllCoroutines();
    }
}
