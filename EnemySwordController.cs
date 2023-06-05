using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                if (player.isGuarding)
                {
                    return;
                }
                else
                {
                    int attackDamage = GetComponentInParent<EnemyController>().attackDamage;
                    player.TakeDamage(attackDamage);
                }
            }
        }
    }
}
