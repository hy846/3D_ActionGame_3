using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                int attackDamage = GetComponentInParent<PlayerController>().attackDamage;
                enemy.TakeDamage(attackDamage);
            }
        }
    }
}
