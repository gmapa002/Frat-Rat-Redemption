using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask theGround, thePlayer; 

    public float health;

    // for patrolling
    public Vector3 enemywalkPoint;
    bool walkPointSet;
    public float walkPointRange;

    private Animator ani;

    //Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public void Awake()
    {
        player = GameObject.Find("fratRat").transform;
        agent = GetComponent<NavMeshAgent>();
        ani = GetComponentInChildren<Animator>(); 
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, thePlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, thePlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) Attackplayer();
    } 

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(enemywalkPoint);

        Vector3 distanceToWalkPoint = transform.position - enemywalkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) 
            walkPointSet = false;

    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange); 

        enemywalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(enemywalkPoint, - transform.up, 2f, theGround)) 
            walkPointSet = true;
    }

    private void ChasePlayer() 
    {
        agent.SetDestination(player.position);
    }
    
    private void Attackplayer() 
    {
        //make sure enenmy does not move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        ani.SetTrigger("AttackEnemyCombo");

        if (!alreadyAttacked)
        {

            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);


        }
    }

    private void ResetAttack() 
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) 
        {
            Invoke(nameof(DestroyEnemy), 2f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
