using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public int attackDamage = 10;
    public int maxHealth = 100;
    public int currentHealth;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float maxDistance = 10f;
    public float timeToDestroy = 2f;
    private float attackCooldown = 2.0f;
    private float attackTimer = 0.0f; 
    public Slider slider;
    public Canvas canvas;
    public GameObject swordObject;
    private Transform player;
    private Animator animator;
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private Collider swordCollider;
    private bool isAttacking;
    private bool isDead = false;


    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        swordCollider = swordObject.GetComponent<Collider>();
        swordCollider.enabled = false;
        SetNewRandomDestination();
        slider.value = 1;
    }

    private void Update()
    {
        canvas.transform.rotation = Camera.main.transform.rotation;
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        if (isAttacking)
        {
            swordCollider.enabled = true;
        }
        else
        {
            swordCollider.enabled = false;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, player.position) <= attackRange && attackTimer <= 0)
        {
            agent.isStopped = true;
            animator.SetTrigger("Idle");
            StartCoroutine(ResumeMovementAfterDelay(1.0f));
            animator.SetTrigger("Attack");
            attackTimer = attackCooldown;
        }
        else if (Vector3.Distance(transform.position, player.position) <= attackRange && attackTimer > 0)
        {
            animator.SetTrigger("Idle");
            agent.ResetPath();
        }
        else if (Vector3.Distance(transform.position, player.position) <= attackRange * 2f)
        {
            animator.SetBool("isRunning", true);
            transform.LookAt(player.position);
            agent.SetDestination(player.position);
        }
        else
        {
            animator.SetBool("isRunning", true);
            if (agent.remainingDistance < 0.5f)
            {
                SetNewRandomDestination();
            }
        }
    }

    void SetNewRandomDestination() {
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, maxDistance, 1);
        targetPosition = hit.position;
        agent.SetDestination(targetPosition);
    }

    IEnumerator ResumeMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.isStopped = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth);
        slider.value = (float)currentHealth / (float)maxHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("Dead", true);
        StartCoroutine(DestroyAfterDelay(timeToDestroy));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        
    }
}
