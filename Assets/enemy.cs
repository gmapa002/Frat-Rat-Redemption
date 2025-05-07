// using UnityEngine;
// using UnityEngine.AI;

// public class EnemyController : MonoBehaviour
// {
//     public float lookRadius = 10f; // Detection range for the player
//     public Transform player; // Reference to the player

//     private NavMeshAgent agent;

//     void Start()
//     {
//         // Get the NavMeshAgent component
//         agent = GetComponent<NavMeshAgent>();

//         // Optionally, set the player reference manually or through an inspector
//         if (player == null)
//         {
//             player = GameObject.FindWithTag("ThirdPersonController").transform; // Assumes the player is tagged "Player"
//         }
//     }

//     void Update()
//     {
//         // Check if the player is within the look radius
//         float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
//         if (distanceToPlayer <= lookRadius)
//         {
//             // Move towards the player
//             agent.SetDestination(player.position);
//         }
//         else
//         {
//             // Optionally, make the enemy stop when the player is out of range
//             agent.SetDestination(transform.position);
//         }
//     }
// }

using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f; // Detection range for the player
    public Transform player; // Reference to the player

    private NavMeshAgent agent;

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        // Optionally, set the player reference manually or through an inspector
        if (player == null)
        {
            player = GameObject.FindWithTag("ThirdPersonController").transform; // Assumes the player is tagged "ThirdPersonController"
            if (player == null)
            {
                Debug.LogError("Player not found! Check the tag.");
            }
        }
    }

    void Update()
    {
        // Check if the player is within the look radius
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        Debug.Log("Distance to player: " + distanceToPlayer);  // Debug log

        if (distanceToPlayer <= lookRadius)
        {
            // Move towards the player
            agent.SetDestination(player.position);
            Debug.Log("Moving towards player");  // Debug log
        }
        else
        {
            // Optionally, make the enemy stop when the player is out of range
            agent.SetDestination(transform.position);
            Debug.Log("Player out of range");  // Debug log
        }

        if (agent.hasPath)
        {
            Debug.Log("Enemy is following the player.");
        }
        else
        {
            Debug.Log("Enemy has no path.");
        }
    }
}
