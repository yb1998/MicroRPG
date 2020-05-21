using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int curHP;
    public int maxHP;
    public float moveSpeed;
    public int xpToGive;

    private Vector2 facingDirection;

    [Header("Target")]
    public float chaseRange;
    public float attackRange;
    private Player player;

    [Header("Attack")]
    public int damage;
    public float attackRate;
    float lastAttackTime;

    //component
    private Rigidbody2D rig;

    void Awake()
    {
        //get the player target
        player = FindObjectOfType<Player>();

        //get the rigidbody component
        rig = GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        //calculate the distance between us and the player
        float playerDist = Vector2.Distance(transform.position, player.transform.position);

        //if we're in attack range, try and attack the player
        if(playerDist <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackRate)
            {
                Attack();
            }
            rig.velocity = Vector2.zero;
        }
        //if we're in the chase range, chase after the player
        else if (playerDist <= chaseRange)
        {
            Chase();
        }
        else
        {
            rig.velocity = Vector2.zero;
        }
    }

    //move towards the player
    void Chase()
    {
        //calculate direction between us and the player
        Vector2 dir = (player.transform.position - transform.position).normalized;

        rig.velocity = dir * moveSpeed;
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        player.TakeDamage(damage);
    }

    public void TakeDamage(int damageTaken)
    {
        curHP -= damageTaken;

        if (curHP <= 0)
            Die();
    }

    void Die()
    {
        //gice the player xp
        player.AddXp(xpToGive);
        Destroy(gameObject);

    }
}
