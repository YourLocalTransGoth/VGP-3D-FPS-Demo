using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    private Animator anim;

    public NavMeshAgent agent;
    public Transform player;

    public GameObject AttackPrefab;

   
    public LayerMask whatISplayer;

    //patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;


    //Attacking 
    public float timeBetweenAttacks;
    public float timeBetweenDeathsSETValue = 1f;
    bool alreadyAttacked;
    public float attackSpeedForward = 5f;
    public float attackSpeedUp = 2f;


    //states
    public float sightRange;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();


    }



    void start()
    {
        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
    }




    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatISplayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatISplayer);

        if (playerInSightRange == false && playerInAttackRange == false)
        {
            Patrolling();
        }
        else if (playerInSightRange == true && playerInAttackRange == false)
        {
            ChasePlayer();
        }
        else if (playerInSightRange == true && playerInAttackRange == true)
        {
            AttackPlayer();
        }
        if (timeBetweenAttacks > 0)
        {
            alreadyAttacked = true;
            timeBetweenAttacks = timeBetweenAttacks - Time.deltaTime;

        }
        else { alreadyAttacked = false; }

    }

    void Patrolling()
    {
        if (walkPointSet == false)
        {
            GenerateWalkPoint();
            walkPointSet = true;
        }
        else if (walkPointSet == true)
        {
            agent.SetDestination(walkPoint);
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }


    void AttackPlayer()
    {
        agent.SetDestination(player.position);
        transform.LookAt(player);
        if (alreadyAttacked == false)
        {
            Attack();

            timeBetweenAttacks = timeBetweenDeathsSETValue;
        }
    }

    void GenerateWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    }

    void Attack()
        {
        
            // anim.SetTrigger("attack");
            Rigidbody rb = Instantiate(AttackPrefab, transform.position + transform.forward * 1f, Quaternion.identity).GetComponent<Rigidbody>();
            rb.gameObject.SetActive(true);
            rb.AddForce(transform.forward * attackSpeedForward, ForceMode.Impulse);
            rb.AddForce(transform.up * attackSpeedUp, ForceMode.Impulse);
       
        }

   



}
