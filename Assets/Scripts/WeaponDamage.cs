using UnityEngine;
using System.Collections.Generic;

public class WeaponDamage : MonoBehaviour
{ 
    [SerializeField] private int damage = 10;
    [SerializeField] private Collider2D swordCollider;

    private bool canDamage = false;
    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    private void Awake()
    {
        if (swordCollider == null)
            swordCollider = GetComponent<Collider2D>();
    }

    public void StartDamage()
    {
        hitEnemies.Clear();
        canDamage = true;
    }

    public void StopDamage()
    {
        canDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (!canDamage)
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null && hitEnemies.Add(enemy))
        {
            enemy.TakeDamage(damage);
        }
    }
}

